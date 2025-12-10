using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Beyond.Networking
{
    public static class NetworkExtensions
    {
        private static BinaryFormatter _binaryFormatter = new();
        private static MemoryStream _memoryStream = null;
        public static NetworkView GetNetworkView(this Component a) => a.GetComponentInParent<NetworkView>();
        public static NetworkView GetNetworkView(this GameObject a) => a.GetComponentInParent<NetworkView>();
        public static byte[] ToBytes(this object obj) {
            using (_memoryStream = new()) {
                _binaryFormatter.Serialize(_memoryStream, obj);
                return _memoryStream.ToArray();
            }
        }
        public static T FromBytes<T>(this byte[] arrBytes) {
            using (_memoryStream = new()) {
                var binForm = new BinaryFormatter();
                _memoryStream.Write(arrBytes, 0, arrBytes.Length);
                _memoryStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(_memoryStream);
                return (T)obj;
            }
        }
    }
}
