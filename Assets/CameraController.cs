using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform player;
    public Vector3 offset;
    public float speed;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 playerForward = (rb.velocity + player.transform.forward).normalized;
        transform.position = Vector3.Lerp(transform.position,
            player.position + player.transform.TransformVector(offset) + playerForward * (-5f), speed * Time.deltaTime);
        transform.LookAt(player);
    }
}
