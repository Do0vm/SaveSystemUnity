using System.IO;
using UnityEngine;
using System; // For Exception
using System.Collections.Generic; // For List in SaveData initialization

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;
    public SaveData data; // This is the currently active game data
    public int CurrentSlot { get; private set; }

    private string filePath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // --- MODIFIED SECTION FOR FILE PATH ---
            string baseSavePath;
            if (Application.isEditor)
            {
                baseSavePath = Directory.GetParent(Application.dataPath).FullName;
            }
            else // This is a build
            {
                baseSavePath = Directory.GetParent(Application.dataPath).FullName;
                if (Application.platform == RuntimePlatform.OSXPlayer)
                {
                    baseSavePath = Directory.GetParent(baseSavePath).FullName;
                }
            }

            filePath = Path.Combine(baseSavePath, "Saves");

            if (!Directory.Exists(filePath))
            {
                try
                {
                    Directory.CreateDirectory(filePath);
                    Debug.Log($"Save directory created at: {filePath}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to create save directory at {filePath}: {e.Message}. Defaulting to persistentDataPath.");
                    filePath = Application.persistentDataPath;
                    if (!Directory.Exists(filePath))
                    {
                        try { Directory.CreateDirectory(filePath); }
                        catch (Exception ex) { Debug.LogError($"Failed to create fallback save directory at {filePath}: {ex.Message}"); }
                    }
                }
            }
            Debug.Log($"Save files will be stored in: {filePath}");
            // --- END OF MODIFIED SECTION ---

            CurrentSlot = PlayerPrefs.GetInt("SaveIndex", 1);
            Load(CurrentSlot); // Load data for this slot into 'data'
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSlot(int slot)
    {
        CurrentSlot = slot;
        PlayerPrefs.SetInt("SaveIndex", slot);
    }

    public void Save()
    {
        var playerSaveComponent = FindObjectOfType<PlayerSave>();
        if (playerSaveComponent != null)
        {
            data.player = playerSaveComponent.ToData();
        }
        else Debug.LogWarning("PlayerSave component not found. Player data not updated for saving.");

        if (LevelLoader.instance != null)
        {
            data.coinPositions.Clear();
            foreach (var coin in LevelLoader.instance.mCoins)
            {
                if (coin != null)
                {
                    data.coinPositions.Add(SerializableVector3.FromVector3(coin.transform.position));
                }
            }
        }
        else Debug.LogWarning("LevelLoader instance not found. Coin data not saved.");

        // gameProgress (including defeatedEnemyIDs) is part of 'data' and should be saved automatically.
        // Ensure 'data.gameProgress' is up-to-date from Enemy script interactions.

        string json = JsonUtility.ToJson(data, true);
        string fullPath = Path.Combine(filePath, $"game{CurrentSlot}.save");

        try
        {
            File.WriteAllText(fullPath, json);
            Debug.Log($"Game saved to: {fullPath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data to {fullPath}: {e.Message}");
        }
    }

    public void Load(int slot)
    {
        CurrentSlot = slot;

        string fullPath = Path.Combine(filePath, $"game{slot}.save");
        if (File.Exists(fullPath))
        {
            try
            {
                string json = File.ReadAllText(fullPath);
                data = JsonUtility.FromJson<SaveData>(json);

                if (data == null)
                {
                    Debug.LogWarning($"Save file for slot {slot} was empty or corrupted. Initializing new SaveData.");
                    data = new SaveData(); // Initialize with default values including GameProgress
                }
                else
                {
                    // Defensive initialization for potentially missing nested objects from older saves
                    if (data.player == null) data.player = new PlayerData();
                    if (data.player.stats == null) data.player.stats = new PlayerStats();
                    if (data.coinPositions == null) data.coinPositions = new List<SerializableVector3>();
                    if (data.audioValues == null) data.audioValues = new SimpleAudioValues();

                    // Specifically ensure gameProgress and its list are initialized
                    if (data.gameProgress == null)
                    {
                        data.gameProgress = new GameProgress();
                    }
                    else if (data.gameProgress.defeatedEnemyIDs == null) // If gameProgress exists but list doesn't (older save)
                    {
                        data.gameProgress.defeatedEnemyIDs = new List<string>();
                    }
                }
                Debug.Log($"Game data loaded from slot {slot}. Player position: {data.player.position.ToVector3()}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load or parse save file for slot {slot}: {e.Message}. Initializing new SaveData.");
                data = new SaveData(); // Fallback to new data on error
            }
        }
        else
        {
            Debug.Log($"No save file found for slot {slot}. Initializing new SaveData.");
            data = new SaveData(); // If no file, create fresh data in memory
        }
    }


    public SaveData PeekSlotData(int slot)
    {
        string fullPath = Path.Combine(filePath, $"game{slot}.save");
        if (File.Exists(fullPath))
        {
            try
            {
                string json = File.ReadAllText(fullPath);
                SaveData tempData = JsonUtility.FromJson<SaveData>(json);

                if (tempData != null)
                {
                    if (tempData.player == null) tempData.player = new PlayerData();
                    // Ensure gameProgress and its list are initialized for peeking too
                    if (tempData.gameProgress == null)
                    {
                        tempData.gameProgress = new GameProgress();
                    }
                    else if (tempData.gameProgress.defeatedEnemyIDs == null)
                    {
                        tempData.gameProgress.defeatedEnemyIDs = new List<string>();
                    }
                }
                else // File exists but is empty/corrupt
                {
                    tempData = new SaveData(); // Return fresh data for display
                }
                return tempData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to peek/parse save file for slot {slot}: {e.Message}");
                return new SaveData();
            }
        }
        return new SaveData(); // Return fresh data if no file
    }

    public bool HasSave(int slot)
    {
        return File.Exists(Path.Combine(filePath, $"game{slot}.save"));
    }

    public void ResetSlot(int slot) // Deletes the save file and resets in-memory 'data' if it's the CurrentSlot
    {
        string path = Path.Combine(filePath, $"game{slot}.save");
        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
                Debug.Log($"Save file for slot {slot} deleted.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error deleting save file for slot {slot}: {e.Message}");
            }
        }

        if (slot == CurrentSlot)
        {
            data = new SaveData(); // Create a fresh SaveData object
            Debug.Log($"In-memory data reset for current slot {CurrentSlot} after deletion.");
        }
    }
}