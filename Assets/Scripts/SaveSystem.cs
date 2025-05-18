using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;
    public SaveData data;
    public int CurrentSlot { get; private set; }

    private string filePath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            filePath = Application.persistentDataPath;
            CurrentSlot = PlayerPrefs.GetInt("SaveIndex", 1);
            Load(CurrentSlot);
        }
        else Destroy(gameObject);
    }

    public void SetSlot(int slot)
    {
        CurrentSlot = slot;
        PlayerPrefs.SetInt("SaveIndex", slot);
    }

    public void Save()
    {
        // Pull player data
        var playerSave = FindObjectOfType<PlayerSave>();
        data.player = playerSave.ToData();

        // Pull coin positions
        data.coinPositions.Clear();
        foreach (var coin in LevelLoader.instance.mCoins)
            data.coinPositions.Add(
                SerializableVector3.FromVector3(coin.transform.position)
            );

        // Write JSON
        string json = JsonUtility.ToJson(data);
        string full = Path.Combine(filePath, $"game{CurrentSlot}.save");
        if (!File.Exists(full)) File.Create(full).Dispose();
        File.WriteAllText(full, json);
    }

    public void Load(int slot)
    {
        CurrentSlot = slot;
        string full = Path.Combine(filePath, $"game{slot}.save");
        if (File.Exists(full))
        {
            string json = File.ReadAllText(full);
            data = JsonUtility.FromJson<SaveData>(json);
        }
        else data = new SaveData();
    }

    public bool HasSave(int slot)
        => File.Exists(Path.Combine(filePath, $"game{slot}.save"));

    public void ResetSlot(int slot)
    {
        var path = Path.Combine(filePath, $"game{slot}.save");
        if (File.Exists(path)) File.Delete(path);
        if (slot == CurrentSlot)
            data = new SaveData();
    }
}
