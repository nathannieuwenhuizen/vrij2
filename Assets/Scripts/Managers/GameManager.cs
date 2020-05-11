using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        AudioManager.instance?.Playmusic(Music.museum, .5f * Settings.Music);
    }

}
