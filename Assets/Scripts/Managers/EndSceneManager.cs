using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSceneManager : BaseManager
{

    public CutsceneHandeler cutSceneHandeler;


    [SerializeField]
    private Transform endShot;


    public override void Start()
    {
        base.Start();

        instance = this;
        musicVolume = .1f;
        musicPauseVolume = .05f;
        AudioManager.instance?.Playmusic(Music.park, musicVolume);

        StartCoroutine(EndCutscene());

    }

    public IEnumerator EndCutscene()
    {
        State = GameState.cutscene;
        cutSceneHandeler.StartCutscene(Data.endDialogue, endShot);
        while (cutSceneHandeler.inCutscene)
        {
            yield return new WaitForFixedUpdate();
        }
        End();
        State = GameState.idle;
    }
    public override void End()
    {
        AudioManager.instance?.PlaySound(AudioEffect.congrats, 1f);
        StartCoroutine(FadeToScene("Credits"));
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
                    ToggleGnomeMovement(false);
                    PPHandeler.ChangeChromaticDistribution(0);
                    break;
                case GameState.cutscene:
                    ToggleGnomeMovement(false);
                    break;
                default:
                    break;
            }
        }
    }
}
