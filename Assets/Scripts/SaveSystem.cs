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
            DontDestroyOnLoad(gameObject); // Persist across scenes
            filePath = Application.persistentDataPath; // Where save files are stored

            // Load the last used slot, or default to slot 1 if none set
            CurrentSlot = PlayerPrefs.GetInt("SaveIndex", 1);
            Load(CurrentSlot); // Load data for this slot into 'data'
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate SaveSystem
        }
    }

    public void SetSlot(int slot)
    {
        CurrentSlot = slot;
        PlayerPrefs.SetInt("SaveIndex", slot); // Remember this slot for next game launch
        
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
                    data = new SaveData(); // Initialize with default values
                }
                else
                {
                    if (data.player == null) data.player = new PlayerData();
                    if (data.player.stats == null) data.player.stats = new PlayerStats();
                    if (data.coinPositions == null) data.coinPositions = new List<SerializableVector3>();
                    if (data.audioValues == null) data.audioValues = new SimpleAudioValues();
                    if (data.gameProgress == null) data.gameProgress = new GameProgress();
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
                
                if (tempData != null && tempData.player == null) tempData.player = new PlayerData(); 
                return tempData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to peek/parse save file for slot {slot}: {e.Message}");
                return new SaveData(); 
            }
        }
        return new SaveData(); 
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

        // If we are resetting the currently loaded slot, also clear the in-memory 'data'
        if (slot == CurrentSlot)
        {
            data = new SaveData(); // Create a fresh SaveData object
            Debug.Log($"In-memory data reset for current slot {CurrentSlot} after deletion.");
        }
    }
}