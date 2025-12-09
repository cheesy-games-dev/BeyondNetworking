using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond.Networking
{
    public class NetworkView : MonoBehaviour
    {
        public MessageSendMode Reliability = MessageSendMode.Unreliable;
        public bool SharedOwnership = true;
        public uint SceneId = 0;
        public uint InstantiationId = 0;
        public uint ViewId = 0;
        public ClientRef Owner = new();
        public bool IsMine => Owner.GetConnection() == BeyondNetwork.Mono.Client.Connection;
        public IObservable[] Observables = new();
        
        public void RPC(Component component, string methodName, RpcTarget target, bool buffered = false, params object[] parameters = null){
        
        }

        public void RPC(Component component, string methodName, ClientRef player, params object[] parameters = null){
        
        }
        
        public void FindObservables(){
        
        }

        public void RequestOwnership(){
        
        }
        public void TransferOwnership(uint newActorNumber){
        
        }
    }

    public enum RpcTarget : ushort {
        All,
        Others,
        Server
    }
}
