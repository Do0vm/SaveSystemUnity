// Make sure these are [System.Serializable]

// PlayerStats.cs (or part of PlayerData.cs)
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public int vigor;
    public int endurance;
    public int force;
    public int dexterity;
    public int intelligence;
    public int charisma;
    public int lvl; // This will be incremented
    public int xp;
    public int money;
    public bool[] defeatedBosses = new bool[2]; // Default or from save

    // Constructor to initialize default values if needed
    public PlayerStats()
    {
        lvl = 1; // Example starting level
        // Initialize other stats as needed
        defeatedBosses = new bool[2]; // Ensure array is initialized
    }
}
[System.Serializable] // IMPORTANT
public class GameProgress
{
    public int enemiesDestroyedCount; // This is correctly saving

    // THIS IS LIKELY THE ISSUE:
    // Make sure this line exists, is PUBLIC, and is part of the GameProgress class
    public List<string> defeatedEnemyIDs = new List<string>();

    // Constructor to ensure the list is always initialized
    public GameProgress()
    {
        defeatedEnemyIDs = new List<string>();
    }
}
// PlayerData.cs (or part of SaveData.cs)
[System.Serializable]
public class PlayerData
{
    public string psuedo = "Patrick";
    public SerializableVector3 position;
    public PlayerStats stats = new PlayerStats(); // Automatically initializes with PlayerStats constructor

    public PlayerData()
    {
        stats = new PlayerStats(); // Ensure stats is never null
    }
}


//// If not already defined, SerializableVector3 for saving Vector3
[System.Serializable] // IMPORTANT
public class SerializableVector3 // Make sure you have this for position
{
    public float x, y, z;
    // Constructor and ToVector3/FromVector3 methods
    public SerializableVector3(float rX, float rY, float rZ) { x = rX; y = rY; z = rZ; }
    public Vector3 ToVector3() { return new Vector3(x, y, z); }
    public static SerializableVector3 FromVector3(Vector3 v3) { return new SerializableVector3(v3.x, v3.y, v3.z); }
}

// If not already defined, SimpleAudioValues for saving audio settings
[System.Serializable]
public class SimpleAudioValues
{
    public float masterVolume = 1f;
    public float musicVolume = 0.8f;
    public float sfxVolume = 0.8f;
    // Add any other audio settings you need
}


// SaveData.cs (ensure it includes GameProgress)
[System.Serializable]
public class SaveData
{
    public PlayerData player = new PlayerData();
    public List<SerializableVector3> coinPositions = new List<SerializableVector3>();
    public SimpleAudioValues audioValues = new SimpleAudioValues();
    public GameProgress gameProgress = new GameProgress(); // Make sure this is initialized

    //public List<string> destroyedEnemyIDs = new List<string>();


    public SaveData() // Constructor for new save data
    {
        player = new PlayerData();
        coinPositions = new List<SerializableVector3>();
        audioValues = new SimpleAudioValues();
        //destroyedEnemyIDs = new List<string>();
        gameProgress = new GameProgress(); // Ensures gameProgress and its defeatedEnemyIDs list are initialized
    }
}