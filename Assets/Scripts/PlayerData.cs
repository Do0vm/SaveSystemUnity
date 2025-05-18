using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    [Header("Player Identity & Stats")]
    public string psuedo = "Patrick"; // Can be initialized from SaveData
    public PlayerStats stats = new PlayerStats(); // Can be initialized from SaveData

    private void Start()
    {
        ApplyLoadedData();
    }

    public void ApplyLoadedData()
    {
        if (SaveSystem.instance != null && SaveSystem.instance.data != null && SaveSystem.instance.data.player != null)
        {
            var loadedPlayerData = SaveSystem.instance.data.player;

            this.psuedo = loadedPlayerData.psuedo;

            // Deep copy for stats to avoid shared reference issues
            this.stats = new PlayerStats();
            this.stats.vigor = loadedPlayerData.stats.vigor;
            this.stats.endurance = loadedPlayerData.stats.endurance;
            this.stats.force = loadedPlayerData.stats.force;
            this.stats.dexterity = loadedPlayerData.stats.dexterity;
            this.stats.intelligence = loadedPlayerData.stats.intelligence;
            this.stats.charisma = loadedPlayerData.stats.charisma;
            this.stats.lvl = loadedPlayerData.stats.lvl;
            this.stats.xp = loadedPlayerData.stats.xp;
            this.stats.money = loadedPlayerData.stats.money;

            if (loadedPlayerData.stats.defeatedBosses != null)
            {
                this.stats.defeatedBosses = new bool[loadedPlayerData.stats.defeatedBosses.Length];
                System.Array.Copy(loadedPlayerData.stats.defeatedBosses, this.stats.defeatedBosses, loadedPlayerData.stats.defeatedBosses.Length);
            }
            else
            {
                this.stats.defeatedBosses = new bool[2]; // Default if null
            }

            // Apply position
            transform.position = loadedPlayerData.position.ToVector3();
            Debug.Log($"PlayerSave: Applied loaded data. Position set to {transform.position}");
        }
        else
        {
            Debug.LogWarning("PlayerSave: SaveSystem instance or data is not available. Using default values.");
            // Optionally initialize with defaults if no save data is found
            transform.position = new Vector3(0, 1, 0); // Default start position
        }
    }

    public PlayerData ToData()
    {
        PlayerData playerData = new PlayerData
        {
            psuedo = this.psuedo,
            position = SerializableVector3.FromVector3(transform.position),
            stats = new PlayerStats() // Create new stats object for saving (deep copy)
        };

        // Copy current stats to the new stats object
        playerData.stats.vigor = this.stats.vigor;
        playerData.stats.endurance = this.stats.endurance;
        playerData.stats.force = this.stats.force;
        playerData.stats.dexterity = this.stats.dexterity;
        playerData.stats.intelligence = this.stats.intelligence;
        playerData.stats.charisma = this.stats.charisma;
        playerData.stats.lvl = this.stats.lvl;
        playerData.stats.xp = this.stats.xp;
        playerData.stats.money = this.stats.money;

        if (this.stats.defeatedBosses != null)
        {
            playerData.stats.defeatedBosses = new bool[this.stats.defeatedBosses.Length];
            System.Array.Copy(this.stats.defeatedBosses, playerData.stats.defeatedBosses, this.stats.defeatedBosses.Length);
        }
        else
        {
            playerData.stats.defeatedBosses = new bool[2]; // Default if null
        }

        return playerData;
    }
}