using Riptide;
using Riptide.Utils;
using UnityEngine;

namespace Beyond.Networking
{
    public class NetworkMono : MonoBehaviour
    {
        public Server Server;
        public Client Client;
        private void Start() {
            DontDestroyOnLoad(this);
            Server = new();
            Client = new();
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        }

        private void LateUpdate() {
            Server?.Update();
            Client?.Update();
        }
    }
}
