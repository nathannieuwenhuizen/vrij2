using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : BaseManager
{

    [SerializeField]
    private PlayerScore playerScore;

    public CutsceneHandeler cutSceneHandeler;
    [SerializeField]
    private Transform crownStolenShot;

    [SerializeField]
    private Transform endObject;
    [SerializeField]
    private OffScreenPointer pointer;
    [SerializeField]
    private Sprite homeSprite;
     
    public override void Start()
    {
        base.Start();
        instance = this;

        //AudioManager.instance?.Playmusic(Music.museum, musicVolume);
        State = GameState.idle;
    }

    public IEnumerator CrownStolenCutscene(Gnome gnome)
    {
        endObject.gameObject.SetActive(true);
        pointer.target = endObject;
        pointer.icon.sprite = homeSprite;

        State = GameState.cutscene;
        StartCoroutine(fadeImage.AlarmLoop(1000));
        yield return new WaitForSeconds(1f);
        cutSceneHandeler.StartCutscene(Data.alarmGoesOff, crownStolenShot, true);
        while (cutSceneHandeler.inCutscene)
        {
            yield return new WaitForFixedUpdate();
        }

        //aleart evry human to always chase the gnome
        foreach (Human human in humans)
        {
            human.foundGnome = gnome;
            human.stateMachine.cState.OnStateSwitch(human.alwaysChaseState);
        }

        State = GameState.escape;
    }

    public override void End()
    {
        AudioManager.instance?.PlaySound(AudioEffect.congrats, 1f);
        DontDestroyOnLoad(gnomeDatas[0].gnome.gameObject);
        DontDestroyOnLoad(gnomeDatas[1].gnome.gameObject);
        DontDestroyOnLoad(gnomeDatas[0].gnome.pulledObject.gameObject);
        DontDestroyOnLoad(gnomeDatas[1].gnome.pulledObject.gameObject);
        StartCoroutine(FadeToScene("ResultScreen"));
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
                    //AudioManager.instance?.FadeMusic(Music.museum, 1f);
                    AudioManager.instance?.ChangeMusicLayers(new float[] { 0, 1, 0, 0, 1, 1, 0 });

                    PPHandeler.ChangeChromaticDistribution(0);
                    break;
                case GameState.alert:
                    //AudioManager.instance?.FadeMusic(Music.guardSeesYou, 1f);
                    AudioManager.instance?.ChangeMusicLayers(new float[] { 1, 1, 1, 0, 1, 1, 0 });

                    PPHandeler.ChangeChromaticDistribution(0.2f);
                    break;
                case GameState.escape:
                    AudioManager.instance?.ChangeMusicLayers(new float[] { 1, 1, 1, 1, 1, 1, 1 });
                    //AudioManager.instance?.FadeMusic(Music.guardSeesYou, 1f);
                    PPHandeler.ChangeChromaticDistribution(.5f);
                    break;
                case GameState.cutscene:
                    AudioManager.instance?.ChangeMusicLayers(new float[] { 0, 0, 0, 0, 1, 0, 0 });
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

    public override void CrownIsStolen(Gnome gnome)
    {
        StartCoroutine(CrownStolenCutscene(gnome));
    }

    public override void UpdateScoreUI ()
    {
        playerScore.UpdateScore(gnomeDatas[0].gnome.StolenArtWork.Count, gnomeDatas[1].gnome.StolenArtWork.Count);
    }

}


