using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private Vector3 Offset;
    private Rigidbody rb;

    void Awake()
    {
        rb = player.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        follow();
    }

    private void follow()
    {
        Vector3 playerForward = (rb.velocity + player.transform.forward).normalized;
        Offset.y = 2;
        Offset.z = -1;
        gameObject.transform.position = Vector3.Lerp(transform.position, player.position + player.transform.TransformVector(Offset) + playerForward * (-3f), 
            Time.deltaTime * 10);
        gameObject.transform.LookAt(player.transform.position);
    }
}
