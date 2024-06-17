using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform player;
    public float speed;
    public Vector3 Offset;
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
        gameObject.transform.position = Vector3.Lerp(transform.position, player.position + player.transform.TransformVector(Offset) + playerForward * (-3f), 
            Time.deltaTime * speed);
        gameObject.transform.LookAt(player.transform.position);
    }
}
