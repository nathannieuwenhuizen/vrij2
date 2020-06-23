using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    idle,
    alert,
    escape,
    cutscene
}

public class BaseManager : MonoBehaviour
{

    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private GameObject pauseButton;
    [SerializeField]
    protected FadeImage fadeImage;
    [SerializeField]

    [HideInInspector]
    public float musicVolume = .3f;
    protected float musicPauseVolume = .1f;

    private SceneLoader sceneLoader;

    public static BaseManager instance;

    public PostProcessingHandeler PPHandeler;

    [HideInInspector]
    public Human[] humans;


    protected GameState state = GameState.idle;

    [SerializeField]
    protected GnomeInfo[] gnomeDatas;

    [SerializeField]
    protected List<Human> alertedHumans;

    private bool fadingToScene = false;

    public void Pause(bool val)
    {
        AudioManager.instance?.PlaySound(AudioEffect.button_click, .4f);
        AudioManager.instance?.ChangeMusicVolume(val ? musicPauseVolume : musicVolume); // change music volume

        fadeImage.alpha = val ? .5f : 0f;
        Time.timeScale = val ? 0 : 1f;
        pauseButton.SetActive(!val);
        pauseScreen.SetActive(val);

    }

    public void ToggleGnomeMovement(bool val)
    {
        foreach(GnomeInfo gnomeInfo in gnomeDatas)
        {
            gnomeInfo.gnome.enabled = val;
        }
    }

    public virtual void Start()
    {
        Debug.Log("Start");
        humans = FindObjectsOfType<Human>(); // one time search
        //StartCoroutine(fadeImage.FadeTo(1f, 0f, .5f));

        alertedHumans = new List<Human>();

        sceneLoader = GetComponent<SceneLoader>();

        StartCoroutine(fadeImage.FadeTo(1, 0, 1f));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(true);
        }
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

    public virtual GameState State
    {
        get { return state; }
        set
        {
            state = value;
        }
    }

    public virtual void FirstMovement()
    {
    }
    public virtual void TrenchCoatWear()
    {
    }
    public virtual void FirstJump()
    {
    }

    public virtual void UpdateScoreUI()
    {
    }
    public virtual void Alarm()
    {
    }

    public virtual void CrownIsStolen(Gnome gnome)
    {
    }
    public virtual void End()
    {
        AudioManager.instance?.PlaySound(AudioEffect.congrats, 1f);
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



    public IEnumerator FadeToScene(string sceneName = "MainMenu")
    {
        if (!fadingToScene)
        {
            fadingToScene = true;

            yield return StartCoroutine(fadeImage.FadeTo(0, 1, 1f));
            if (sceneLoader == null)
            {
                sceneLoader = GetComponent<SceneLoader>();
            }
            sceneLoader.LoadNewScene(sceneName);
        }
    }

    [System.Serializable]
    public class GnomeInfo
    {
        public Gnome gnome;
    }
}
