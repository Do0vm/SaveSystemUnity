// LevelLoader.cs
using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Needed for .Contains()

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;
    public GameObject coinPrefab;
    public List<GameObject> mCoins = new List<GameObject>(); // List of active coin GameObjects

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Found duplicate LevelLoader, destroying this one.");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        Debug.Log("LevelLoader Start. Applying loaded data...");
        ApplyLoadedDataToScene();
    }

    public void ApplyLoadedDataToScene()
    {
        Debug.Log("Applying loaded data to scene...");
        SpawnCoinsFromSave();
        ApplyLoadedEnemyState(); // Apply enemy state data
    }

    public void SpawnCoinsFromSave()
    {
        Debug.Log("Applying coin data from save...");
        GameObject[] existingCoins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (var c in existingCoins)
        {
            Destroy(c);
        }
        mCoins.Clear();

        if (SaveSystem.instance != null && SaveSystem.instance.data != null && SaveSystem.instance.data.coinPositions != null)
        {
            var positions = SaveSystem.instance.data.coinPositions;
            if (positions.Count > 0)
            {
                if (coinPrefab == null)
                {
                    Debug.LogError("Coin Prefab is not assigned in LevelLoader! Cannot spawn coins.");
                    return;
                }

                foreach (var pos in positions)
                {
                    var coin = Instantiate(coinPrefab, pos.ToVector3(), Quaternion.identity);
                    if (!coin.CompareTag("Coin")) coin.tag = "Coin";
                    mCoins.Add(coin);
                }
                Debug.Log($"Spawned {mCoins.Count} coins from save data.");
            }
            else
            {
                Debug.Log("No coin positions found in save data. No coins spawned from save.");
            }
        }
        else
        {
            Debug.Log("SaveSystem instance, data, or coinPositions list is null. Cannot spawn coins from save.");
        }
    }

    /// <summary>
    /// Finds enemies in the scene and destroys those marked as killed in the save data.
    /// </summary>
    public void ApplyLoadedEnemyState()
    {
        Debug.Log("Applying enemy state from save...");
        // Ensure gameProgress and its defeatedEnemyIDs list exist in save data
        if (SaveSystem.instance == null ||
            SaveSystem.instance.data == null ||
            SaveSystem.instance.data.gameProgress == null || // Check gameProgress
            SaveSystem.instance.data.gameProgress.defeatedEnemyIDs == null) // Check the correct list
        {
            Debug.LogWarning("SaveSystem data, GameProgress, or defeated enemy IDs list is null. Cannot apply saved enemy state.");
            return;
        }

        // Load from the correct list!
        List<string> defeatedIDs = SaveSystem.instance.data.gameProgress.defeatedEnemyIDs; // <--- THIS IS THE KEY CHANGE!

        Enemy[] enemiesInScene = FindObjectsOfType<Enemy>();
        Debug.Log($"Found {enemiesInScene.Length} enemies in the scene.");

        int destroyedCount = 0;
        foreach (Enemy enemy in enemiesInScene)
        {
            if (!string.IsNullOrEmpty(enemy.enemyID) && defeatedIDs.Contains(enemy.enemyID))
            {
                // Debug.Log($"Enemy with ID '{enemy.enemyID}' found in scene and marked as defeated in save. Destroying now.");
                Destroy(enemy.gameObject); // Destroy the GameObject
                destroyedCount++;
            }
            else if (string.IsNullOrEmpty(enemy.enemyID))
            {
                Debug.LogWarning($"Enemy found in scene ({enemy.gameObject.name}) without an 'enemyID' set. Cannot track its state across saves.");
            }
        }

        Debug.Log($"Applied saved enemy state: {destroyedCount} enemies destroyed based on save data.");
    }
}