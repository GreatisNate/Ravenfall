﻿using Assets.Scripts.PacketHandlers;
using Shinobytes.Ravenfall.RavenNet;
using Shinobytes.Ravenfall.RavenNet.Modules;
using Shinobytes.Ravenfall.RavenNet.Packets;
using Shinobytes.Ravenfall.RavenNet.Packets.Client;
using System.Linq;
using System.Net;
using System.Reflection;

using ILogger = Shinobytes.Ravenfall.RavenNet.Core.ILogger;

public class Client : IRavenClient
{
    private readonly ILogger logger;
    private readonly RavenNetworkClient client;
    private readonly Authentication auth;
    private readonly PlayerHandler playerHandler;
    private readonly ObjectHandler objectHandler;
    private readonly NpcHandler npcHandler;

    private readonly object connectionMutex = new object();

    public IModuleManager Modules { get; }

    public bool IsConnected => client.IsConnected;
    public bool IsConnecting => client.IsConnecting;

    public Client(ILogger logger, IModuleManager moduleManager, INetworkPacketController controller)
    {
        this.logger = logger;
        this.Modules = moduleManager;

        this.client = new RavenNetworkClient(logger, RegisterPacketHandlers(controller));
        this.auth = this.Modules.AddModule(new Authentication(this.client));
        this.playerHandler = this.Modules.AddModule(new PlayerHandler());
        this.objectHandler = this.Modules.AddModule(new ObjectHandler());
        this.npcHandler = this.Modules.AddModule(new NpcHandler());
    }
    public void Send<T>(short packetId, T packet, SendOption sendOption)
    {
        client.Send(packetId, packet, sendOption);
    }

    public void Send<T>(T packet, SendOption sendOption)
    {
        client.Send(packet, sendOption);
    }

    public void ConnectAsync(IPAddress address, int port)
    {
        lock (connectionMutex)
        {
            client.ConnectAsync(address, port);
        }
    }

    public bool Connect(IPAddress address, int port)
    {
        lock (connectionMutex)
        {
            if (client.Connect(address, port))
            {
                logger.Debug("Connected to FS");
                return true;
            }

            logger.Error("Unable to connect to server.");
            return false;
        }
    }

    public void Dispose()
    {
        Modules.Dispose();
        client.Dispose();
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // private static INetworkPacketController GetPacketController(INetworkPacketController controller)
    // {
    //     return controller
    //         .Register<AuthResponse, AuthResponseHandler>()
    //         .Register<PlayerAdd, PlayerAddHandler>()
    //         .Register<PlayerMoveResponse, PlayerMoveResponseHandler>()
    //         .Register<PlayerRemove, PlayerRemoveHandler>()
    //         .Register<PlayerObjectActionResponse, PlayerObjectActionResponseHandler>();
    // }

    private static INetworkPacketController RegisterPacketHandlers(INetworkPacketController controller)
    {
        var packetHandlers = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => !x.IsAbstract && typeof(INetworkPacketHandler).IsAssignableFrom(x))
            .ToArray();

        foreach (var handler in packetHandlers)
        {
            var declaringType = handler.GetInterfaces().OrderByDescending(x => x.FullName).FirstOrDefault();
            var packetType = declaringType.GetGenericArguments().FirstOrDefault();
            var packetId = (short)packetType.GetField("OpCode").GetValue(null);
            controller.Register(packetType, handler, packetId);
        }

        return controller;
    }
}
