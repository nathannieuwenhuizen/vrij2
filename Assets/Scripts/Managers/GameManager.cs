using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum GameState
{
    idle,
    alert,
    escape,
    cutscene
}
public class GameManager : MonoBehaviour
{


    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private GameObject pauseButton;
    [SerializeField]
    private FadeImage fadeImage;
    [SerializeField]
    private PostProcessingHandeler PPHandeler;

    [SerializeField]
    private PlayerScore playerScore;

    [HideInInspector]
    public Human[] humans;

    public float musicVolume = .3f;
    public float musicPauseVolume = .1f;

    [SerializeField]
    private GnomeInfo[] gnomeDatas;

    private SceneLoader sceneLoader;

    private List<Human> alertedHumans;
    private GameState state = GameState.idle;

    public static GameManager instance;
    void Start()
    {
        instance = this;

        sceneLoader = GetComponent<SceneLoader>();

        humans = FindObjectsOfType<Human>(); // one time search
        AudioManager.instance?.Playmusic(Music.museum, musicVolume);

       
        StartCoroutine(fadeImage.FadeTo(1f, 0f, .5f));

        alertedHumans = new List<Human>();
        Data.ControllerConnected();
    }


    public GameState State
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
        playerScore.UpdateScore(gnomeDatas[0].gnome.StolenArtWork.Count, gnomeDatas[1].gnome.StolenArtWork.Count);
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

    public void HumanIsAlerted(Human human)
    {
        if (alertedHumans.Contains(human)) return;

        alertedHumans.Add(human);
        if (alertedHumans.Count == 1 && state != GameState.escape)
        {
            State = GameState.alert;
        }
    }

    public void HumanIsNormal(Human human)
    {
        if (!alertedHumans.Contains(human)) return;

        alertedHumans.Remove(human);
        Debug.Log("gnome count: " + alertedHumans.Count);
        if (alertedHumans.Count == 0 && state != GameState.escape)
        {
            State = GameState.idle;
        }
    }


}


[System.Serializable]
public class GnomeInfo 
{
    public Gnome gnome;
}