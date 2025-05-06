using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; // Required for file operations
using System;

public class SaveUtility : MonoBehaviour
{

    public PlayerController playerController;
    private string saveFileName = "MySave.json";
    private string saveFilePath;


    private void Awake()
    {
        
    }
    public void Save()
    {

        string json = JsonUtility.ToJson(mPlayer.playerInfo);


        Debug.Log(json);


    }





}
