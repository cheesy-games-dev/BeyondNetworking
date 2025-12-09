using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Riptide;

namespace Beyond.Networking
{
    public class NetworkView : MonoBehaviour
    {
        public bool SharedOwnership = false;
        public int SceneId = 0;
        public int InstantiationId { get; internal set; } = 0;
        public int ViewId { get; internal set; } = 0;
        public ClientRef Owner { get; internal set; } = new();
        public bool IsMine => Owner.GetConnection() == Network.Mono.Client.Connection;
        public IObservable[] Observables;
        
        public void RPC(IObservable component, string methodName, RpcTarget target, bool buffered = false, MessageSendMode sendMode = MessageSendMode.Unreliable, params object[] parameters){
            
        }

        public void RPC(IObservable component, string methodName, ClientRef player, MessageSendMode sendMode = MessageSendMode.Unreliable, params object[] parameters){
        
        }
        
        public void FindObservables(){
        
        }

        public void RequestOwnership(){
        
        }
        public void TransferOwnership(uint newActorNumber){
        
        }

        private void Start() {
        }
    }

    public enum RpcTarget : ushort {
        All,
        Others,
        Server
    }
}
