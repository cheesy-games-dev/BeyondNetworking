using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond.Networking
{
    public static partial class BeyondNetwork
    {
        public static Buffer buffer;
    }

  public struct Buffer {
    public string CurrentScene;
    public Dictionary<uint, NetworkView> Spawned;
  }
}
