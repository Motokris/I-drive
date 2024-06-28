using UnityEngine;

public class BrakeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        AICarController car = other.GetComponent<AICarController>();
        if (car)
        {
            car.isBraking = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AICarController car = other.GetComponent<AICarController>();
        if (car)
        {
            car.isBraking = false;
        }
    }
}
