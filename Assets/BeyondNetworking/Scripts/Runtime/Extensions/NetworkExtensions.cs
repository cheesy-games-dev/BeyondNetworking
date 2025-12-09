using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond.Networking
{
    public static class NetworkExtensions
    {
        public static NetworkView GetNetworkView(this Component a) => a.GetComponentInParent<NetworkView>();
        public static NetworkView GetNetworkView(this GameObject a) => a.GetComponentInParent<NetworkView>();
    }
}
