using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject savePanel;
    public GameObject audioPanel;

    public void OnPlayClicked()
    {
        SceneManager.LoadScene("GameScene"); // Or whatever your scene name is
    }

    public void OnLoadClicked()
    {
        mainPanel.SetActive(false);
        savePanel.SetActive(true);
    }

    public void OnAudioClicked()
    {
        mainPanel.SetActive(false);
        audioPanel.SetActive(true);
    }

    public void OnBackClicked()
    {
        savePanel.SetActive(false);
        audioPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
