using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{

    public static bool GamePaused = false;
    public GameObject pauseUI, speedometerUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        speedometerUI.SetActive(true);
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
    }

    private void Pause()
    {
        speedometerUI.SetActive(false);
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scenes/Main_menu");
    }
}
