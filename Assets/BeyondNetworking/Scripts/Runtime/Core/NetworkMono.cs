using Riptide;
using Riptide.Utils;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Beyond.Networking
{
    [DefaultExecutionOrder(-ushort.MaxValue)]
    public class NetworkMono : ObservableBehaviour
    {
        public Server Server;
        public Client Client;

        private void Awake() {
            DontDestroyOnLoad(this);
            Server = new();
            Client = new();
            SceneManager.sceneLoaded += Network.OnSceneLoaded;
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            Network.AllocateSceneViews();
        }

        private void LateUpdate() {
            Server?.Update();
            Client?.Update();
        }

        private void OnApplicationQuit() {
            Server.Stop();
            Client.Disconnect();
            Server = null;
            Client = null;
        }
    }
}
