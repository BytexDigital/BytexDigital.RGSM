using System.Net;

namespace BytexDigital.RGSM.Node.Domain.Models.BattlEye
{
    public class BeRconPlayer
    {
        public int Id { get; set; }
        public string RemoteEndpoint { get; set; }
        public int Ping { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public bool IsVerified { get; set; }
        public bool IsInLobby { get; set; }
    }
}
