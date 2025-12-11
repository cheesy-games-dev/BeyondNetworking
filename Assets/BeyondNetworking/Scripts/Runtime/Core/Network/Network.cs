using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Beyond.Networking {
    public static partial class Network
    {
        public static NetworkMono Mono {
            get; internal set;
        }
        public static NetworkSettings Settings {
            get; internal set;
        }
        public static bool isHost => Server.IsRunning;
        public static bool isConnected => Client.IsConnected;
        public static Server Server => Mono.Server;
        public static Client Client => Mono.Client;
        public static Dictionary<string, GameObject> Prefabs = new();
        static Network() {
            StartNetwork();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void StartNetwork() {
            var settings = Resources.Load<NetworkSettings>(NetworkSettings.SETTINGSPATH);
            settings.Start();
            if (!Application.IsPlaying(settings))
                return;
            NickName = PlayerPrefs.GetString("NICKNAME", $"Player{Random.Range(1000, 9999)}");
            UserId = Application.buildGUID;
            Mono = new GameObject("Beyond Mono").AddComponent<NetworkMono>();
        }

        internal static void Server_ClientDisconnected(object sender, Riptide.ServerDisconnectedEventArgs e) {
            UpdateServerData();
        }
    }
}
