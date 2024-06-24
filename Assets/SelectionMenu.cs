using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionMenu : MonoBehaviour
{
    private GameObject[] carList, mapList;
    public int carIndex = 0, mapIndex = 0;
    private string[] maps = { "Scenes/Map1", "Scenes/Map2" };

    /// <summary>
    /// Start is called before the Update function is called. Here it remembers the player's selection in a PlayerPrefs in the user's platform registry and loads the selected choices.
    /// </summary>
    void Start()
    {
        carIndex = PlayerPrefs.GetInt("CarSelected");
        mapIndex = PlayerPrefs.GetInt("MapSelected");

        carList = new GameObject[GameObject.Find("CarList").transform.childCount];
        mapList = new GameObject[GameObject.Find("MapList").transform.childCount];

        for (int i = 0; i < GameObject.Find("CarList").transform.childCount; i++)
        {
            carList[i] = GameObject.Find("CarList").transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < GameObject.Find("MapList").transform.childCount; i++)
        {
            mapList[i] = GameObject.Find("MapList").transform.GetChild(i).gameObject;
        }

        foreach (GameObject go in carList)
        {
            go.SetActive(false);
        }

        foreach (GameObject go in mapList)
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

    public void Confirm()
    {
        PlayerPrefs.SetInt("CarSelected", carIndex);
        PlayerPrefs.SetInt("MapSelected", mapIndex);

        SceneManager.LoadScene(maps[mapIndex]);
    }
}
