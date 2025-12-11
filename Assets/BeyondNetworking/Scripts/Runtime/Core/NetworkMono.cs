using Riptide;
using Riptide.Utils;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Beyond.Networking
{
    [DefaultExecutionOrder(-ushort.MaxValue)]
    public class NetworkMono : MonoBehaviour
    {
        public Server Server;
        public Client Client;

        private void Awake() {
            DontDestroyOnLoad(this);
            Server = new();
            Client = new();
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        }

        private void Start() {
            SceneManager.sceneLoaded += Network.OnSceneLoaded;
            Server.ClientDisconnected += Network.Server_ClientDisconnected;
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
