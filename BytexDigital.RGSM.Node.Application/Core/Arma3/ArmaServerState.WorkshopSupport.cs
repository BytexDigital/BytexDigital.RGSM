using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Features.Workshop;
using BytexDigital.RGSM.Node.Application.Core.Steam;
using BytexDigital.RGSM.Node.Application.Core.Steam.Commands;
using BytexDigital.RGSM.Node.Domain.Models.Workshop;
using BytexDigital.Steam.Core.Structs;

namespace BytexDigital.RGSM.Node.Application.Core.Arma3
{
    public partial class ArmaServerState : IWorkshopSupport
    {
        public async Task<List<WorkshopModState>> GetWorkshopModStatesAsync(CancellationToken cancellationToken = default)
        {
            var mods = Settings.WorkshopMods;
            var states = new List<WorkshopModState>();

            foreach (var mod in mods)
            {
                states.Add(new WorkshopModState
                {
                    IsInstalled = System.IO.Directory.Exists(mod.Directory),
                    RequiresUpdate = false, // TODO: Make dynamic

                    IsUpdating = WorkshopUpdateStates.Any(x =>
                        x.Key == mod.Id && (x.Value.State == UpdateState.Status.Queued || x.Value.State == UpdateState.Status.Processing)),
                    UpdateProgress = WorkshopUpdateStates.GetValueOrDefault(mod.Id)?.Progress ?? 0,
                    UpdateFailureReason = WorkshopUpdateStates.GetValueOrDefault(mod.Id)?.FailureException?.Message
                });
            }

            return await Task.FromResult(states);
        }

        public async Task<CanResult> CanUpdateWorkshopModsAsync(CancellationToken cancellationToken = default)
        {
            if (await IsRunningAsync(cancellationToken)) return CanResult.CannotBecause("Server is running.");
            if (IsUpdating) return CanResult.CannotBecause("Server is updating.");
            if (IsUpdatingWorkshopMods) return CanResult.CannotBecause("Server is updating workshop mods.");

            return CanResult.Can();
        }

        public async Task BeginUpdatingWorkshopModsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _isUpdatingMods = true;

                List<Task> modTasks = new List<Task>();

                foreach (var mod in Settings.WorkshopMods)
                {
                    var response = await Mediator.Send(new UpdatePublishedFileCmd
                    {
                        AppId = /*440,*/ mod.OfAppId ?? 107410,
                        PublishedFileId = mod.Id,
                        Directory = mod.Directory
                    });

                    WorkshopUpdateStates.AddOrUpdate(mod.Id, response.UpdateState, (key, value) => response.UpdateState);

                    var modTask = Task.Run(async () =>
                    {
                        // Wait for the mod to finish downloading
                        await response.UpdateState.ProcessedEvent.WaitAsync(cancellationToken);

                        if (response.UpdateState.CancellationToken.IsCancellationRequested) return;

                        // Do not continue if an exception occurred
                        if (response.UpdateState.FailureException != default) return;

                        // Activate the keys from this downloaded mod and deactivate old keys
                        // -- MOVED TO WHEN THE SERVER STARTS
                        //await ModKeyManager.DeactivateKeysAsync(mod.Id, response.UpdateState.CancellationToken.Token);
                        //await ModKeyManager.ActivateKeysAsync(mod.Id, mod.Directory, response.UpdateState.CancellationToken.Token);
                    });

                    modTasks.Add(modTask);
                }

                _ = Task.Run(async () =>
                {
                    await Task.WhenAll(modTasks);

                    _isUpdatingMods = false;
                });

                Logger.Information($"Beginning update of {modTasks.Count} workshop mods.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Beginning update of workshop mods failed.");
                throw;
            }
        }

        public Task CancelUpdatingWorkshopModsAsync(CancellationToken cancellationToken = default)
        {
            if (WorkshopUpdateStates == null) return Task.CompletedTask;

            foreach (var updateState in WorkshopUpdateStates.ToList())
            {
                updateState.Value.CancellationToken.Cancel();
            }

            Logger.Information($"Cancelled update of workshop mods.");

            return Task.CompletedTask;
        }

        public async Task AddOrUpdateWorkshopModAsync(PublishedFileId id, Dictionary<string, string> metadata, CancellationToken cancellationToken = default)
        {
            if (Settings.WorkshopMods.Any(x => x.Id == id))
            {
                if (metadata != null) Settings.WorkshopMods.First(x => x.Id == id).Metadata = metadata;
            }
            else
            {
                Settings.WorkshopMods.Add(new WorkshopMod
                {
                    Directory = Path.Combine(BaseDirectory, ".rgsm", "mods", id.ToString()),
                    Metadata = metadata ?? new Dictionary<string, string>()
                });
            }

            await Task.CompletedTask;
        }

        public async Task RemoveWorkshopModAsync(PublishedFileId id, CancellationToken cancellationToken = default)
        {
            if (!Settings.WorkshopMods.Any(x => x.Id == id)) return;

            Settings.WorkshopMods.Remove(Settings.WorkshopMods.First(x => x.Id == id));

            await Task.CompletedTask;
        }

        public async Task EnableOrDisableWorkshopModAsync(PublishedFileId id, bool enabled, CancellationToken cancellationToken = default)
        {
            if (!Settings.WorkshopMods.Any(x => x.Id == id)) return;

            Settings.WorkshopMods.First(x => x.Id == id).Enabled = enabled;

            await Task.CompletedTask;
        }
    }
}
