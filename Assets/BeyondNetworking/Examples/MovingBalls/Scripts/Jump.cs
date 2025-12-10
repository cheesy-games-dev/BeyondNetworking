using Beyond.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour, IObservable
{
    public void Deserialize() {
        throw new System.NotImplementedException();
    }

    public void Serialize() {
        throw new System.NotImplementedException();
    }

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            this.GetNetworkView().RPC(this, );
        }
    }
    public Rigidbody rb;
    public void JumpRPC() {
        rb.AddForce(Vector3.up * 50, ForceMode.Impulse);
    }
}
