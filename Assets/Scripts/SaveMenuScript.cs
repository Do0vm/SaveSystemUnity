// SaveMenuScript.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Linq; // Needed for List operations if any are added

[System.Serializable]
public class SaveSlotUIElements
{
    public int slotNumber;
    public GameObject slotRootPanel;

    [Header("Panels within Slot")]
    public GameObject hasSavePanel; // Panel to show if save exists
    public GameObject noSavePanel; // Panel to show if slot is empty

    [Header("Text Elements")]
    public TextMeshProUGUI infoText_HasSave; // e.g., "Player: Pat | Lvl: 5 | Money: 1000 | Enemies: 5"
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
            // Find or instantiate SaveSystem if it doesn't exist? (Less common pattern)
            GameObject saveSystemGO = GameObject.Find("SaveSystem"); // Or the name of your SaveSystem GameObject
            if (saveSystemGO == null)
            {
                Debug.LogError("SaveSystem GameObject not found in scene!");
                // Consider instantiating the prefab if it should be auto-created
            }
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

    // Refreshes the visual display of all save slots in the UI
    private void UpdateAllSlotsVisuals()
    {
        if (SaveSystem.instance == null)
        {
            Debug.LogError("SaveSystem instance not available for UI update.");
            return;
        }

        Debug.Log("Updating all save slot visuals...");

        foreach (var slotVisuals in saveSlotVisuals)
        {
            bool hasSave = SaveSystem.instance.HasSave(slotVisuals.slotNumber);
            slotVisuals.hasSavePanel.SetActive(hasSave);
            slotVisuals.noSavePanel.SetActive(!hasSave);

            if (hasSave)
            {
                // Peek the data directly from the file for display
                SaveData tempData = SaveSystem.instance.PeekSlotData(slotVisuals.slotNumber);
                // Ensure tempData and its nested objects are not null before accessing
                if (tempData != null && tempData.player != null && tempData.player.stats != null && tempData.gameProgress != null)
                {
                    if (slotVisuals.infoText_HasSave != null)
                    {
                        // Customize this text as needed - Includes player level and enemies destroyed
                        slotVisuals.infoText_HasSave.text = $"Player: {tempData.player.psuedo} | Lvl: {tempData.player.stats.lvl}\nMoney: {tempData.player.stats.money}\nEnemies Defeated: {tempData.gameProgress.enemiesDestroyedCount}";
                    }
                    if (slotVisuals.slotNameText_HasSave != null)
                    {
                        slotVisuals.slotNameText_HasSave.text = $"Save Slot {slotVisuals.slotNumber}"; // Or a custom name/timestamp
                    }
                }
                else
                {
                    // Handle cases where file exists but data is corrupted or incomplete
                    Debug.LogWarning($"Save file for slot {slotVisuals.slotNumber} seems corrupted or incomplete.");
                    if (slotVisuals.infoText_HasSave != null) slotVisuals.infoText_HasSave.text = "Data Error or Empty";
                    if (slotVisuals.slotNameText_HasSave != null) slotVisuals.slotNameText_HasSave.text = $"Save Slot {slotVisuals.slotNumber} - Error";
                }
            }
            else
            {
                // Slot is empty
                if (slotVisuals.slotNameText_NoSave != null)
                {
                    slotVisuals.slotNameText_NoSave.text = $"Save Slot {slotVisuals.slotNumber} - Empty";
                }
                // No need to set infoText_HasSave if noSavePanel is active, but can clear it
                if (slotVisuals.infoText_HasSave != null) slotVisuals.infoText_HasSave.text = "";
            }
        }
        Debug.Log("Finished updating save slot visuals.");
    }


    // Called by UI button to start a new game
    public void OnNewGameButtonPressed(int slotNumber)
    {
        Debug.Log($"New Game button pressed for slot: {slotNumber}");
        if (SaveSystem.instance == null) return;

        // Set the current slot and reset its data (deletes file and clears in-memory data)
        SaveSystem.instance.SetSlot(slotNumber);
        SaveSystem.instance.ResetSlot(slotNumber); // This creates a new default SaveData in memory

        // PlayerPrefs.SetInt("SaveIndex", slotNumber); // ResetSlot also updates the CurrentSlot which is stored

        // Load the game scene. The LevelLoader in the scene will use the new default SaveSystem.instance.data
        SceneManager.LoadScene(gameSceneName);
    }

