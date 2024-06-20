using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour
{
    private GameObject[] gameObjectsList;
    private EventSystem eventSystem;

    private void Start()
    {
        gameObjectsList = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            gameObjectsList[i] = transform.GetChild(i).gameObject;
        }
    }
}
