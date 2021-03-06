﻿using Shinobytes.Ravenfall.RavenNet;
using Shinobytes.Ravenfall.RavenNet.Core;
using Shinobytes.Ravenfall.RavenNet.Modules;
using Shinobytes.Ravenfall.RavenNet.Packets;
using Shinobytes.Ravenfall.RavenNet.Packets.Client;

namespace Assets.Scripts.PacketHandlers
{
    public class PlayerStatUpdateHandler : INetworkPacketHandler<PlayerStatUpdate>
    {
        private readonly ILogger logger;
        private readonly IModuleManager moduleManager;

        public PlayerStatUpdateHandler(ILogger logger, IModuleManager moduleManager)
        {
            this.logger = logger;
            this.moduleManager = moduleManager;
        }

        public void Handle(PlayerStatUpdate data, IRavenNetworkConnection connection, SendOption sendOption)
        {
            var playerHandler = moduleManager.GetModule<PlayerHandler>();
            playerHandler.PlayerStatUpdate(data.PlayerId, data.Skill, data.Level, data.Experience);
        }
    }
}
