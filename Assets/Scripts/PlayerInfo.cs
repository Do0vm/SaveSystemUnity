using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;



[Serializable]
public class PlayerInfo : MonoBehaviour
{
    public string psuedo = "Patrick";
    public PlayerStats stats;
    public SerializableVector3 position = SerializableVector3.FromVector3(new Vector3(0,1,0));  




}

[Serializable]
public class PlayerStats
{
    public int vigor, endurance, force, dexterity, intelligence, charisma;
    public int lvl, xp;
    public int money;
    public bool[] defeatedBosses = new bool[2];

    public PlayerStats() { /* defaults if needed */ }
}


[Serializable]
public class SimpleAudioValues
{
    public float audioVolume = 1.0f;
}



[Serializable]
public class PlayerData
{
    public string psuedo;
    public PlayerStats stats;
    public SerializableVector3 position;
}

[Serializable]
public class SaveData
{
    public PlayerData player;
    public List<SerializableVector3> coinPositions = new List<SerializableVector3>();
    public SimpleAudioValues audioValues = new SimpleAudioValues();
}


