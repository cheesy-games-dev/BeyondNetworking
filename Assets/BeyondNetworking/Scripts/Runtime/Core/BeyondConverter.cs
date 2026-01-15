using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Beyond.Networking
{
    public static class BeyondConverter
    {
        static BinaryFormatter binaryFormatter = new BinaryFormatter();
        public static byte[] ObjectToByteArray(object obj) {
            using (var ms = new MemoryStream()) {
                binaryFormatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static object ByteArrayToObject(byte[] arrBytes) {
            using (var memStream = new MemoryStream()) {
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binaryFormatter.Deserialize(memStream);
                return obj;
            }
        }
    }
}
