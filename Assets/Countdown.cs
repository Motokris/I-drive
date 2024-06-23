using System.Collections;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public GameObject countdown, lapTimer;
    private GameObject car;
    private string[] cars = { "Chevy", "Ford", "Ferrari", "Bugatti" };
    public GameObject[] carPrefabs;

    private void Start()
    {
        foreach (GameObject go in carPrefabs)
        {
            if (go.name == cars[PlayerPrefs.GetInt("CarSelected")])
            {
                car = go;
            }
        }
        StartCoroutine(CountStart());
    }

    IEnumerator CountStart()
    {
        car.GetComponent<CarController>().clutch = 1;
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

        car.GetComponent<CarController>().clutch = 0;
        lapTimer.SetActive(true);
    }
}
