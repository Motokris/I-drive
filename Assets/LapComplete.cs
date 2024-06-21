using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LapComplete : MonoBehaviour
{
    public GameObject lapCompleteTrigg, halfLapTrigg;
    public GameObject bestMin, bestSec, bestMilisec, lapTimeBox, lapCount, endGameScreen;
    private int lapCounter = 1;

    private void OnTriggerEnter()
    {
        if (PlayerPrefs.GetInt("ModeSelected") == 1)
        {
            if (LapTimeManager.sec <= 9)
            {
                bestSec.GetComponent<TextMeshProUGUI>().text = "0" + LapTimeManager.sec.ToString("F0") + ".";
            }
            else
            {
                bestSec.GetComponent<TextMeshProUGUI>().text = "" + LapTimeManager.sec.ToString("F0") + ".";
            }

            if (LapTimeManager.min <= 9)
            {
                bestMin.GetComponent<TextMeshProUGUI>().text = "0" + LapTimeManager.min.ToString("F0") + ".";
            }
            else
            {
                bestMin.GetComponent<TextMeshProUGUI>().text = "" + LapTimeManager.min.ToString("F0") + ".";
            }

            bestMilisec.GetComponent<TextMeshProUGUI>().text = "" + LapTimeManager.milisec.ToString("F0");

            LapTimeManager.milisec = 0;
            LapTimeManager.sec = 0;
            LapTimeManager.min = 0;
        }

        else if (PlayerPrefs.GetInt("ModeSelected") == 3)
        {
            lapCounter++;
            if (lapCounter == 3)
            {
                lapCount.SetActive(false);
                endGameScreen.SetActive(true);
                StartCoroutine(BackToMenu());
                SceneManager.LoadScene("Scenes/Main_menu");
            }
            lapCount.GetComponent<TextMeshProUGUI>().text = "Lap: " + lapCounter.ToString() + "/2";
        }

        halfLapTrigg.SetActive(true);
        lapCompleteTrigg.SetActive(false);
    }

    IEnumerator BackToMenu()
    {
        yield return new WaitForSeconds(6);
    }
}
