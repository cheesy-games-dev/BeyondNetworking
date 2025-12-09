using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Beyond.Networking
{
    [CreateAssetMenu(fileName = SETTINGSPATH, menuName ="Beyond/Networking/Settings")]
    public class NetworkSettings : ScriptableObject
    {
        public static NetworkSettings Settings;
        [Header("Default Settings")]
        public int MaxMessagePayloadSize = ushort.MaxValue;
        public bool PublishUserIds = false;

        [Header("Data")]
        [SerializeField] private PrefabReference[] _prefabs;

        private void Awake() {
            Start();
        }

        public const string SETTINGSPATH = "Network Settings";

        public void Start() {
            Settings = this;
            this.name = SETTINGSPATH;
            Message.MaxPayloadSize = MaxMessagePayloadSize;
            Network.Prefabs.Clear();
            foreach (var prefab in _prefabs) {
                Network.Prefabs.Add(prefab.Key, prefab.Prefab);
            }
        }

        private void OnValidate() {
            Start();
        }
    }

    [Serializable]
    public struct PrefabReference {
        public string Key;
        public GameObject Prefab;
    }
}
