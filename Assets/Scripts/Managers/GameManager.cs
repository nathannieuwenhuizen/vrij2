using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : BaseManager
{

    [SerializeField]
    private PlayerScore playerScore;


    public override void Start()
    {
        base.Start();
        instance = this;

        AudioManager.instance?.Playmusic(Music.museum, musicVolume);
    }


    public override GameState State
    {
        get { return state; }
        set
        {
            state = value;
            switch (state)
            {
                case GameState.idle:
                    AudioManager.instance?.FadeMusic(Music.museum, 1f);
                    PPHandeler.ChangeChromaticDistribution(0);
                    break;
                case GameState.alert:
                    AudioManager.instance?.FadeMusic(Music.guardSeesYou, 1f);
                    PPHandeler.ChangeChromaticDistribution(0.2f);
                    break;
                case GameState.escape:
                    PPHandeler.ChangeChromaticDistribution(1f);
                    break;
                case GameState.cutscene:
                    break;
                default:
                    break;
            }
        }
    }


    public override void Alarm()
    {
        StartCoroutine(fadeImage.AlarmLoop(5));
    }

    public override void CrownIsStolen()
    {
        AudioManager.instance?.PlaySound(AudioEffect.congrats, .3f);
        StartCoroutine(FadeToScene("MainMenu"));
    }

    public override void UpdateScoreUI ()
    {
        playerScore.UpdateScore(gnomeDatas[0].gnome.StolenArtWork.Count, gnomeDatas[1].gnome.StolenArtWork.Count);
    }

}


