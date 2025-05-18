using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; 

public class AudioMenu : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioMixer mixer;

    private SimpleAudioValues audioValues; 

    private void Start()
    {
        if (SaveSystem.instance != null && SaveSystem.instance.data != null)
        {
            audioValues = SaveSystem.instance.data.audioValues ?? new SimpleAudioValues();

            if (SaveSystem.instance.data.audioValues == null)
            {
                SaveSystem.instance.data.audioValues = audioValues;
            }
        }
        else
        {
            Debug.LogWarning("AudioMenu: SaveSystem.instance or SaveSystem.instance.data is null in Start. Using default audio values.");
            audioValues = new SimpleAudioValues();
        }

        if (volumeSlider != null) 
        {
            volumeSlider.value = audioValues.audioVolume;
            volumeSlider.onValueChanged.RemoveAllListeners(); // Clear previous listeners
            volumeSlider.onValueChanged.AddListener(v => {
                if (this.audioValues != null) // Check if audioValues is initialized
                {
                    this.audioValues.audioVolume = v;
                }
                ApplyVolume(v);
            });
        }
        else
        {
            Debug.LogError("AudioMenu: VolumeSlider is not assigned in the Inspector!");
        }

        if (mixer != null) // Good practice to check if mixer is assigned
        {
            ApplyVolume(audioValues.audioVolume);
        }
        else
        {
            Debug.LogError("AudioMenu: AudioMixer is not assigned in the Inspector!");
        }
    } // End of Start() method

    void ApplyVolume(float v)
    {
        if (mixer != null)
        {
            
            if (v <= 0.0001f) 
            {
                mixer.SetFloat("MasterVolume", -80f); 
            }
            else
            {
                mixer.SetFloat("MasterVolume", Mathf.Log10(v) * 20f);
            }
        }
    }

    private void OnDisable()
    {
        if (SaveSystem.instance != null && SaveSystem.instance.data != null && audioValues != null)
        {
            SaveSystem.instance.data.audioValues = audioValues;
            
        }
        else
        {
            Debug.LogWarning("AudioMenu: SaveSystem instance/data or local audioValues is null in OnDisable. Audio settings may not be persisted in SaveSystem's data.");
        }
    }
} 