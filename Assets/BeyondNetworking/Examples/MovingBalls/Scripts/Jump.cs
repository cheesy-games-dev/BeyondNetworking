using Beyond.Networking;
using UnityEngine;

public class Jump : ObservableBehaviour
{
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!NetworkView.IsMine) return;
        if (Input.GetKeyDown(KeyCode.Z)) {
            Debug.Log("Sending Jump RPC");
            RPC(nameof(JumpRPC), reliability: Riptide.MessageSendMode.Reliable);
        }
        if (Input.GetKeyDown(KeyCode.X)) {
            Debug.Log("Requesting Ownership");
            NetworkView.RequestOwnership();
        }
    }
    public Rigidbody rb;
    public float jumpForce;

    protected override string _payloadHeaderKey => "JUMP";

    public void JumpRPC() {
        rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
    }
}
