using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static float Music
    {
        set
        {
            PlayerPrefs.SetFloat("Volume", value);
        }
        get
        {
            return PlayerPrefs.GetFloat("Volume", 1);
        }
    }

    public static float SFX
    {
        set
        {
            PlayerPrefs.SetFloat("SFX", value);
        }
        get
        {
            return PlayerPrefs.GetFloat("SFX", 1);
        }
    }

    public static int LevelProgression
    {
        set
        {
            PlayerPrefs.SetInt("levelprogress", value);
        }
        get
        {
            return PlayerPrefs.GetInt("levelprogress", 1);
        }
    }

    public static int HighscoreLvl1
    {
        set
        {
            PlayerPrefs.SetInt("highscorelvl1", value);
        }
        get
        {
            return PlayerPrefs.GetInt("highscorelvl1", 0);
        }
    }
    public static int HighscoreLvl2
    {
        set
        {
            PlayerPrefs.SetInt("highscorelvl2", value);
        }
        get
        {
            return PlayerPrefs.GetInt("highscorelvl2", 0);
        }
    }
    public static int HighscoreLvl3
    {
        set
        {
            PlayerPrefs.SetInt("highscorelvl3", value);
        }
        get
        {
            return PlayerPrefs.GetInt("highscorelvl3", 0);
        }
    }
    public static int HighscoreLvl4
    {
        set
        {
            PlayerPrefs.SetInt("highscorelvl4", value);
        }
        get
        {
            return PlayerPrefs.GetInt("highscorelvl4", 0);
        }
    }



    public static bool Vibration
    {
        set
        {
            PlayerPrefs.SetInt("Vibration", value == true ? 1 : 0);
            if (value)
            {
                //Handheld.Vibrate();
            }

        }
        get
        {
            return PlayerPrefs.GetInt("Vibration", 1) == 1;
        }
    }

}