    // Called by UI button to continue a saved game (reloads the scene)
    public void OnContinueButtonPressed(int slotNumber)
    {
        Debug.Log($"Continue button pressed for slot: {slotNumber}");
        if (SaveSystem.instance == null) return;

        if (!SaveSystem.instance.HasSave(slotNumber))
        {
            Debug.LogWarning($"Slot {slotNumber} is empty. Cannot continue.");
            // Optionally show a message to the player
            return;
        }

        // Set the current slot and load its data into SaveSystem.instance.data
        SaveSystem.instance.SetSlot(slotNumber);
        SaveSystem.instance.Load(slotNumber); // This loads data INTO SaveSystem.instance.data

        // PlayerPrefs.SetInt("SaveIndex", slotNumber); // Load also updates the CurrentSlot which is stored

        // Load the game scene. The LevelLoader.Start will pick up SaveSystem.instance.data
        // and call ApplyLoadedDataToScene()
        SceneManager.LoadScene(gameSceneName);
    }


    // Called by UI button to delete a save file
    public void OnDeleteButtonPressed(int slotNumber)
    {
        Debug.Log($"Delete button pressed for slot: {slotNumber}");
        if (SaveSystem.instance == null) return;

        if (!SaveSystem.instance.HasSave(slotNumber))
        {
            Debug.Log($"No save file found for slot {slotNumber} to delete.");
            return;
        }

        // IMPORTANT: Implement a confirmation dialog before deleting!
        // For now, direct delete:
        SaveSystem.instance.ResetSlot(slotNumber); // Deletes file and resets in-memory data IF it was the current slot
        UpdateAllSlotsVisuals(); // Refresh UI after deletion
    }

    // --- ADDED/MODIFIED ---
    // Called by a button in the game scene (e.g., pause menu) to save the current game state
    public void SaveCurrentGameToSlot(int slotNumber)
    {
        Debug.Log($"Attempting to save current game to slot: {slotNumber}");
        if (SaveSystem.instance == null || SaveSystem.instance.data == null)
        {
            Debug.LogError("SaveSystem not available or data not loaded to save game.");
            return;
        }

        // Ensure SaveSystem is set to the correct slot before saving
        SaveSystem.instance.SetSlot(slotNumber);

        // Trigger the save logic in SaveSystem
        SaveSystem.instance.Save();

        Debug.Log($"Game saved to slot {slotNumber}.");

        // If the save slots UI is currently open, refresh it to show the update
        if (saveSlotsPanel != null && saveSlotsPanel.activeInHierarchy)
        {
            UpdateAllSlotsVisuals();
        }
    }

    // --- MODIFIED ---
    // Called by a button in the save menu to load data into the *currently active* scene
    // Use with caution, requires careful scene setup (e.g., enemies must already be present)
    public void LoadSlotDataIntoCurrentScene(int slotNumber)
    {
        Debug.Log($"Loading data from slot {slotNumber} via scene reload."); // Changed log message
        if (SaveSystem.instance == null)
        {
            Debug.LogError("SaveSystem not available to load data.");
            return;
        }

        if (!SaveSystem.instance.HasSave(slotNumber))
        {
            Debug.LogWarning($"Slot {slotNumber} is empty. Cannot load data.");
            return;
        }

        // 1. Set the current slot
        SaveSystem.instance.SetSlot(slotNumber);

        // 2. Load data from the file into SaveSystem.instance.data
        SaveSystem.instance.Load(slotNumber); // This populates SaveSystem.instance.data

        // 3. Load the game scene. The LevelLoader and PlayerSave in the new scene
        //    will automatically apply the loaded data upon their Start() calls.
        SceneManager.LoadScene(gameSceneName); // <--- THIS IS THE ADDITION

        // The following lines become redundant if SceneManager.LoadScene is called,
        // as the current scene objects will be destroyed and new ones instantiated.
        // If you intend to keep a distinct "load into current scene" functionality,
        // then you'd remove the SceneManager.LoadScene call and keep these.
        /*
        PlayerSave player = FindObjectOfType<PlayerSave>();
        if (player != null)
        {
            player.ApplyLoadedData();
            Debug.Log("Applied player data from load.");
        }
        else Debug.LogError("PlayerSave object not found in current scene to apply loaded data.");

        LevelLoader levelLoader = FindObjectOfType<LevelLoader>();
        if (levelLoader != null)
        {
            levelLoader.ApplyLoadedDataToScene();
            Debug.Log("Applied LevelLoader data from load.");
        }
        else Debug.LogWarning("LevelLoader not found in current scene. Cannot update scene state (coins/enemies) from loaded data.");
        */

        // 4. Refresh the UI (only if the menu persists after scene load, which is unlikely)
        // This part is typically not needed if you're loading a new scene, as the menu itself might be destroyed.
        // If (saveSlotsPanel != null && saveSlotsPanel.activeInHierarchy)
        // {
        //     UpdateAllSlotsVisuals();
        // }

        // Debug.Log($"Data from slot {slotNumber} loaded, new scene '{gameSceneName}' will be loaded. UI refreshed.");
    }
}