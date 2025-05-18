using UnityEngine;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;

    void Update()
    {
        // Toggle on Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;     // freeze all Update/fixed physics
        isPaused = true;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;     // resume normal time
        isPaused = false;
    }

    public void QuitToMenu()
    {
        Resume();                // ensure timeScale reset
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
