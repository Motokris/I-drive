using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private Vector3 Offset;
    private Rigidbody rb;


    /// <summary>
    /// Awake is being called when an enabled script instance is loaded, before the application starts
    /// </summary>
    void Awake()
    {
        rb = player.GetComponent<Rigidbody>();
    }


    /// <summary>
    /// Called every fixed frame and has the frequency of the physics system.
    /// </summary>
    void FixedUpdate()
    {
        follow();
    }


    /// <summary>
    /// Using the player controlled entity's speed, change the camera's coordinates to follow the entity
    /// </summary>
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
