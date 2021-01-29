using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Application.Options;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;
using BytexDigital.Steam.ContentDelivery;
using BytexDigital.Steam.ContentDelivery.Exceptions;
using BytexDigital.Steam.ContentDelivery.Models;
using BytexDigital.Steam.Core;
using BytexDigital.Steam.Core.Enumerations;
using BytexDigital.Steam.Core.Exceptions;
using BytexDigital.Steam.Core.Structs;

using Microsoft.Extensions.Options;

using Nito.AsyncEx;

using SteamKit2.Unified.Internal;

namespace BytexDigital.RGSM.Node.Application.Core.Steam
{
    public class SteamDownloadService : IDisposable
    {
        private readonly ConcurrentQueue<UpdateItem> _downloadQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly HttpClient _httpClient;
        private List<SteamLoginDto> _steamCredentials;
        private ConcurrentDictionary<string, (SteamLoginDto Credentials, SteamClient Client, SteamContentClient ContentClient)> _steamClients;
        private ConcurrentDictionary<string, DateTimeOffset> _rateLimitedAccounts;
        private ConcurrentBag<string> _invalidLoginKeys;
        private Task _workTask;
        private AsyncLock _clientsLock;

        public SteamDownloadService(IOptions<NodeOptions> nodeOptions, HttpClient httpClient)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _clientsLock = new AsyncLock();
            _downloadQueue = new ConcurrentQueue<UpdateItem>();
            _invalidLoginKeys = new ConcurrentBag<string>();
            _steamClients = new ConcurrentDictionary<string, (SteamLoginDto Credentials, SteamClient Client, SteamContentClient ContentClient)>();
            _rateLimitedAccounts = new ConcurrentDictionary<string, DateTimeOffset>();
            _httpClient = httpClient;
        }

        public async Task InitializeAsync()
        {
            await RefreshCredentialsAsync();
        }

