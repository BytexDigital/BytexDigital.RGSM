using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.BattlEye.Rcon;
using BytexDigital.BattlEye.Rcon.Commands;
using BytexDigital.RGSM.Node.Domain.Models.BattlEye;

using Serilog.Core;

namespace BytexDigital.RGSM.Node.Application.Core.Features.BattlEye
{
    public class BeRconMonitor
    {
        private RconClient _rconClient;
        private List<BeRconMessage> _messages;
        private readonly Logger _logger;

        public BeRconMonitor(Logger logger)
        {
            _messages = new List<BeRconMessage>();
            _logger = logger;
        }

        public Task ConfigureAsync(string host, int port, string password)
        {
            if (_rconClient != null)
            {
                try { _rconClient.Disconnect(); } catch { }
            }

            if (host == "localhost" || host == "0.0.0.0")
            {
                host = "127.0.0.1";
            }

            var ipHost = IPAddress.Parse(host);
            var endpoint = new IPEndPoint(ipHost, port);

            _rconClient = new RconClient(endpoint, password);

            _rconClient.MessageReceived += RconClient_MessageReceived;
            _rconClient.Connected += RconClient_Connected;
            _rconClient.PlayerDisconnected += RconClient_PlayerDisconnected;

            bool initialConnectionAttemptResult = _rconClient.Connect();

            _logger.Information($"Created rcon client to ip {host}:{port} = {initialConnectionAttemptResult}");

            return Task.CompletedTask;
        }

        private void RconClient_PlayerDisconnected(object sender, BytexDigital.BattlEye.Rcon.Events.PlayerDisconnectedArgs e)
        {
            _logger.Information("Disconnected from rcon server.");
        }

        private void RconClient_Connected(object sender, EventArgs e)
        {
            _logger.Information("Connected to rcon server.");
        }

        public async Task<List<BeRconPlayer>> GetPlayersAsync(CancellationToken cancellationToken = default)
        {
            if (!_rconClient.IsConnected) return new List<BeRconPlayer>();

            (bool success, var playerList) = await _rconClient.Fetch<List<BytexDigital.BattlEye.Rcon.Domain.Player>>(new GetPlayersRequest(), cancellationToken);

            return playerList?.Select(x => new BeRconPlayer
            {
                Id = x.Id,
                Guid = x.Guid,
                IsInLobby = x.IsInLobby,
                IsVerified = x.IsVerified,
                Name = x.Name,
                Ping = x.Ping,
                RemoteEndpoint = x.RemoteEndpoint.ToString()
            }).ToList() ?? new List<BeRconPlayer>();
        }

        public Task<List<BeRconMessage>> GetMessagesAsync(int limit = 0, CancellationToken cancellationToken = default)
        {
            if (limit > 0)
            {
                return Task.FromResult(_messages.TakeLast(limit).ToList());
            }
            else
            {
                return Task.FromResult(_messages.ToList());
            }
        }

        public Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            _rconClient?.Send(new SendMessageCommand(message));

            return Task.CompletedTask;
        }

        private void RconClient_MessageReceived(object sender, string message)
        {
            _messages.Add(new BeRconMessage
            {
                TimeSent = DateTimeOffset.UtcNow,
                Text = message
            });
        }

        public bool IsConnected() => _rconClient != null && _rconClient.IsConnected;
    }
}
