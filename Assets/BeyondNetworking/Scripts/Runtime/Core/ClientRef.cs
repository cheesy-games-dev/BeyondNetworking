using Riptide;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond.Networking
{
    [Serializable]
    public struct ClientRef : IMessageSerializable {
        public Connection GetConnection() {
            if (!Network.Mono.Server.IsRunning) {
                Debug.LogWarning("Can't get Connection, Server not running");
                return null;
            }
            return Network.Mono.Server.Clients[ActorNumber];
        }

        public void Serialize(Message message) {
            message.Add(Nickname);
            message.Add(ActorNumber);
            message.Add(IsHost);
            message.Add(CustomProperties);
            if(NetworkSettings.Settings.PublishUserIds) message.Add(UserId);
        }

        public void Deserialize(Message message) {
            Nickname = message.GetString();
            ActorNumber = message.GetUInt();
            IsHost = message.GetBool();
            CustomProperties = message.GetStrings();
            if (NetworkSettings.Settings.PublishUserIds)
                UserId = message.GetString();
        }

        public string Nickname;
        public uint ActorNumber;
        public bool IsHost;
        public string[] CustomProperties;

        public string UserId;
    }
}
