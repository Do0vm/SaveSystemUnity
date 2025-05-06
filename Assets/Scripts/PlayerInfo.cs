using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable] //Json cant Vectors handle directly
public struct SerializableVector3
{
    public float x,y,z;

    public SerializableVector3(float rX, float rY, float rZ)

    {

        x = rX;
        y = rY;
        z = rZ;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x,y,z);
    }

    public static SerializableVector3 FromVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}


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


}

[Serializable]
public class SimpleAudioValues
{
    public float audioVolume = 1.0f;
}