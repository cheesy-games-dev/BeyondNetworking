using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond.Networking
{
    public enum Messages : ushort {
        ServerDataMessage,
        ObjectSpawnMessage,
        ObjectDestroyMessage,
        RpcMessage,
        SyncDataMessage,
        ChangeSceneMessage,
    }
}
