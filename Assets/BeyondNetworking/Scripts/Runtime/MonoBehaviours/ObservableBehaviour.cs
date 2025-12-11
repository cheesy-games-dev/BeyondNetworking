using Riptide;

namespace Beyond.Networking
{
    public abstract class ObservableBehaviour : UnityEngine.MonoBehaviour {
        public NetworkView NetworkView {
            get; internal set;
        }
        public MessageSendMode sendMode;
        public bool clientAuthority = true;
        protected string _payloadKey => _payloadHeaderKey + _payloadEndKey;
        protected abstract string _payloadHeaderKey {
            get;
        }
        protected string _payloadEndKey => !clientAuthority && Network.isHost ? "SERVER" : "OWNER";
        public bool canWrite => (NetworkView.IsMine && clientAuthority) || (!clientAuthority && Network.isHost);
        public void RPC(string methodName, RpcTarget target = RpcTarget.All, bool buffered = false, MessageSendMode reliability = MessageSendMode.Unreliable, params object[] args) {
            NetworkView.RPC(this, methodName, target, buffered, reliability, args);
        }

        public void RPC(string methodName, ushort target, MessageSendMode reliability = MessageSendMode.Unreliable, params object[] args) {
            NetworkView.RPC(this, methodName, target, reliability, args);
        }
        /// <summary>
        /// call OnSerialize when you need it!
        /// please put base.OnSerialize(); after doing your message
        /// </summary>
        /// <param name="message">Create new Message when you call OnSerialize</param>
        /// <param name="overwrite">Allow the writer to send the new message</param>
        public virtual void OnSerialize(Message message, bool overwrite = true) {
            if (!canWrite)
                return;
            if (overwrite) {
                NetworkView.SendPayload(_payloadKey, message, sendMode);
            }
        }
        /// <summary>
        /// OnDeserialize is called when it's received
        /// Put base.OnDeserialize(); before you do your message
        /// </summary>
        /// <param name="message">Message received from payload</param>
        public virtual void OnDeserialize(Message message) {
            if (canWrite)
                return;
        }

        private void Awake() {
            NetworkView = this.GetNetworkView();
            NetworkView.PayloadReceived += (x, y) => {
                if (x == _payloadKey)
                    OnDeserialize(y);
            };
        }
    }
}
