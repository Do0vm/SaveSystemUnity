using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CoinPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerSave>();
        if (player != null)
        {
            // Increase player money (or score)
            player.stats.money += 1;

            // Remove this coin from LevelLoader’s list so SaveSystem won’t respawn it
            LevelLoader.instance.mCoins.Remove(gameObject);

            // Destroy the coin
            Destroy(gameObject);

            // Optionally, immediately save
            SaveSystem.instance.Save();
        }
    }
}
