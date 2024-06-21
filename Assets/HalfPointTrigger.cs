using UnityEngine;

public class HalfPointTrigger : MonoBehaviour
{
    public GameObject lapCompleteTrig, halfLapTrig;

    private void OnTriggerEnter()
    {
        lapCompleteTrig.SetActive(true);
        halfLapTrig.SetActive(false);
    }
}
