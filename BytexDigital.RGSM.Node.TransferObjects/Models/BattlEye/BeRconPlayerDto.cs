using System.Net;

namespace BytexDigital.RGSM.Node.TransferObjects.Models.BattlEye
{
    public class BeRconPlayerDto
    {
        public int Id { get; set; }
        public IPEndPoint RemoteEndpoint { get; set; }
        public int Ping { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public bool IsVerified { get; set; }
        public bool IsInLobby { get; set; }
    }
}
