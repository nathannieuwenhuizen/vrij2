using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{

    [SerializeField]
    private Slider volumeSlider;

    [SerializeField]
    private Slider sfxSlider;

    [SerializeField]
    private AudioSource uiClick;

    private float oldSFXVal;
    void Start()
    {
        volumeSlider.value = Settings.Music;
        sfxSlider.value = Settings.SFX;

        volumeSlider.onValueChanged.AddListener(delegate {
            Settings.Music = volumeSlider.value;
            MenuManager.instance.ChangemusicVOlume();
        });

        sfxSlider.onValueChanged.AddListener(delegate {
            if (Mathf.Abs(oldSFXVal - sfxSlider.value) > .1f)
            {
                oldSFXVal = sfxSlider.value;
                uiClick.Play();
            }
            uiClick.volume = sfxSlider.value;
            Settings.SFX = sfxSlider.value;
        });
    }

}
