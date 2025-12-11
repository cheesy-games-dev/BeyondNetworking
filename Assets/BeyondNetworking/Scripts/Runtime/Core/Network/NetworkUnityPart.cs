using Riptide;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Beyond.Networking
{
    public static partial class Network
    {
        public static string CurrentScene;
        public static Dictionary<KeyValuePair<int, string>, NetworkView> SpawnedFromInstantiate = new();
        public static Dictionary<int, NetworkView> Spawned = new();
        public static void AllocateSceneViews() {
            int sceneId = 0;
            foreach (var view in GameObject.FindObjectsByType<NetworkView>(FindObjectsSortMode.InstanceID)) {
                view.SceneId = sceneId;
                sceneId++;
                AllocateView(view);
            }
        }
        public static void Send(Message message, bool asServer = false) {
            if (!Server.IsRunning && !Client.IsConnected)
                return;
            if (asServer)
                Mono.Server.SendToAll(message);
            else
                Mono.Client.Send(message);
        }
        public static void Send(Message message, ushort exceptClient) {
            Mono.Server.SendToAll(message, exceptClient);
        }
        public static void SendTo(Message message, ushort toClient) {
            Mono.Server.Send(message, toClient);
        }
        internal static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            Spawned.Clear();
            SpawnedFromInstantiate.Clear();
            CurrentScene = scene.name;
            AllocateSceneViews();
        }
        public static int AllocateView(NetworkView view) {
            view.ViewId = Spawned.Count;
            Spawned.Add(view.ViewId, view);
            view.Allocated();
            return view.ViewId;
        }
        public static GameObject Instantiate(string key, Vector3 position = new(), Quaternion rotation = new()) {
            var id = SpawnedFromInstantiate.Count;
            Message spawnMessage = Message.Create(MessageSendMode.Reliable, Messages.ObjectSpawnMessage);
            spawnMessage.Add(key).Add(id).Add(position).Add(rotation);
            Send(spawnMessage, true);
            while (Spawned[id] == null);
            return Spawned[id].gameObject;
        }

        public static void Destroy(GameObject gameObject) {
            Destroy(gameObject.GetNetworkView());
        }
        public static void Destroy(NetworkView networkView) {
            if (!isHost) {
                Debug.LogWarning("Server not running, Cannot instantiate");
                return;
            }
            var message = Message.Create(MessageSendMode.Reliable, Messages.ObjectDestroyMessage);
            message.Add(networkView.ViewId);
            Send(message, true);
        }
        [MessageHandler((ushort)Messages.ObjectDestroyMessage)]
        internal static void ObjectDestroy_Handler(Message message) {
            Destroy(Spawned[message.GetInt()]);
        }
        [MessageHandler((ushort)Messages.ObjectSpawnMessage)]
        internal static void ObjectSpawn_Handler(Message message) {
            InstantiatePrefab(message.GetString(), message.GetInt(), message.GetVector3(), message.GetQuaternion());
        }

        internal static GameObject InstantiatePrefab(string key, int id, Vector3 position, Quaternion rotation) {
            var prefab = Prefabs[key];
            var gameObject = Object.Instantiate(prefab, position, rotation);
            gameObject.GetNetworkView().InstantiationId = id;
            SpawnedFromInstantiate.Add(new(id, key), gameObject.GetNetworkView());
            AllocateView(gameObject.GetNetworkView());
            return gameObject;
        }
    }
}
