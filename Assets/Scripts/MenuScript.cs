using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveMenuScript : MonoBehaviour
{
    [SerializeField] private int saveNumber = 1;
    [SerializeField] private GameObject hasSavePanel;
    [SerializeField] private GameObject noSavePanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private string gameSceneName = "GameScene";

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject saveSlotsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject optionsPanel;

 

    private void Start()
    {
        UpdateUI();
        // At start, only the main panel is visible
        mainPanel.SetActive(true);
        saveSlotsPanel.SetActive(false);
        audioPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }



    private void UpdateUI()
    {
        bool has = SaveSystem.instance.HasSave(saveNumber);
        hasSavePanel.SetActive(has);
        noSavePanel.SetActive(!has);
        if (has)
        {
            SaveSystem.instance.Load(saveNumber);
            scoreText.text = SaveSystem.instance.data.player.stats.money.ToString();
        }
    }
    public void PlayButton()
    {
        mainPanel.SetActive(false);
        saveSlotsPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }


    public void Back()
    {
        // Hide all sub-panels
        saveSlotsPanel.SetActive(false);
        audioPanel.SetActive(false);
        optionsPanel.SetActive(false);
        // …and any you added

        // Show the main
        mainPanel.SetActive(true);
    }

    public void AudioButton()
    {
        mainPanel.SetActive(false);
        audioPanel.SetActive(true);
    }

    public void OptionsButton()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }


    // Existing: jumps into gameplay
    public void Continue()
    {
        PlayerPrefs.SetInt("SaveIndex", saveNumber);
        SceneManager.LoadScene(gameSceneName);
    }

    public void NewGame()
    {
        PlayerPrefs.SetInt("SaveIndex", saveNumber);
        SaveSystem.instance.ResetSlot(saveNumber);
        SceneManager.LoadScene(gameSceneName);
    }

    public void Delete()
    {
        PlayerPrefs.SetInt("SaveIndex", saveNumber);
        SaveSystem.instance.ResetSlot(saveNumber);
        UpdateUI();
    }

    public void SaveHere()
    {
        PlayerPrefs.SetInt("SaveIndex", saveNumber);
        SaveSystem.instance.SetSlot(saveNumber);

        SaveSystem.instance.Save();
        UpdateUI();
    }

    // ← NEW: reloads the current scene state from the slot without quitting to menu
    public void LoadHere()
    {
        PlayerPrefs.SetInt("SaveIndex", saveNumber);
        SaveSystem.instance.SetSlot(saveNumber);
        SaveSystem.instance.Load(saveNumber);

        // 1) Reposition player
        var player = FindObjectOfType<PlayerInfo>();
        player.psuedo = SaveSystem.instance.data.player.psuedo;
        player.stats = SaveSystem.instance.data.player.stats;
        player.transform.position = SaveSystem.instance
                                       .data.player
                                       .position
                                       .ToVector3();

        // 2) Clear old coins and respawn
        foreach (var c in LevelLoader.instance.mCoins)
            Destroy(c);
        LevelLoader.instance.mCoins.Clear();
        LevelLoader.instance.SpawnCoinsFromSave();

        // 3) Update UI/score display
        UpdateUI();
    }
}
