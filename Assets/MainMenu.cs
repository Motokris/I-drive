using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void FreeRoam()
    {
        SceneManager.LoadScene("Scenes/Map1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
