using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private GameObject pauseButton;
    [SerializeField]
    private FadeImage fadeImage;

    public float musicVolume = .3f;
    public float musicPauseVolume = .1f;


    public static GameManager instance;
    void Start()
    {
        instance = this;

        AudioManager.instance?.Playmusic(Music.museum, musicVolume);
    }


    public void Pause(bool val)
    {
        AudioManager.instance?.PlaySound(AudioEffect.button_click, .4f); //TODO: button click
        AudioManager.instance?.CHangeMusicVolume(val ? musicPauseVolume : musicVolume); // change music volume

        fadeImage.alpha = val ? .5f : 0f;
        Time.timeScale = val ? 0 : 1f;
        pauseButton.SetActive(!val);
        pauseScreen.SetActive(val);
    }



}
