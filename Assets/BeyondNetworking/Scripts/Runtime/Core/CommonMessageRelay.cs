using Riptide;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond.Networking
{
    public delegate void CustomMessageHandler<T>(T message, short sender) where T : CustomMesssage;

    public abstract class CustomMesssage {
    }
    public class CustomMessageListener {
        public System.Type MessageType {
            get; private set;
        }
        public object Handler {
            get; private set;
        }

        public CustomMessageListener(System.Type messageType, object handler) {
            MessageType = messageType;
            Handler = handler;
        }
    }
    public abstract class CommonMessageRelay
    {
        private List<CustomMessageListener> listeners = new List<CustomMessageListener>();
        public abstract void SendTo<T>(T message, ushort target, MessageSendMode sendMode = MessageSendMode.Unreliable) where T : CustomMesssage;

        public void CreateListener<T>(CustomMessageHandler<T> handler) where T : CustomMesssage {
            listeners.Add(new CustomMessageListener(typeof(T), handler));
        }
        protected void ReceiveMessage<T>(T message, short sender) where T : CustomMesssage {
            foreach (var listener in listeners) {
                if (listener.MessageType == message.GetType()) {
                    ((CustomMessageHandler<T>)listener.Handler).Invoke(message, sender);
                }
            }
        }
        public bool RemoveListener<T>(CustomMessageHandler<T> handler) where T : CustomMesssage {
            for (int i = 0; i < listeners.Count; i++) {
                CustomMessageListener listenerEntry = listeners[i];
                if (listenerEntry.MessageType == typeof(T) && (CustomMessageHandler<T>)listenerEntry.Handler == handler) {
                    listeners.Remove(listenerEntry);
                    return true;
                }
            }
            return false;
        }
    }
}
