using TMPro;
using UnityEngine;


public class LapTimeManager : MonoBehaviour
{
    public static float min, sec;
    public static float milisec;

    public GameObject minBox, secBox, milisecBox;


    // Update is called once per frame
    void Update()
    {
        milisec += Time.deltaTime * 10;
        milisecBox.GetComponent<TextMeshProUGUI>().text = "0" + milisec.ToString("F0"); ;

        if (milisec > 10)
        {
            sec += 1;
            milisec = 0;
        }

        if (sec < 10)
        {
            secBox.GetComponent<TextMeshProUGUI>().text = "0" + sec.ToString("F0") + ".";
        }
        else
        {
            secBox.GetComponent<TextMeshProUGUI>().text = "" + sec.ToString("F0") + ".";
        }

        if (sec >= 60)
        {
            sec = 0;
            min += 1;
        }

        if (min < 10)
        {
            minBox.GetComponent<TextMeshProUGUI>().text = "0" + min.ToString("F0") + ":";
        }
        else
        {
            minBox.GetComponent<TextMeshProUGUI>().text = "" + min.ToString("F0") + ":";
        }
    }
}
