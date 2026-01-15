namespace Beyond.Networking
{
    public enum MessageIds : ushort {
        CustomMessage = 0,
    }
    public class RpcMessage : CustomMesssage {
        public bool Targeted;
        public int ViewId;
        public int ComponentIndex;
        public string MethodName;
        public object[] Args;
        public uint Target;
        public bool Buffered;
        public RpcMessage(bool targeted, int viewId, int componentIndex, string methodName, object[] args, uint target, bool buffered) {
            Targeted = targeted;
            ViewId = viewId;
            ComponentIndex = componentIndex;
            MethodName = methodName;
            Args = args;
            Target = target;
            Buffered = buffered;
        }
    }
}
