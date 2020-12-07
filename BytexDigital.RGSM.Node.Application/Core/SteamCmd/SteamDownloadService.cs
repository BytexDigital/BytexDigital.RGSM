using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Application.Options;
using BytexDigital.RGSM.Panel.Server.TransferObjects.Entities;
using BytexDigital.Steam.ContentDelivery;
using BytexDigital.Steam.Core;
using BytexDigital.Steam.Core.Enumerations;
using BytexDigital.Steam.Core.Structs;

using Microsoft.Extensions.Options;

namespace BytexDigital.RGSM.Node.Application.Core.SteamCmd
{
    public class SteamDownloadService : IDisposable
    {
        private readonly BlockingCollection<UpdateItem> _downloadQueueCollection;
        private readonly ConcurrentQueue<UpdateItem> _downloadQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly HttpClient _httpClient;
        private List<SteamCredentialDto> _steamCredentials;
        private ConcurrentDictionary<string, (SteamCredentialDto Credentials, SteamClient Client, SteamContentClient ContentClient)> _steamClients;
        private Task _workTask;

        public SteamDownloadService(IOptions<NodeOptions> nodeOptions, HttpClient httpClient)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _downloadQueue = new ConcurrentQueue<UpdateItem>();
            _downloadQueueCollection = new BlockingCollection<UpdateItem>(_downloadQueue);
            _steamClients = new ConcurrentDictionary<string, (SteamCredentialDto Credentials, SteamClient Client, SteamContentClient ContentClient)>();
            _httpClient = httpClient;
        }

        public async Task InitializeAsync()
        {
            _steamCredentials = await _httpClient.GetFromJsonAsync<List<SteamCredentialDto>>("API/Steam/Credentials");
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        public Task<UpdateState> DownloadAppIdAsync(AppId appId, string directory, string branch, string branchPassword, string useUsername = default)
        {
            var updateState = new UpdateState
            {
                CancellationToken = new System.Threading.CancellationTokenSource(),
                Progress = 0,
                State = UpdateState.Status.Queued
            };

            _downloadQueue.Enqueue(new UpdateItem
            {
                UpdateState = updateState,
                CancellationToken = updateState.CancellationToken.Token,

                AppId = appId,
                Directory = directory,
                Branch = branch,
                BranchPassword = branchPassword,
                UseSteamUsername = useUsername
            });

            if (_workTask == default || _workTask.IsCompleted)
            {
                _workTask = Task.Run(async () => await WorkAsync());
            }

            return Task.FromResult(updateState);
        }

        private async Task WorkAsync()
        {
            foreach (var item in _downloadQueueCollection.GetConsumingEnumerable())
            {
                try
                {
                    // Determine the Steam user we need to use to download this item
                    var usernameQuery = _steamClients.Where(x => x.Value.Credentials.SteamCredentialSupportedApps.Any(app => app.AppId == item.AppId));

                    if (item.PublishedFileId.HasValue)
                    {
                        usernameQuery = usernameQuery.Where(x => x.Value.Credentials.SteamCredentialSupportedApps.Any(app => app.SupportsWorkshop));
                    }

                    var usernameToUse = item.UseSteamUsername ?? usernameQuery.FirstOrDefault().Value.Credentials.Username;

                    // No compatible user found
                    if (usernameToUse == default) throw item.PublishedFileId.HasValue ?
                            new NoCompatibleSteamUserFoundException(item.AppId, item.PublishedFileId.Value) :
                            new NoCompatibleSteamUserFoundException(item.AppId);

                    // Check if the user already has a ready-to-use client
                    var hadEntry = _steamClients.TryGetValue(usernameToUse, out var steamInfo);

                    if (!hadEntry || steamInfo.Client.IsFaulted)
                    {
                        // We don't have a usable client, make a new one
                        (var client, var contentClient) = await CreateSteamClientAsync(steamInfo.Credentials, _cancellationTokenSource.Token);

                        _steamClients.AddOrUpdate(
                            steamInfo.Credentials.Username,
                            username => (steamInfo.Credentials, client, contentClient),
                            (username, existingEntry) => (steamInfo.Credentials, client, contentClient));
                    }

                    _ = _steamClients.TryGetValue(usernameToUse, out steamInfo);

                    item.UpdateState.State = UpdateState.Status.Processing;

                    await DownloadAsync(item, steamInfo.Client.GetSteamOs(), steamInfo.ContentClient, item.CancellationToken);
                }
                catch (Exception ex)
                {
                    item.UpdateState.FailureException = ex;
                }

                item.UpdateState.State = UpdateState.Status.Completed;
            }
        }

        private async Task DownloadAsync(UpdateItem updateItem, SteamOs os, SteamContentClient contentClient, CancellationToken cancellationToken = default)
        {
            // Download Workshop items
            if (updateItem.PublishedFileId.HasValue)
            {
                var downloadHandler = await contentClient.GetPublishedFileDataAsync(updateItem.PublishedFileId.Value, default, default, default, os);

                while (downloadHandler.IsRunning)
                {
                    await Task.Delay(100, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    updateItem.UpdateState.Progress = downloadHandler.TotalProgress;
                }

                updateItem.UpdateState.Progress = 1;

                return;
            }

            // Download Apps
            var depots = (await contentClient.GetDepotsOfBranchAsync(updateItem.AppId, updateItem.Branch))
                .Where(depot => depot.OperatingSystems.Any(x => x.Identifier == os.Identifier))
                .ToList();

            int completedItems = 0;

            foreach (var depot in depots)
            {
                var downloadHandler = await contentClient.GetAppDataAsync(updateItem.AppId, depot.Id, default, updateItem.Branch, updateItem.BranchPassword, os);

                while (downloadHandler.IsRunning)
                {
                    await Task.Delay(100, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    updateItem.UpdateState.Progress = ((double)completedItems + downloadHandler.TotalProgress) / depots.Count;
                }

                completedItems += 1;
            }
        }

        private async Task<(SteamClient client, SteamContentClient contentClient)> CreateSteamClientAsync(
            SteamCredentialDto steamCredentials, CancellationToken cancellationToken = default)
        {
            var client = new SteamClient(
                new SteamCredentials(steamCredentials.Username, steamCredentials.Password),
                new DefaultSteamAuthenticationCodesProvider(),
                new PersistedSteamAuthenticationFilesProvider(steamCredentials.LoginKey, Convert.FromBase64String(steamCredentials.Sentry)));

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

            public AppId AppId { get; set; }
            public PublishedFileId? PublishedFileId { get; set; }
            public string Directory { get; set; }
            public string Branch { get; set; }
            public string BranchPassword { get; set; }
            public string UseSteamUsername { get; set; }
            public UpdateState UpdateState { get; set; }
        }
    }
}
