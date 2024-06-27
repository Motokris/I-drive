using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public List<Transform> waypoints;

    void Awake()
    {
        foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
        {
            waypoints.Add(t);
        }
        waypoints.Remove(waypoints[0]);
    }

    void Update()
    {

    }
}
