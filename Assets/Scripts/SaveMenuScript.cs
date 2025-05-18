//using TMPro;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class SaveMenuScript : MonoBehaviour
//{
//    [Tooltip("1-based slot index")]
//    [SerializeField] private int saveNumber = 1;

//    [Header("UI Elements")]
//    [SerializeField] private GameObject hasSavePanel;  // contains Continue/Delete
//    [SerializeField] private GameObject noSavePanel;   // contains New Game only
//    [SerializeField] private TextMeshProUGUI scoreText;

//    [Tooltip("Name of your gameplay scene")]
//    [SerializeField] private string gameSceneName = "GameScene";

//    private void Start()
//    {
//        UpdateUI();
//    }

//    private void UpdateUI()
//    {
//        bool has = SaveSystem.instance.HasSave(saveNumber);
//        hasSavePanel.SetActive(has);
//        noSavePanel.SetActive(!has);

//        if (has)
//        {
//            SaveSystem.instance.Load(saveNumber);
//            scoreText.text = SaveSystem.instance.data.player.stats.money.ToString();
//        }
//    }

//    public void Continue()
//    {
//        PlayerPrefs.SetInt("SaveIndex", saveNumber);
//        SaveSystem.instance.Load(saveNumber);
//        SceneManager.LoadScene(gameSceneName);
//    }

//    public void NewGame()
//    {
//        PlayerPrefs.SetInt("SaveIndex", saveNumber);
//        SaveSystem.instance.Reset(saveNumber);
//        SceneManager.LoadScene(gameSceneName);
//    }

//    public void Delete()
//    {
//        PlayerPrefs.SetInt("SaveIndex", saveNumber);
//        SaveSystem.instance.Reset(saveNumber);
//        UpdateUI();
//    }
//}
