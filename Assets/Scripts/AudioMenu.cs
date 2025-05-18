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
        // load or default
        audioValues = SaveSystem.instance.data.audioValues ?? new SimpleAudioValues();

        volumeSlider.value = audioValues.audioVolume;
        ApplyVolume(audioValues.audioVolume);

        volumeSlider.onValueChanged.AddListener(v => {
            audioValues.audioVolume = v;
            ApplyVolume(v);
        });
    }

    void ApplyVolume(float v)
    {
        mixer.SetFloat("MasterVolume", Mathf.Log10(v) * 20f);
    }

    private void OnDisable()
    {
        // persist back into SaveSystem
        SaveSystem.instance.data.audioValues = audioValues;
    }
}
