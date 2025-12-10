using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Beyond.Networking {
    public static partial class Network
    {
        public static NetworkMono Mono;
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
            Mono.Server.ClientDisconnected += Server_ClientDisconnected;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void Server_ClientDisconnected(object sender, Riptide.ServerDisconnectedEventArgs e) {
            CurrentServer.Clients.Remove(CurrentServer.Clients.Find(x => x.GetConnection() == e.Client));
            UpdateServerData();
        }
    }
}
