using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beyond.Networking
{
    public class NetworkView : MonoBehaviour
    {
        public MessageSendMode Reliability;
        public uint SceneId;
        public uint InstantiationId;
        public uint ViewId;
        public ClientRef Owner;
        public bool IsMine => Owner.GetConnection() == BeyondNetwork.Mono.Client.Connection;
        public List</*IObservable (add it)*/> Observables;
        
        public void RPC(Component component, string methodName, RpcTarget target, bool buffered = false, params object[] parameters = null){
        
        }

        public void RPC(Component component, string methodName, ClientRef player, rams object[] parameters = null){
        
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
