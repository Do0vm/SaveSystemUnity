
using System;
using System.Collections.Generic;


[Serializable]
public class SimpleAudioValues
{
    public float audioVolume = 1.0f; 
    public SimpleAudioValues()
    {
        audioVolume = 1.0f;
    }
}

[System.Serializable]
public class PlayerStats
{
    public int vigor, endurance, force, dexterity, intelligence, charisma;
    public int lvl, xp;
    public int money;
    public bool[] defeatedBosses = new bool[2]; // Consider initializing size if fixed

    public PlayerStats()
    {
        // Set default values if needed
        money = 0;
        lvl = 1;
        // Initialize defeatedBosses if not done by array declaration
        if (defeatedBosses == null || defeatedBosses.Length != 2)
        {
            defeatedBosses = new bool[2];
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public string psuedo = "Player"; // Default name
    public PlayerStats stats = new PlayerStats(); // Initialize
    public SerializableVector3 position; // Will be set from player's transform

    public PlayerData()
    {
        if (stats == null) stats = new PlayerStats();
        // Default position if necessary, though usually set on save
        position = SerializableVector3.FromVector3(UnityEngine.Vector3.zero);
    }
}

[System.Serializable]
public class GameProgress // For enemy count and other general progress
{
    public int enemiesDestroyedCount = 0;
    // Add other general game progress flags/data here
}

[System.Serializable]
public class SaveData
{
    public PlayerData player = new PlayerData(); // Initialize
    public List<SerializableVector3> coinPositions = new List<SerializableVector3>(); // Initialize
    public SimpleAudioValues audioValues = new SimpleAudioValues(); // Initialize
    public GameProgress gameProgress = new GameProgress(); // Initialize for enemy count

    public SaveData()
    {
        // Ensure all members are initialized to avoid nulls
        if (player == null) player = new PlayerData();
        if (coinPositions == null) coinPositions = new List<SerializableVector3>();
        if (audioValues == null) audioValues = new SimpleAudioValues();
        if (gameProgress == null) gameProgress = new GameProgress();
    }
}