using Riptide;
using Riptide.Transports;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond.Networking
{
    public static partial class Network
    {
        public static ServerData CurrentServer;
        public static void InitializeServer(ushort connections, ushort listenPort = 25000, string name = "SERVER", string[] properties = null) {
            if (name == "SERVER")
                name += Guid.NewGuid().GetHashCode().ToString();
            Mono.Server.Start(listenPort, connections);
            Connect("127.0.0.1", listenPort);
            CurrentServer = new ServerData(name, connections, properties);
        }
        public static void Connect(string address, ushort remotePort = 25000, int connectionAttempts = 5) {
            Message connectMessage = Message.Create();
            connectMessage.AddString(NickName);
            connectMessage.AddString(UserId);
            if (!Mono.Client.Connect(address, connectionAttempts, 0, connectMessage))
                Mono.Client.Connect(address + $":{remotePort}", connectionAttempts, 0, connectMessage);
        }
        public static string NickName;
        public static string UserId;

        [MessageHandler((ushort)MessageHeader.Connect)]
        internal static void ConnectedHandler(ushort fromClientId, Message message) {
            UpdateServerData();
        }

        public static void UpdateServerData() {
            var message = Message.Create(MessageSendMode.Reliable, Messages.ServerDataMessage);
            message.AddSerializable(CurrentServer);
            Mono.Server.SendToAll(message);
        }

        [MessageHandler((ushort)Messages.ServerDataMessage)]
        internal static void ServerDataUpdatedHandler(Message message) {
            if (isHost)
                return;
            CurrentServer = message.GetSerializable<ServerData>();
        }
    }

    [Serializable]
    public struct ServerData : IMessageSerializable {
        public string Name;
        public uint MaxClients;
        public string[] CustomProperties;

        public ServerData(string name, uint maxClients, string[] properties) {
            Name = name;
            MaxClients = maxClients;
            CustomProperties = properties;
        }

        public void Serialize(Message message) {
            message.Add(Name);
            message.Add(MaxClients);
            message.Add(CustomProperties);
        }

        public void Deserialize(Message message) {
            Name = message.GetString();
            MaxClients = message.GetUInt();
            CustomProperties = message.GetStrings();
        }
    }
}
