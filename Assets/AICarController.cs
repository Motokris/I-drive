using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class AICarController : MonoBehaviour
{
    public WaypointManager waypointManager;
    public List<Transform> waypoints;
    public int currentWaypoint;
    private CarController controller;
    public float range;
    private float currentAngle, acceleration, maxAngle = 45f, maxSpeed = 150f, dampenAcc;
    public bool isBraking;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CarController>();
        waypoints = waypointManager.waypoints;
        currentWaypoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(waypoints[currentWaypoint].position, transform.position) < range)
        {
            currentWaypoint++;
            if (currentWaypoint == waypoints.Count)
            {
                currentWaypoint = 0;
            }
        }
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        currentAngle = Vector3.SignedAngle(forward, waypoints[currentWaypoint].position - transform.position, Vector3.up);
        acceleration = Mathf.Clamp01(1f - Mathf.Abs(controller.speed * 0.01f * currentAngle) / maxAngle);
        if (isBraking)
        {
            acceleration = -acceleration * (Mathf.Clamp01(controller.speed / maxSpeed) * 2 - 1f);
        }
        dampenAcc = Mathf.Lerp(dampenAcc, acceleration, Time.deltaTime * 3f);
        controller.SetInput(dampenAcc, currentAngle, 0, 0);

    }
}
