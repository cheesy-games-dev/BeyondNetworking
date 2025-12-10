using Riptide;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Beyond.Networking
{
    public static partial class Network
    {
        public static Buffer Buffer = new();
        public static void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
            List<NetworkView> views = new();
            foreach (var gameObject in arg0.GetRootGameObjects()) {
                views.AddRange(gameObject.GetComponentsInChildren<NetworkView>());
            }
            int sceneId = 0;
            foreach (var view in views) {
                view.SceneId = sceneId;
                sceneId++;
                AllocateView(view);
            }
        }
        public static int AllocateView(NetworkView view) {
            view.ViewId = Buffer.Spawned.Count;
            Buffer.Spawned.Add(view.ViewId, view);
            return view.ViewId;
        }
        public static GameObject Instantiate(string key, Vector3 position = new(), Quaternion rotation = new()) {
            if (!Mono.Server.IsRunning) {
                Debug.LogWarning("Server not running, Cannot instantiate");
                return null;
            }
            var id = Buffer.SpawnedFromInstantiate.Count;
            Message spawnMessage = Message.Create(MessageSendMode.Reliable, Messages.ObjectSpawnMessage);
            spawnMessage.Add(key).Add(id).Add(position).Add(rotation);
            Mono.Server.SendToAll(spawnMessage);
            return InstantiatePrefab(key, id, position, rotation);
        }

        [MessageHandler((ushort)Messages.ObjectSpawnMessage)]
        internal static void ObjectSpawn_Handler(Message message) {
            if (Mono.Server.IsRunning)
                return;
            InstantiatePrefab(message.GetString(), message.GetInt(), message.GetVector3(), message.GetQuaternion());
        }

        internal static GameObject InstantiatePrefab(string key, int id, Vector3 position, Quaternion rotation) {
            var prefab = Prefabs[key];
            var gameObject = Object.Instantiate(prefab, position, rotation);
            gameObject.GetNetworkView().InstantiationId = id;
            Buffer.SpawnedFromInstantiate.Add(new(id, key), gameObject.GetNetworkView());
            AllocateView(gameObject.GetNetworkView());
            return gameObject;
        }
    }

  public class Buffer {
        public string CurrentScene;
        public Dictionary<KeyValuePair<int, string>, NetworkView> SpawnedFromInstantiate = new();
        public Dictionary<int, NetworkView> Spawned = new();

        public Buffer() {
            CurrentScene = SceneManager.GetActiveScene().name;
            SpawnedFromInstantiate = new();
            Spawned = new();
        }
  }
}
