using System;

using BytexDigital.Steam.Core.Structs;

namespace BytexDigital.RGSM.Node.Application.Exceptions
{
    public class NoCompatibleSteamUserFoundException : Exception
    {
        public NoCompatibleSteamUserFoundException(AppId appId) : base($"No Steam user found that can be used to download the app {appId}.")
        {

        }

        public NoCompatibleSteamUserFoundException(AppId appId, PublishedFileId publishedFileId) : base($"No Steam user found that can be used to download the workshop item {publishedFileId}.")
        {

        }
    }
}
