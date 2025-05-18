//// SaveData.cs - This is an example structure based on your usage
//using UnityEngine;
//using System;
//using System.Collections.Generic;

//[System.Serializable]
//public class SerializableVector3
//{
//    public float x, y, z;
//    public static SerializableVector3 FromVector3(Vector3 v) => new SerializableVector3 { x = v.x, y = v.y, z = v.z };
//    public Vector3 ToVector3() => new Vector3(x, y, z);
//    public override string ToString() => $"({x}, {y}, {z})";
//}

//[System.Serializable]
//public class PlayerStats
//{
//    public int vigor = 1;
//    public int endurance = 1;
//    public int force = 1;
//    public int dexterity = 1;
//    public int intelligence = 1;
//    public int charisma = 1;
//    public int lvl = 1; // Player Level
//    public int xp = 0;
//    public int money = 0;
//    public bool[] defeatedBosses = new bool[2]; // Example for tracking bosses
//}

//[System.Serializable]
//public class PlayerData
//{
//    public string psuedo = "Patrick";
//    public SerializableVector3 position = SerializableVector3.FromVector3(new Vector3(0, 1, 0));
//    public PlayerStats stats = new PlayerStats();
//}

//[System.Serializable]
//public class SimpleAudioValues
//{
//    public float masterVolume = 1.0f;
//    public float musicVolume = 1.0f;
//    public float sfxVolume = 1.0f;
//}

//[System.Serializable]
//public class GameProgress
//{
//    public int enemiesDestroyedCount = 0; // Already exists
//    // Add other progress markers here later if needed
//}


//// --- ADD THIS SECTION ---
//[System.Serializable]
//public class SaveData
//{
//    public PlayerData player = new PlayerData();
//    public List<SerializableVector3> coinPositions = new List<SerializableVector3>();
//    public SimpleAudioValues audioValues = new SimpleAudioValues();
//    public GameProgress gameProgress = new GameProgress(); // Already exists
//    public DateTime saveTimestamp = DateTime.Now; // Optional: add timestamp


//    // --- NEW: List to store IDs of enemies that have been destroyed ---
//    public List<string> destroyedEnemyIDs = new List<string>();

//    // Constructor to ensure lists/objects are never null when creating new data
//    public SaveData()
//    {
//        player = new PlayerData();
//        coinPositions = new List<SerializableVector3>();
//        audioValues = new SimpleAudioValues();
//        gameProgress = new GameProgress();
//        destroyedEnemyIDs = new List<string>(); // Initialize the new list
//        saveTimestamp = DateTime.Now;
//    }
//}