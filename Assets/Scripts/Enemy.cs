// Enemy.cs
using UnityEngine;
using System.Linq; // Needed for .Contains()
using System.Collections.Generic; // Make sure this is present for List

public class Enemy : MonoBehaviour
{
    public string enemyID; // IMPORTANT: Must be unique per savable enemy

    private PlayerSave playerSave;

    void Start()
    {
        playerSave = FindObjectOfType<PlayerSave>();

        // If a valid enemyID exists and SaveSystem is ready, check if this enemy was already defeated
        if (SaveSystem.instance != null && SaveSystem.instance.data != null && !string.IsNullOrEmpty(enemyID))
        {
            // Ensure gameProgress and its defeatedEnemyIDs list are initialized for loading
            if (SaveSystem.instance.data.gameProgress == null)
            {
                SaveSystem.instance.data.gameProgress = new GameProgress();
            }
            if (SaveSystem.instance.data.gameProgress.defeatedEnemyIDs == null)
            {
                SaveSystem.instance.data.gameProgress.defeatedEnemyIDs = new List<string>();
            }

            // Check against the correct list: gameProgress.defeatedEnemyIDs
            if (SaveSystem.instance.data.gameProgress.defeatedEnemyIDs.Contains(this.enemyID))
            {
                Debug.Log($"Enemy with ID '{enemyID}' was already defeated in save data. Destroying now.");
                Destroy(gameObject); // Destroy self
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Early exit if enemyID is not set or collision is not with player/projectile
        if (string.IsNullOrEmpty(enemyID))
        {
            Debug.LogWarning($"Enemy at {transform.position} has no ID! It won't be tracked across saves. Destroying on hit.");
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerProjectile"))
            {
                Destroy(gameObject);
            }
            return;
        }

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("PlayerProjectile"))
        {
            DestroyEnemy();
        }
    }

    public void DestroyEnemy()
    {
        // Only proceed if the enemy has a valid ID and SaveSystem is available
        if (!string.IsNullOrEmpty(enemyID) && SaveSystem.instance != null && SaveSystem.instance.data != null)
        {
            // Ensure gameProgress and its defeatedEnemyIDs list are initialized
            if (SaveSystem.instance.data.gameProgress == null)
                SaveSystem.instance.data.gameProgress = new GameProgress();
            if (SaveSystem.instance.data.gameProgress.defeatedEnemyIDs == null)
                SaveSystem.instance.data.gameProgress.defeatedEnemyIDs = new List<string>();

            // Add the ID to the correct list if it's not already there
            if (!SaveSystem.instance.data.gameProgress.defeatedEnemyIDs.Contains(this.enemyID))
            {
                SaveSystem.instance.data.gameProgress.defeatedEnemyIDs.Add(this.enemyID); // <--- THIS IS THE KEY CHANGE!
                Debug.Log($"Enemy with ID '{enemyID}' added to DEFEATED ENEMIES list for saving.");

                // Increment general enemies destroyed count
                SaveSystem.instance.data.gameProgress.enemiesDestroyedCount++;
                Debug.Log("General enemies destroyed count: " + SaveSystem.instance.data.gameProgress.enemiesDestroyedCount);

                // Give player a level
                if (playerSave == null) playerSave = FindObjectOfType<PlayerSave>();
                if (playerSave != null && playerSave.stats != null)
                {
                    playerSave.stats.lvl++;
                    Debug.Log($"Player leveled up! New level: {playerSave.stats.lvl}");
                }
                else
                {
                    Debug.LogWarning("PlayerSave component or player stats not found. Cannot level up player.");
                }
            }
        }
        else
        {
            // This branch handles cases where enemyID is invalid or SaveSystem isn't ready.
            Debug.LogWarning("Enemy destroyed without a valid ID or SaveSystem not ready. State not saved.");
        }

        // --- Destroy the enemy GameObject ---
        Destroy(gameObject);
    }
}