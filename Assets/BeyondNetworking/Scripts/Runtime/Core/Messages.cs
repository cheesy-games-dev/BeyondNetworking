using Riptide;

namespace Beyond.Networking
{
    public enum Messages : ushort {
        ServerDataMessage,
        ObjectSpawnMessage,
        ObjectDestroyMessage,
        RpcMessage,
        ChangeOwnerMessage,
        EntityStateMessage,
        ChangeSceneMessage,
    }
    public struct EntityStateMessage : IMessageSerializable {
        public int ViewId;
        public byte[] Payload;

        public EntityStateMessage(NetworkView view, byte[] payload) {
            ViewId = view.ViewId;
            Payload = payload;
        }

        public void Deserialize(Message message) {
            throw new System.NotImplementedException();
        }

        public void Serialize(Message message) {
            message.Add(ViewId);
            message.Add(Payload);
        }
    }
    public struct RpcMessage : IMessageSerializable {
        public bool Targeted;
        public int ViewId;
        public int ComponentIndex;
        public string MethodName;
        public object[] Args;
        public uint Target;
        public bool Buffered;
        public void Deserialize(Message message) {
            Targeted = message.GetBool();
            ViewId = message.GetInt();
            ComponentIndex = message.GetInt();
            MethodName = message.GetString();
            Args = message.GetBytes().FromBytes<object[]>();
            Target = message.GetUShort();
            Buffered = message.GetBool();
        }

        public RpcMessage(bool targeted, int viewId, int componentIndex, string methodName, object[] args, uint target, bool buffered) {
            Targeted = targeted;
            ViewId = viewId;
            ComponentIndex = componentIndex;
            MethodName = methodName;
            Args = args;
            Target = target;
            Buffered = buffered;
        }

        public void Serialize(Message message) {
            message.Add(Targeted);
            message.Add(ViewId);
            message.Add(ComponentIndex);
            message.Add(MethodName);
            message.Add(Args.ToBytes());
            message.Add(Target);
            message.Add(Buffered);
        }
    }
}
