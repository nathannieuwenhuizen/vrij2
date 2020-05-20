using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public static MenuManager instance;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        AudioManager.instance?.Playmusic(Music.menu, .6f);
    }

    public void ButtonClick()
    {
        AudioManager.instance?.PlaySound(AudioEffect.button_click, 1f);
    }

    public void ChangemusicVOlume()
    {
        AudioManager.instance?.ChangeMusicVolume(.6f);
    }

}
