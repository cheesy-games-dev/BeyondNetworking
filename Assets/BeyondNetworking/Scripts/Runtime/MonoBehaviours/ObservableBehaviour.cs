using Riptide;

namespace Beyond.Networking
{
    public abstract class ObservableBehaviour : UnityEngine.MonoBehaviour {
        public NetworkView NetworkView {
            get; internal set;
        }
        public void RPC(string methodName, RpcTarget target = RpcTarget.All, bool buffered = false, MessageSendMode reliability = MessageSendMode.Unreliable, params object[] args) {
            NetworkView.RPC(this, methodName, target, buffered, reliability, args);
        }

        public void RPC(string methodName, ClientRef target, MessageSendMode reliability = MessageSendMode.Unreliable, params object[] args) {
            NetworkView.RPC(this, methodName, target, reliability, args);
        }

        private void Awake() {
            NetworkView = this.GetNetworkView();
        }
    }
}
