using System.Collections.Generic;

using BytexDigital.RGSM.Shared.Enumerations;

namespace BytexDigital.RGSM.Shared
{
    public static class PermissionConstants
    {
        public static Dictionary<ServerType, Dictionary<string, string>> Permissions { get; } = new Dictionary<ServerType, Dictionary<string, string>>
        {
            {
                ServerType.Arma3,
                new Dictionary<string, string>
                {
                    { STARTSTOP, "Start and stop the server." },
                    { FILEBROWSER_READ, "Read files from disk and download them." },
                    { FILEBROWSER_WRITE, "Write files to disk." },
                    { UPDATE, "Update the server." },
                    { WORKSHOP, "Manage workshop mods." },
                    { SCHEDULER, "Manage scheduler." },
                    { BE_RCON_READ, "Read RCON messages." },
                    { BE_RCON_SEND, "Send RCON messages." }
                }
            }
        };

        public const string STARTSTOP = "generic.startstop";
        public const string UPDATE = "generic.update";

        public const string WORKSHOP = "generic.workshop";

        public const string SCHEDULER = "generic.scheduler";

        public const string FILEBROWSER_READ = "generic.filebrowser.read";
        public const string FILEBROWSER_WRITE = "generic.filebrowser.write";

        public const string BE_RCON_READ = "generic.bercon.read";
        public const string BE_RCON_SEND = "generic.bercon.send";
    }
}
