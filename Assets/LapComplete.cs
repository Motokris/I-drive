using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LapComplete : MonoBehaviour
{
    public GameObject lapCompleteTrigg, halfLapTrigg;
    public GameObject bestMin, bestSec, bestMilisec, lapTimeBox, lapCount, endGameScreen;
    private int lapCounter = 0;


    /// <summary>
    /// Once the lapCompleteTrigg is triggered, checks if it should replace the best time
    /// </summary>
    private void OnTriggerEnter()
    {
        if (lapCounter == 0)
        {
            UpdateBestTime();
        }
        else
        {
            CheckTime();
        }

        LapTimeManager.milisec = 0;
        LapTimeManager.sec = 0;
        LapTimeManager.min = 0;


        lapCounter++;
        /*
        if (lapCounter == 3)
        {
            lapCount.SetActive(false);
            endGameScreen.SetActive(true);
            StartCoroutine(BackToMenu());
            SceneManager.LoadScene("Scenes/Main_menu");
        }

        lapCount.GetComponent<TextMeshProUGUI>().text = "Lap: " + lapCounter.ToString() + "/2";
        */

        halfLapTrigg.SetActive(true);
        lapCompleteTrigg.SetActive(false);
    }

    IEnumerator BackToMenu()
    {
        yield return new WaitForSeconds(6);
    }

    /// <summary>
    /// Checks if the new time is better than the best time
    /// </summary>
    void CheckTime()
    {
        float min, sec;
        float milisec;

        bestMin.GetComponent<TextMeshProUGUI>().ForceMeshUpdate(true);
        bestSec.GetComponent<TextMeshProUGUI>().ForceMeshUpdate(true);
        bestMilisec.GetComponent<TextMeshProUGUI>().ForceMeshUpdate(true);

        float.TryParse(bestMin.GetComponent<TextMeshProUGUI>().text, out min);
        float.TryParse(bestSec.GetComponent<TextMeshProUGUI>().text, out sec);
        float.TryParse(bestMilisec.GetComponent<TextMeshProUGUI>().text, out milisec);


        if (LapTimeManager.min < min)
        {
            UpdateBestTime();
        }
        else if (LapTimeManager.sec < sec && LapTimeManager.min == min)
        {
            UpdateBestTime();
        }
        else if (LapTimeManager.milisec < milisec && LapTimeManager.sec == sec && LapTimeManager.min == min)
        {
            UpdateBestTime();
        }
    }

    void UpdateBestTime()
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
    }
}
