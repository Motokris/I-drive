using System.Collections;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public GameObject countdown, lapTimer;

    private void Start()
    {
        StartCoroutine(CountStart());
    }

    IEnumerator CountStart()
    {
        yield return new WaitForSeconds(0.5f);
        countdown.GetComponent<TextMeshProUGUI>().text = "3";
        countdown.SetActive(true);

        yield return new WaitForSeconds(1);
        countdown.SetActive(false);
        countdown.GetComponent<TextMeshProUGUI>().text = "2";
        countdown.SetActive(true);

        yield return new WaitForSeconds(1);
        countdown.SetActive(false);
        countdown.GetComponent<TextMeshProUGUI>().text = "1";
        countdown.SetActive(true);

        yield return new WaitForSeconds(1);
        countdown.SetActive(false);


        lapTimer.SetActive(true);
    }
}
