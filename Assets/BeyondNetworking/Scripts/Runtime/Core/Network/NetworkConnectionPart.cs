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
        public static ClientRef LocalClient;
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
            ClientRef newClientRef = new();
            newClientRef.Nickname = message.GetString();
            newClientRef.UserId = message.GetString();
            newClientRef.ActorNumber = fromClientId;
            newClientRef.IsHost = newClientRef.GetConnection() == Mono.Client.Connection;
            CurrentServer.Clients.Add(newClientRef);
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
        public List<ClientRef> Clients;
        public string[] CustomProperties;

        public ServerData(string name, uint maxClients, string[] properties) {
            Name = name;
            MaxClients = maxClients;
            CustomProperties = properties;
            Clients = new List<ClientRef>();
        }

        public void Serialize(Message message) {
            message.AddString(Name);
            message.AddUInt(MaxClients);
            message.AddStrings(CustomProperties);
            message.AddString(JsonUtility.ToJson(CustomProperties));
        }

        public void Deserialize(Message message) {
            Name = message.GetString();
            MaxClients = message.GetUInt();
            CustomProperties = message.GetStrings();
            Clients = JsonUtility.FromJson<List<ClientRef>>(message.GetString());
        }
    }
}
