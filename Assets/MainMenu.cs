using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void SelectionScreen()
    {
        SceneManager.LoadScene("Scenes/Selection");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
