using Riptide;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Beyond.Networking
{
    public static partial class Network
    {
        public static Buffer Buffer;

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
                Debug.LogWarning("Server not Running, Can't instantiate");
                return null;
            }
            var prefab = Prefabs[key];
            var id = Buffer.SpawnedFromInstantiate.Count;
            Message spawnMessage = Message.Create(MessageSendMode.Reliable, Messages.ObjectSpawnMessage);
            spawnMessage.Add(id).Add(key).Add(position).Add(rotation);
            Mono.Server.SendToAll(spawnMessage);
            return InstantiatePrefab(id, prefab, position, rotation);
        }

        [MessageHandler((ushort)Messages.ObjectSpawnMessage)]
        internal static void ObjectSpawn_Handler(Message message) {
            if (Mono.Server.IsRunning)
                return;
            var id = message.GetInt();
            var prefab = Prefabs[message.GetString()];
            InstantiatePrefab(id, prefab, message.GetVector3(), message.GetQuaternion());
        }

        internal static GameObject InstantiatePrefab(int id, GameObject prefab, Vector3 position, Quaternion rotation) {
            var gameObject = Object.Instantiate(prefab, position, rotation);
            gameObject.GetNetworkView().InstantiationId = id;
            Buffer.SpawnedFromInstantiate.Add(id, gameObject.GetNetworkView());
            AllocateView(gameObject.GetNetworkView());
            return gameObject;
        }
    }

  public struct Buffer {
        public string CurrentScene;
        public Dictionary<int, NetworkView> SpawnedFromInstantiate;
        public Dictionary<int, NetworkView> Spawned;
  }
}
