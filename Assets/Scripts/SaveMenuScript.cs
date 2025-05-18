using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
using System.Collections.Generic; // For List

[System.Serializable]
public class SaveSlotUIElements 
{
    public int slotNumber; 
    public GameObject slotRootPanel; 

    [Header("Panels within Slot")]
    public GameObject hasSavePanel; // Panel to show if save exists
    public GameObject noSavePanel;  // Panel to show if slot is empty

    [Header("Text Elements")]
    public TextMeshProUGUI infoText_HasSave; // e.g., "Money: 1000 | Player: Pat | Enemies: 5"
    public TextMeshProUGUI slotNameText_HasSave; // e.g. "Slot 1 - Forest Glade"
    public TextMeshProUGUI slotNameText_NoSave; // e.g. "Slot 1 - Empty"
}

public class SaveMenuScript : MonoBehaviour
{
    [Header("Scene Configuration")]
    [SerializeField] private string gameSceneName = "GameScene"; 

    [Header("Main UI Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject saveSlotsPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Save Slot UI Collection")]
    [SerializeField] private List<SaveSlotUIElements> saveSlotVisuals; // References to the visual parts of each slot

    private void Start()
    {
        if (SaveSystem.instance == null)
        {
            Debug.LogError("SaveSystem instance not found! Ensure it's initialized before this menu.");
            // Optionally disable UI or show an error message to the player
            return;
        }
        ShowMainPanel(); // Start by showing the main menu panel
    }

    // Manages which main panel (Main, SaveSlots, Audio, Options) is active
    private void ShowPanel(GameObject panelToShow)
    {
        mainPanel.SetActive(panelToShow == mainPanel);
        saveSlotsPanel.SetActive(panelToShow == saveSlotsPanel);
        audioPanel.SetActive(panelToShow == audioPanel);
        optionsPanel.SetActive(panelToShow == optionsPanel);

        // If showing the save slots panel, refresh its UI
        if (panelToShow == saveSlotsPanel)
        {
            UpdateAllSlotsVisuals();
        }
    }

    // Public methods to be called by UI buttons for panel navigation
    public void ShowMainPanel() => ShowPanel(mainPanel);
    public void ShowSaveSlotsPanel() => ShowPanel(saveSlotsPanel);
    public void ShowAudioPanel() => ShowPanel(audioPanel);
    public void ShowOptionsPanel() => ShowPanel(optionsPanel);

    public void QuitGame()
    {
        Debug.Log("QuitGame method called.");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Also stop play mode in the editor
#endif
    }

    private void UpdateAllSlotsVisuals()
    {
        if (SaveSystem.instance == null)
        {
            Debug.LogError("SaveSystem instance not available for UI update.");
            return;
        }

        foreach (var slotVisuals in saveSlotVisuals)
        {
            bool hasSave = SaveSystem.instance.HasSave(slotVisuals.slotNumber);
            slotVisuals.hasSavePanel.SetActive(hasSave);
            slotVisuals.noSavePanel.SetActive(!hasSave);



            if (hasSave)
            {
                SaveData tempData = SaveSystem.instance.PeekSlotData(slotVisuals.slotNumber);
                if (tempData != null && tempData.player != null)
                {
                    if (slotVisuals.infoText_HasSave != null)
                    {
                        // Customize this text as needed
                        slotVisuals.infoText_HasSave.text = $"Player: {tempData.player.psuedo}\nMoney: {tempData.player.stats.money}\nEnemies Defeated: {tempData.gameProgress.enemiesDestroyedCount}";
                    }
                    if (slotVisuals.slotNameText_HasSave != null)
                    {
                        slotVisuals.slotNameText_HasSave.text = $"Save Slot {slotVisuals.slotNumber}"; // Or a custom name/timestamp
                    }
                }
                else
                {
                    if (slotVisuals.infoText_HasSave != null) slotVisuals.infoText_HasSave.text = "Data Error";
                    if (slotVisuals.slotNameText_HasSave != null) slotVisuals.slotNameText_HasSave.text = $"Save Slot {slotVisuals.slotNumber} - Error";
                }
            }
            else
            {
                if (slotVisuals.slotNameText_NoSave != null)
                {
                    slotVisuals.slotNameText_NoSave.text = $"Save Slot {slotVisuals.slotNumber} - Empty";
                }
                if (slotVisuals.infoText_HasSave != null && slotVisuals.noSavePanel.activeSelf)
                {
      
                }
            }
        }
    }