        public async Task RefreshCredentialsAsync()
        {
            var response = await _httpClient.GetAsync("API/Steam/GetLogins");

            var str = await response.Content.ReadAsStringAsync();

            _steamCredentials = await _httpClient.GetFromJsonAsync<List<SteamLoginDto>>("API/Steam/GetLogins");
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        public Task<UpdateState> DownloadPublishedFileAsync(AppId appId, PublishedFileId publishedFileId, string directory, bool useAnonymous)
        {
            var updateState = new UpdateState
            {
                CancellationToken = new CancellationTokenSource(),
                Progress = 0,
                State = UpdateState.Status.Queued,
                ProcessedEvent = new Nito.AsyncEx.AsyncManualResetEvent(false)
            };

            _downloadQueue.Enqueue(new UpdateItem
            {
                UpdateState = updateState,
                CancellationToken = updateState.CancellationToken.Token,

                AppId = appId,
                PublishedFileId = publishedFileId,
                UseAnonymousUser = useAnonymous,
                Directory = directory
            });

            if (_workTask == default || _workTask.IsCompleted)
            {
                _workTask = Task.Run(async () => await WorkAsync());
            }

            return Task.FromResult(updateState);
        }

        public Task<UpdateState> DownloadAppIdAsync(AppId appId, Func<Depot, bool> depotCondition, string directory, string branch, string branchPassword, bool useAnonymousUser)
        {
            var updateState = new UpdateState
            {
                CancellationToken = new CancellationTokenSource(),
                Progress = 0,
                State = UpdateState.Status.Queued,
                ProcessedEvent = new Nito.AsyncEx.AsyncManualResetEvent(false)
            };

            _downloadQueue.Enqueue(new UpdateItem
            {
                UpdateState = updateState,
                CancellationToken = updateState.CancellationToken.Token,
                DepotCondition = depotCondition,

                AppId = appId,
                Directory = directory,
                Branch = branch,
                BranchPassword = branchPassword,
                UseAnonymousUser = useAnonymousUser
            });

            if (_workTask == default || _workTask.IsCompleted)
            {
                _workTask = Task.Run(async () => await WorkAsync());
            }

            return Task.FromResult(updateState);
        }

        public async Task<PublishedFileDetails> GetPublishedFileDetailsAsync(AppId appId, PublishedFileId publishedFileId)
        {
            (var credentials, var client, var contentClient) = await GetSteamClientAsync(appId, true);

            return await contentClient.GetPublishedFileDetailsAsync(publishedFileId);
        }

        private async Task WorkAsync()
        {
            while (!_downloadQueue.IsEmpty)
            {
                _downloadQueue.TryDequeue(out var item);

                if (item == null) continue;

                if (item.CancellationToken.IsCancellationRequested) continue;

                try
                {
                    (SteamLoginDto credentials, SteamClient client, SteamContentClient contentClient)
                        = await GetSteamClientAsync(item.AppId, item.UseAnonymousUser);

                    item.UpdateState.State = UpdateState.Status.Processing;

                    // For workshop items, we will perform a folder synchronization, meaning remotely non existant files will be deleted locally
                    // to ensure a 1to1 copy of the mod.
                    if (item.PublishedFileId.HasValue)
                    {
                        var details = await contentClient.GetPublishedFileDetailsAsync(item.PublishedFileId.Value);
                        Manifest manifest = await contentClient.GetManifestAsync(item.AppId, item.AppId, details.hcontent_file);

                        foreach (var localFilePath in Directory.GetFiles(item.Directory, "*", SearchOption.AllDirectories))
                        {
                            var relativeLocalPath = Path.GetRelativePath(item.Directory, localFilePath);

                            if (manifest.Files.Count(x => x.FileName.ToLowerInvariant() == relativeLocalPath.ToLowerInvariant()) == 0)
                            {
                                File.Delete(localFilePath);
                            }
                        }
                    }

                    await DownloadAsync(item, client.GetSteamOs(), contentClient, item.CancellationToken);
                }
                catch (SteamClientFaultedException ex)
                {
                    item.UpdateState.FailureException = ex.InnerException ?? ex;
                }
                catch (Exception ex)
                {
                    item.UpdateState.FailureException = ex;
                }

                item.UpdateState.State = UpdateState.Status.Completed;
                item.UpdateState.ProcessedEvent.Set();
            }
        }

        private async Task<(SteamLoginDto Credentials, SteamClient Client, SteamContentClient ContentClient)>
            GetSteamClientAsync(AppId appId, bool useAnonymous)
        {
            using (var lockRef = await _clientsLock.LockAsync())
            {
                await RefreshCredentialsAsync();

                // Remove accounts from the rateLimited dictionary after their time has been passed
                foreach (var rateLimitedUser in _rateLimitedAccounts.Where(x => x.Value <= DateTimeOffset.UtcNow).ToList())
                {
                    _ = _rateLimitedAccounts.TryRemove(rateLimitedUser.Key, out _);
                }

                string usernameToUse = default;

                if (!useAnonymous)
                {
                    // Determine the Steam user we need to use to download this item
                    var usernameQuery = _steamCredentials
                        // Find credentials that support this appid
                        .Where(x => x.SteamLoginSupportedApps.Any(app => app.AppId == appId))
                        // Find credentials that have not been rate limited recently
                        .Where(x => !_rateLimitedAccounts.ContainsKey(x.Username))
                        // Find crednetials that have not been marked as broken
                        .Where(x => !_invalidLoginKeys.Contains(x.LoginKey));

                    usernameToUse = usernameQuery.FirstOrDefault()?.Username;
                }
                else
                {
                    usernameToUse = "anonymous";
                }

                // No compatible user found
                if (usernameToUse == default) throw new NoCompatibleSteamUserFoundException(appId);

                // Check if the user already has a ready-to-use client
                var hadEntry = _steamClients.TryGetValue(usernameToUse, out var steamInfo);

                if (!hadEntry || steamInfo.Client.IsFaulted)
                {
                    SteamLoginDto credentials = null;

                    if (usernameToUse == "anonymous")
                    {
                        credentials = new SteamLoginDto { Username = "anonymous" };
                    }
                    else
                    {
                        credentials = _steamCredentials.First(x => x.Username == usernameToUse);
                    }

                    // We don't have a usable client, make a new one
                    try
                    {
                        (var client, var contentClient) = await CreateSteamClientAsync(credentials, _cancellationTokenSource.Token);

                        _steamClients.AddOrUpdate(
                            credentials.Username,
                            username => (credentials, client, contentClient),
                            (username, existingEntry) => (credentials, client, contentClient));
                    }
                    catch (SteamClientFaultedException ex)
                    {
                        if (ex.InnerException is SteamLogonException logonException)
                        {
                            // If we have been rate limited, exclude this account from further login attempts for some time
                            if (logonException.Result == SteamKit2.EResult.RateLimitExceeded)
                            {
                                _rateLimitedAccounts.TryAdd(credentials.Username, DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30)));
                            }
                            // If we got a invalid password result, mark this login key as invalid,
                            // this will result in this account only be chosen again once the login key
                            // changes, meaning the system administrator generated a new login key with sentry data.
                            else if (logonException.Result == SteamKit2.EResult.InvalidPassword)
                            {
                                _invalidLoginKeys.Add(credentials.LoginKey);
                            }
                        }

                        throw;
                    }
                }

                _ = _steamClients.TryGetValue(usernameToUse, out steamInfo);

                return steamInfo;
            }
        }

        private async Task DownloadAsync(UpdateItem updateItem, SteamOs os, SteamContentClient contentClient, CancellationToken cancellationToken = default)
        {
            // Download Workshop items
            if (updateItem.PublishedFileId.HasValue)
            {
                var downloadHandler = await contentClient.GetPublishedFileDataAsync(updateItem.PublishedFileId.Value, default, default, default, os);

                var downloadTask = downloadHandler.DownloadToFolderAsync(updateItem.Directory, cancellationToken);

                while (downloadHandler.IsRunning || !downloadTask.IsCompleted)
                {
                    await Task.Delay(100, cancellationToken);

                    if (downloadTask.IsFaulted) throw downloadTask.Exception;

                    cancellationToken.ThrowIfCancellationRequested();

                    updateItem.UpdateState.Progress = downloadHandler.TotalProgress;
                }

                updateItem.UpdateState.Progress = 1;

                return;
            }

            // Download Apps
            List<Depot> depots = (await contentClient.GetDepotsOfBranchAsync(updateItem.AppId, updateItem.Branch)).ToList();

            if (updateItem.DepotCondition != default)
            {
                depots = depots.Where(depot => updateItem.DepotCondition.Invoke(depot)).ToList();
            }
            else
            {
                depots = depots
                    .Where(depot => depot.OperatingSystems.Any(x => x.Identifier == os.Identifier))
                    .ToList();
            }

            int completedItems = 0;

            foreach (var depot in depots)
            {
                var downloadHandler = await contentClient.GetAppDataAsync(updateItem.AppId, depot.Id, default, updateItem.Branch, updateItem.BranchPassword, os);

                var downloadTask = downloadHandler.DownloadToFolderAsync(updateItem.Directory, cancellationToken);

                while (!downloadTask.IsCompleted)
                {
                    await Task.Delay(100, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    updateItem.UpdateState.Progress = (double)(completedItems + downloadHandler.TotalProgress) / depots.Count;
                }

                completedItems += 1;
            }
        }

        private async Task<(SteamClient client, SteamContentClient contentClient)> CreateSteamClientAsync(
            SteamLoginDto steamCredentials, CancellationToken cancellationToken = default)
        {
            SteamClient client = default;

            if (steamCredentials.Username == "anonymous")
            {
                client = new SteamClient(
                    SteamCredentials.Anonymous,
                    new DefaultSteamAuthenticationCodesProvider(),
                    new DefaultSteamAuthenticationFilesProvider());
            }
            else
            {
                client = new SteamClient(
                    new SteamCredentials(steamCredentials.Username, steamCredentials.Password),
                    new DefaultSteamAuthenticationCodesProvider(),
                    new PersistedSteamAuthenticationFilesProvider(
                        steamCredentials.LoginKey,
                        steamCredentials.Sentry != null ? Convert.FromBase64String(steamCredentials.Sentry) : null));
            }

            var contentClient = new SteamContentClient(client);

            await client.ConnectAsync(cancellationToken);

            return (client, contentClient);
        }

        private class PersistedSteamAuthenticationFilesProvider : SteamAuthenticationFilesProvider
        {
            private readonly string _loginKey;
            private readonly byte[] _sentryFile;

            public PersistedSteamAuthenticationFilesProvider(string loginKey, byte[] sentryFile)
            {
                _loginKey = loginKey;
                _sentryFile = sentryFile;
            }

            public override string GetLoginKey(SteamCredentials steamCredentials)
            {
                return _loginKey;
            }

            public override byte[] GetSentryFileContent(SteamCredentials steamCredentials)
            {
                return _sentryFile;
            }

            public override void SaveLoginKey(SteamCredentials steamCredentials, string loginKey)
            {

            }

            public override void SaveSentryFileContent(SteamCredentials steamCredentials, byte[] data)
            {

            }
        }

        private class UpdateItem
        {
            public CancellationToken CancellationToken { get; set; }

            public Func<Depot, bool> DepotCondition { get; set; }

            public AppId AppId { get; set; }
            public PublishedFileId? PublishedFileId { get; set; }
            public string Directory { get; set; }
            public string Branch { get; set; }
            public string BranchPassword { get; set; }
            public bool UseAnonymousUser { get; set; }
            public UpdateState UpdateState { get; set; }
        }
    }
}
