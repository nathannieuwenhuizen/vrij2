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

    [HideInInspector]
    public Human[] humans;

    public float musicVolume = .3f;
    public float musicPauseVolume = .1f;

    [SerializeField]
    private GnomeInfo[] gnomeDatas;

    private SceneLoader sceneLoader;

    public static GameManager instance;
    void Start()
    {
        instance = this;

        sceneLoader = GetComponent<SceneLoader>();

        humans = FindObjectsOfType<Human>(); // one time search
        AudioManager.instance?.Playmusic(Music.museum, musicVolume);

        GameManager.instance.UpdateScoreUI();

        
         StartCoroutine(fadeImage.FadeTo(1f, 0f, .5f));
    }


    public void Pause(bool val)
    {
        AudioManager.instance?.PlaySound(AudioEffect.button_click, .4f);
        AudioManager.instance?.ChangeMusicVolume(val ? musicPauseVolume : musicVolume); // change music volume

        fadeImage.alpha = val ? .5f : 0f;
        Time.timeScale = val ? 0 : 1f;
        pauseButton.SetActive(!val);
        pauseScreen.SetActive(val);
    }

    public void Alarm()
    {
        StartCoroutine(fadeImage.AlarmLoop(5));
    }

    public void UpdateScoreUI ()
    {
        foreach(GnomeInfo gnomeData in gnomeDatas)
        {
            gnomeData.UpdateScore();
        }
    }

    public void CrownIsStolen()
    {
        AudioManager.instance?.PlaySound(AudioEffect.congrats, .3f);
        StartCoroutine(Ending());
    }
    public IEnumerator Ending()
    {
        yield return StartCoroutine(fadeImage.FadeTo(0, 1, 1f));
        sceneLoader.LoadNewScene("MainMenu");
    }
}


[System.Serializable]
public class GnomeInfo
{
    public Gnome gnome;
    private int score = 0;
    public Text scoreText;
    public void UpdateScore()
    {
        if (gnome.StolenArtWork != null)
        {
            score = gnome.StolenArtWork.Count;
        }
        scoreText.text = score + "";
    }
}