    public void OnContinueButtonPressed(int slotNumber)
    {
        Debug.Log($"Continue button pressed for slot: {slotNumber}");
        if (SaveSystem.instance == null) return;

        PlayerPrefs.SetInt("SaveIndex", slotNumber);
        SaveSystem.instance.SetSlot(slotNumber);
        SaveSystem.instance.Load(slotNumber);
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnNewGameButtonPressed(int slotNumber)
    {
        Debug.Log($"New Game button pressed for slot: {slotNumber}");
        if (SaveSystem.instance == null) return;


        PlayerPrefs.SetInt("SaveIndex", slotNumber);
        SaveSystem.instance.SetSlot(slotNumber);
        SaveSystem.instance.ResetSlot(slotNumber); // Deletes old file, creates new empty SaveData in memory
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnDeleteButtonPressed(int slotNumber)
    {
        Debug.Log($"Delete button pressed for slot: {slotNumber}");
        if (SaveSystem.instance == null) return;

        // IMPORTANT: Add a confirmation dialog here! "Are you sure you want to delete this save?"
        // Example (pseudo-code for confirmation):
        // ConfirmationDialog.Show("Delete Save?", $"Really delete save data for slot {slotNumber}?", () => {
        //     SaveSystem.instance.ResetSlot(slotNumber);
        //     UpdateAllSlotsVisuals(); // Refresh UI
        // });

        SaveSystem.instance.ResetSlot(slotNumber); 
        UpdateAllSlotsVisuals(); 
    }

   

    public void SaveCurrentGameToSlot(int slotNumber)
    {
        if (SaveSystem.instance == null)
        {
            Debug.LogError("SaveSystem not available to save game.");
            return;
        }

        Debug.Log($"Attempting to save current game to slot: {slotNumber}");
        PlayerPrefs.SetInt("SaveIndex", slotNumber); // Good to keep this updated
        SaveSystem.instance.SetSlot(slotNumber); 
        SaveSystem.instance.Save(); 
        Debug.Log($"Game explicitly saved to slot {slotNumber}");

        if (saveSlotsPanel.activeInHierarchy)
        {
            UpdateAllSlotsVisuals();
        }
    }

    
    public void LoadSlotDataIntoCurrentScene(int slotNumber)
    {
        if (SaveSystem.instance == null)
        {
            Debug.LogError("SaveSystem not available to load data into current scene.");
            return;
        }

        Debug.Log($"Loading data from slot {slotNumber} into current game session without scene reload.");
        PlayerPrefs.SetInt("SaveIndex", slotNumber);
        SaveSystem.instance.SetSlot(slotNumber);
        SaveSystem.instance.Load(slotNumber); // Load data into SaveSystem.instance.data

        PlayerSave player = FindObjectOfType<PlayerSave>();
        if (player != null)
        {
            player.ApplyLoadedData(); // This method in PlayerSave should read from SaveSystem.instance.data
        }
        else Debug.LogError("PlayerSave object not found to apply loaded data.");

        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader != null)
        {
            levelLoader.SpawnCoinsFromSave(); // Assumes this clears old and spawns from SaveSystem.instance.data
        }
        else Debug.LogWarning("LevelLoader not found. Cannot update coins from loaded data.");

        

        Debug.Log($"Data from slot {slotNumber} applied to current scene state.");

        if (saveSlotsPanel.activeInHierarchy)
        {
            UpdateAllSlotsVisuals();
        }
    }
}