using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionMenu : MonoBehaviour
{
    private GameObject[] carList, mapList, modeList;
    public int carIndex = 0, mapIndex = 0, modeIndex = 0;
    private string[] maps = { "Scenes/Free_roam", "Scenes/Map1" };
    private string[] modes = { "Free roam", "Time Trial" };

    // Start is called before the first frame update
    void Start()
    {
        carIndex = PlayerPrefs.GetInt("CarSelected");
        mapIndex = PlayerPrefs.GetInt("MapSelected");
        modeIndex = PlayerPrefs.GetInt("ModeSelected");

        carList = new GameObject[GameObject.Find("CarList").transform.childCount];
        mapList = new GameObject[GameObject.Find("MapList").transform.childCount];
        modeList = new GameObject[GameObject.Find("ModeList").transform.childCount];

        for (int i = 0; i < GameObject.Find("CarList").transform.childCount; i++)
        {
            carList[i] = GameObject.Find("CarList").transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < GameObject.Find("MapList").transform.childCount; i++)
        {
            mapList[i] = GameObject.Find("MapList").transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < GameObject.Find("ModeList").transform.childCount; i++)
        {
            modeList[i] = GameObject.Find("ModeList").transform.GetChild(i).gameObject;
        }

        foreach (GameObject go in carList)
        {
            go.SetActive(false);
        }

        foreach (GameObject go in mapList)
        {
            go.SetActive(false);
        }

        foreach (GameObject go in modeList)
        {
            go.SetActive(false);
        }

        if (carList[carIndex])
        {
            carList[carIndex].SetActive(true);
        }

        if (mapList[mapIndex])
        {
            mapList[mapIndex].SetActive(true);
        }

        if (modeList[modeIndex])
        {
            modeList[modeIndex].SetActive(true);
        }
    }

    public void ToggleCarLeft()
    {
        carList[carIndex].SetActive(false);

        carIndex--;
        if (carIndex < 0)
            carIndex = carList.Length - 1;

        carList[carIndex].SetActive(true);
    }

    public void ToggleCarRight()
    {
        carList[carIndex].SetActive(false);

        carIndex++;
        if (carIndex == carList.Length)
            carIndex = 0;

        carList[carIndex].SetActive(true);
    }

    public void ToggleMapLeft()
    {
        mapList[mapIndex].SetActive(false);

        mapIndex--;
        if (mapIndex < 0)
            mapIndex = mapList.Length - 1;

        mapList[mapIndex].SetActive(true);
    }

    public void ToggleMapRight()
    {
        mapList[mapIndex].SetActive(false);

        mapIndex++;
        if (mapIndex == mapList.Length)
            mapIndex = 0;

        mapList[mapIndex].SetActive(true);
    }

    public void ToggleModeLeft()
    {
        modeList[modeIndex].SetActive(false);

        modeIndex--;
        if (modeIndex < 0)
            modeIndex = modeList.Length - 1;

        modeList[modeIndex].SetActive(true);
    }

    public void ToggleModeRight()
    {
        modeList[modeIndex].SetActive(false);

        modeIndex++;
        if (modeIndex == modeList.Length)
            modeIndex = 0;

        modeList[modeIndex].SetActive(true);
    }

    public void Confirm()
    {
        PlayerPrefs.SetInt("CarSelected", carIndex);
        PlayerPrefs.SetInt("MapSelected", mapIndex);
        PlayerPrefs.SetInt("ModeSelected", modeIndex);

        SceneManager.LoadScene(maps[mapIndex]);
    }
}
