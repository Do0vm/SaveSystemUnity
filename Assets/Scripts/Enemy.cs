// In your Enemy.cs script
using UnityEngine;

public class Enemy : MonoBehaviour
{

    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerProjectile"))
        {
            DestroyEnemy();
        }
    }

    public void DestroyEnemy() 
    {
        if (SaveSystem.instance != null && SaveSystem.instance.data != null)
        {
            // Ensure gameProgress is initialized
            if (SaveSystem.instance.data.gameProgress == null)
            {
                SaveSystem.instance.data.gameProgress = new GameProgress();
            }
            SaveSystem.instance.data.gameProgress.enemiesDestroyedCount++;
            Debug.Log("Enemy destroyed. Total count: " + SaveSystem.instance.data.gameProgress.enemiesDestroyedCount);
        }
        else
        {
            Debug.LogWarning("SaveSystem not found or data not loaded. Enemy destruction not counted.");
        }

        Destroy(gameObject);

       
    }
}