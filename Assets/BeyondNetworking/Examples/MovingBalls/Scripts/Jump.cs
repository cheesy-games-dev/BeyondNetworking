using Beyond.Networking;
using UnityEngine;

public class Jump : ObservableBehaviour
{
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && NetworkView.IsMine) {
            RPC(nameof(JumpRPC), reliability: Riptide.MessageSendMode.Reliable);
        }
        if (Input.GetKeyDown(KeyCode.X) && NetworkView.IsMine) {
            NetworkView.RequestOwnership();
        }
    }
    public Rigidbody rb;
    public float jumpForce;
    public void JumpRPC() {
        rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
    }
}
