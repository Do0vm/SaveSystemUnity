using UnityEngine;

public class PlayerSave : MonoBehaviour
{
    [Header("Player Identity & Stats")]
    public string psuedo = "Patrick";
    public PlayerStats stats = new PlayerStats();

    private void Start()
    {
        // initialize from loaded data
        var data = SaveSystem.instance.data.player;
        psuedo = data.psuedo;
        stats = data.stats;
        transform.position = data.position.ToVector3();
    }

    // Call this before SaveSystem.Save()
    public PlayerData ToData()
    {
        return new PlayerData
        {
            psuedo = this.psuedo,
            stats = this.stats,
            position = SerializableVector3.FromVector3(transform.position)
        };
    }
}
