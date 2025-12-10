using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;
using System.Linq;
using System;

namespace Beyond.Networking
{
    public class NetworkView : MonoBehaviour
    {
        public bool SharedOwnership = false;
        public bool AutoFindObservables = true;
        public int SceneId = 0;
        public int InstantiationId { get; internal set; } = 0;
        public int ViewId { get; internal set; } = 0;
        public ClientRef Owner { get; internal set; } = new();
        public bool IsMine => Owner.GetConnection() == Network.Mono.Client.Connection;
        public List<IObservable> Observables = new();
        
        public void RPC(IObservable component, string methodName, RpcTarget target, bool buffered = false, MessageSendMode sendMode = MessageSendMode.Unreliable, params object[] args){
            var componentIndex = Observables.ToList().IndexOf(component);
            Message rpcMessage = Message.Create(sendMode, Messages.RpcMessage);
            rpcMessage.AddSerializable<RpcMessage>(new(false, ViewId, Observables.IndexOf(component), methodName, args, (uint)target, buffered));
            Network.Mono.Client.Send(rpcMessage);
        }

        public void RPC(IObservable component, string methodName, ClientRef target, MessageSendMode sendMode = MessageSendMode.Unreliable, params object[] args){
            var componentIndex = Observables.ToList().IndexOf(component);
            Message rpcMessage = Message.Create(sendMode, Messages.RpcMessage);
            rpcMessage.AddSerializable<RpcMessage>(new(true, ViewId, Observables.IndexOf(component), methodName, args, target.ActorNumber, false));
            Network.Mono.Client.Send(rpcMessage);
        }

        [MessageHandler((ushort)Messages.RpcMessage)]
        public static void RPC_MessageHandlerSERVER(ushort fromClientId, Message message) {
            Debug.Log($"Got RPC from {fromClientId}");
            var rpc = message.GetSerializable<RpcMessage>();
            if (rpc.Targeted) {

            }
            else {
                if (rpc.Target == (uint)RpcTarget.Server) {
                    message.GetBool();
                    HandleRPC(message.GetInt(), message.GetInt(), message.GetString(), message.GetBytes());
                    return;
                }
                else {
                    Message clientRpcMessage = Message.Create();
                }
            }
            
        }

        [MessageHandler((ushort)Messages.RpcMessage)]
        public static void RPC_MessageHandlerCLIENT(Message message) {
            
        }

        private static void HandleRPC(int viewId, int componentIndex, string methodName, byte[] args) {
            var view = Network.Buffer.Spawned[viewId];
            (view.Observables[componentIndex] as MonoBehaviour).SendMessage(methodName, args.FromBytes<object[]>());
        }

        public void FindObservables(){
            var observables = GetComponents<IObservable>();
            foreach (var observable in observables) {
                if (observable is MonoBehaviour) {
                    var mB = (MonoBehaviour)observable;
                    if (mB.GetNetworkView() == this)
                        Observables.Add(observable);
                }
            }
        }

        public void RequestOwnership(){
        
        }
        public void TransferOwnership(uint newActorNumber){
        
        }

        private void Start() {
            if (AutoFindObservables)
                FindObservables();
        }
    }

    public enum RpcTarget : uint {
        All,
        Others,
        Server
    }

    [Serializable]
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
            Targeted=targeted; ViewId = viewId; ComponentIndex = componentIndex; MethodName = methodName; Args = args; Target = target; Buffered = buffered;
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
