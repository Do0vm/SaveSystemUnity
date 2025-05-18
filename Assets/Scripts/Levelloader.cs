// In LevelLoader.cs
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;
    public GameObject coinPrefab;
    public List<GameObject> mCoins = new List<GameObject>();

    private void Awake() => instance = this;

    private void Start()
    {
        SpawnCoinsFromSave();
    }

    /// <summary>
    /// Clears any existing in-scene coins, then instantiates based on saved positions.
    /// </summary>
    public void SpawnCoinsFromSave()
    {
        // Remove any existing
        foreach (var c in mCoins)
            if (c != null) Destroy(c);
        mCoins.Clear();

        // Spawn from save data
        var positions = SaveSystem.instance.data.coinPositions;
        if (positions != null && positions.Count > 0)
        {
            foreach (var pos in positions)
            {
                var coin = Instantiate(coinPrefab, pos.ToVector3(), Quaternion.identity);
                mCoins.Add(coin);
            }
        }
        else
        {
        }
    }
}
