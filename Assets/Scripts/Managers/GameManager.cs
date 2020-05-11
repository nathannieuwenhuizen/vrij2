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
    void Start()
    {
        AudioManager.instance?.Playmusic(Music.museum, .5f * Settings.Music);
    }


    public void Pause(bool val)
    {
        //AudioManager.instance?.PlaySound(AudioEffect) //TODO: button click
        AudioManager.instance?.CHangeMusicVolume(val ? .2f : 1f); // change music volume

        fadeImage.alpha = val ? .5f : 0f;
        Time.timeScale = val ? 0 : 1f;
        pauseButton.SetActive(!val);
        pauseScreen.SetActive(val);
    }



}
