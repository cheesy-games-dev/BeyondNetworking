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
        public ushort Owner { get; internal set; } = 0;
        public bool IsMine {
            get {
                if (Network.Client.Connection != null)
                    return Owner == Network.Client.Id;
                else
                    return true;
            }
        }

        [ContextMenu("Destroy")]
        public void Destroy() {
            Network.Destroy(this);
        }

        public void RPC(ObservableBehaviour observable, string methodName, NetworkTarget target, bool buffered = false, MessageSendMode reliability = MessageSendMode.Unreliable, params object[] args){
            var componentIndex = Observables.ToList().IndexOf(observable);
            Message rpcMessage = Message.Create(reliability, MessageIds.RpcMessage);
            rpcMessage.AddSerializable<RpcMessage>(new(false, ViewId, componentIndex, methodName, args, (uint)target, buffered));
            Network.Mono.Client.Send(rpcMessage);
        }

        public void RPC(ObservableBehaviour observable, string methodName, ushort target, MessageSendMode reliability = MessageSendMode.Unreliable, params object[] args){
            var componentIndex = Observables.ToList().IndexOf(observable);
            Message rpcMessage = Message.Create(MessageSendMode.Reliable, MessageIds.RpcMessage);
            rpcMessage.AddSerializable<RpcMessage>(new(true, ViewId, componentIndex, methodName, args, target, false));
            Network.Send(rpcMessage);
        }

        [MessageHandler((ushort)MessageIds.RpcMessage)]
        public static void RPC_MessageHandlerSERVER(ushort fromClientId, Message message) {
            Debug.Log($"Got RPC from {fromClientId}");
            var rpc = message.GetSerializable<RpcMessage>();
            if (rpc.Target == (uint)NetworkTarget.Server && !rpc.Targeted) {
                HandleRPC(rpc);
                return;
            }
            else {
                Message clientRpcMessage = Message.Create(MessageSendMode.Reliable, MessageIds.RpcMessage);
                clientRpcMessage.Add(rpc);
                if (rpc.Targeted) {
                    Network.Send(clientRpcMessage, fromClientId);
                }
                else {
                    if(rpc.Target == (uint)NetworkTarget.Others)
                        Network.Send(clientRpcMessage, fromClientId);
                    else
                        Network.Send(clientRpcMessage, true);
                }
            }               
        }

        [MessageHandler((ushort)MessageIds.RpcMessage)]
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
            if (Network.isHost)
                TransferOwnership(Network.Client.Id);
        }

        [MessageHandler((ushort)MessageIds.ChangeOwnerMessage)]
        internal static void TransferOwnership_Handler(ushort fromClientId, Message message) {
            var backup = message;
            var view = Network.Spawned[backup.GetInt()];
            if (view.SharedOwnership)
                Network.Send(message, true);
        }

        [MessageHandler((ushort)MessageIds.ChangeOwnerMessage)]
        internal static void TransferOwnership_Handler(Message message) {
            var view = Network.Spawned[message.GetInt()];
            view.Owner = message.GetUShort();
        }

        public void TransferOwnership(int newActorNumber){
            if (!Network.isHost)
                return;
            Message message = Message.Create(MessageSendMode.Reliable, MessageIds.ChangeOwnerMessage);
            message.Add(ViewId).Add(newActorNumber);
            Network.Send(message, true);
        }

        private void Awake() {
            FindObservables();     
        }

        internal void Allocated() {
            if (SceneId > 0 && Network.isHost)
                TransferOwnership(Network.Client.Id);
        }
    }
}
