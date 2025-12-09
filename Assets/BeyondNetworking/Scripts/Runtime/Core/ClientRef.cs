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
            if (!BeyondNetwork.Mono.Server.IsRunning) {
                Debug.LogWarning("Can't get Connection, Server not running");
                return null;
            }
            return BeyondNetwork.Mono.Server.Clients[ActorNumber];
        }

        public void Serialize(Message message) {
            message.Add(Nickname);
            message.Add(ActorNumber);
            message.Add(IsHost);
            message.Add(CustomProperties);
            if(BeyondSettings.Settings.PublishUserIds) message.Add(UserId);
        }

        public void Deserialize(Message message) {
            Nickname = message.GetString();
            ActorNumber = message.GetUInt();
            IsHost = message.GetBool();
            CustomProperties = message.GetStrings();
            if (BeyondSettings.Settings.PublishUserIds)
                UserId = message.GetString();
        }

        public string Nickname {
            get; set;
        }
        public uint ActorNumber {
            get; internal set;
        }
        public string UserId {
            get; set;
        }
        public bool IsHost {
            get; internal set;
        }
        public string[] CustomProperties {
            get; set;
        }
    }
}
