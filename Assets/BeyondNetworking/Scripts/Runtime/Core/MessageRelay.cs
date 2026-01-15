using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond.Networking
{
    public static class MessageRelay
    {
        public static ServerMessageRelay ServerRelay {
            get; internal set;
        } = new();
        public static ClientMessageRelay ClientRelay {
            get; internal set;
        } = new();
    }
}
