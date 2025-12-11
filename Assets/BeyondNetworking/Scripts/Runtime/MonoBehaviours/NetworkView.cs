using UnityEngine;
using Riptide;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Beyond.Networking
{
    public class NetworkView : MonoBehaviour
    {
        public bool SharedOwnership = false;
        public List<ObservableBehaviour> Observables = new();
        public int SceneId {
            get; internal set;
        } = -1;
        public int InstantiationId {
            get; internal set; 
        } = -1;
        public int ViewId { get; internal set; } = 0;
        public ClientRef Owner { get; internal set; } = new();
        public bool IsMine => Owner.GetConnection() == Network.Mono.Client.Connection;

        public event Action<string, Message> PayloadReceived;

        public void SendPayload(string header, Message payload, MessageSendMode sendMode = MessageSendMode.Reliable) {
            Message payloadMessage = Message.Create(sendMode, Messages.EntityStateMessage);
            payloadMessage.Add(ViewId);
            payloadMessage.Add(header);
            payloadMessage.AddMessage(payload);
            Network.Send(payloadMessage);
        }

        [MessageHandler((ushort)Messages.EntityStateMessage)]
        internal static void EntityState_HandlerServer(ushort fromClientId, Message message) {
            Network.Send(message, true);
        }

        [MessageHandler((ushort)Messages.EntityStateMessage)]
        internal static void EntityState_HandlerClient(Message message) {
            var view = Network.Spawned[message.GetInt()];
            //Message.Create().Add(message.bits, message.UnreadBits);
            view.PayloadReceived.Invoke(message.GetString(), message);
        }

        [ContextMenu("Destroy")]
        public void Destroy() {
            Network.Destroy(this);
        }

        public void RPC(ObservableBehaviour observable, string methodName, RpcTarget target, bool buffered = false, MessageSendMode reliability = MessageSendMode.Unreliable, params object[] args){
            var componentIndex = Observables.ToList().IndexOf(observable);
            Message rpcMessage = Message.Create(reliability, Messages.RpcMessage);
            rpcMessage.AddSerializable<RpcMessage>(new(false, ViewId, componentIndex, methodName, args, (uint)target, buffered));
            Network.Mono.Client.Send(rpcMessage);
        }

        public void RPC(ObservableBehaviour observable, string methodName, ClientRef target, MessageSendMode reliability = MessageSendMode.Unreliable, params object[] args){
            var componentIndex = Observables.ToList().IndexOf(observable);
            Message rpcMessage = Message.Create(MessageSendMode.Reliable, Messages.RpcMessage);
            rpcMessage.AddSerializable<RpcMessage>(new(true, ViewId, componentIndex, methodName, args, target.ActorNumber, false));
            Network.Send(rpcMessage);
        }

        [MessageHandler((ushort)Messages.RpcMessage)]
        public static void RPC_MessageHandlerSERVER(ushort fromClientId, Message message) {
            Debug.Log($"Got RPC from {fromClientId}");
            var rpc = message.GetSerializable<RpcMessage>();
            if (rpc.Target == (uint)RpcTarget.Server && !rpc.Targeted) {
                HandleRPC(rpc);
                return;
            }
            else {
                Message clientRpcMessage = Message.Create(MessageSendMode.Reliable, Messages.RpcMessage);
                clientRpcMessage.Add(rpc);
                if (rpc.Targeted) {
                    Network.Send(clientRpcMessage, fromClientId);
                }
                else {
                    if(rpc.Target == (uint)RpcTarget.Others)
                        Network.Send(clientRpcMessage, fromClientId);
                    else
                        Network.Send(clientRpcMessage, true);
                }
            }               
        }

        [MessageHandler((ushort)Messages.RpcMessage)]
        public static void RPC_MessageHandlerCLIENT(Message message) {
            HandleRPC(message.GetSerializable<RpcMessage>());
        }

        private static void HandleRPC(RpcMessage rpc) {
            Network.Spawned[rpc.ViewId].HandleRPC(rpc.ComponentIndex, rpc.MethodName, rpc.Args);
        }

        internal void HandleRPC(int compIndex, string methodName, object[] args) {
            var method = Observables[compIndex];
            method.SendMessage(methodName, args);
        }

        public void FindObservables(){
            Observables = GetComponentsInChildren<ObservableBehaviour>().ToList();
            Observables.RemoveAll(x => x.GetNetworkView() != this);
        }

        public void RequestOwnership(){
        
        }

        [MessageHandler((ushort)Messages.ChangeOwnerMessage)]
        internal static void TransferOwnership_Handler(Message message) {
        
        }

        public void TransferOwnership(uint newActorNumber){
            
        }

        private void Awake() {
            FindObservables();
            
        }

        internal void Allocated() {
            if (SceneId > 0 && Network.isHost)
                TransferOwnership(Network.LocalClient.ActorNumber);
        }
    }
}
