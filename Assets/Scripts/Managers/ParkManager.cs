using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkManager : BaseManager
{

    public CutsceneHandeler cutSceneHandeler;


    [SerializeField]
    private Transform introShot;

    [SerializeField]
    private OffScreenPointer pointer;

    [SerializeField]
    private TutorialUI tutorialUI;

    public override void Start()
    {
        base.Start();

        instance = this;
        musicVolume = .1f;
        musicPauseVolume = .05f;
        AudioManager.instance?.Playmusic(Music.park, musicVolume);

        StartCoroutine(IntroCutscene());

    }

    public IEnumerator IntroCutscene()
    {
        State = GameState.cutscene;
        yield return new WaitForSeconds(1f);
        cutSceneHandeler.StartCutscene(Data.introDialogue, introShot);
        while(cutSceneHandeler.inCutscene)
        {
            yield return new WaitForFixedUpdate();
        }
        State = GameState.idle;
        tutorialUI.ShowMovement();
    }
    public override void End()
    {
        AudioManager.instance?.PlaySound(AudioEffect.congrats, 1f);
        StartCoroutine(FadeToScene("Museum"));
    }

    public override void FirstMovement()
    {
        if (State == GameState.cutscene) { return; }

        tutorialUI.FadeMovement();
    }
    public override void TrenchCoatWear()
    {
        tutorialUI.ShowJump();
    }
    public override void FirstJump()
    {
        if (State == GameState.cutscene) { return; }
        tutorialUI.FadeJump();
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
                    ToggleGnomeMovement(true);
                    pointer.gameObject.SetActive(true);
                    PPHandeler.ChangeChromaticDistribution(0);
                    break;
                case GameState.alert:
                    PPHandeler.ChangeChromaticDistribution(0.2f);
                    break;
                case GameState.escape:
                    break;
                case GameState.cutscene:
                    ToggleGnomeMovement(false);
                    pointer.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }

}
