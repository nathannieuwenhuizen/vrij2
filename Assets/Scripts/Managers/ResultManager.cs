using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [SerializeField]
    private Gnome[] gnomes;

    [SerializeField]
    private ScoreData spawnDataPlayer1;
    [SerializeField]
    private ScoreData spawnDataPlayer2;

    [SerializeField]
    private Text totalText;

    [SerializeField]
    private GameObject nextButton;

    private AnimationCurve showCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    void Start()
    {
        gnomes = FindObjectsOfType<Gnome>();

        AudioManager.instance?.Playmusic(Music.park, .5f);

        nextButton.SetActive(false);
        SetUpGnome(gnomes[0], spawnDataPlayer1);
        SetUpGnome(gnomes[1], spawnDataPlayer2);

        StartCoroutine(ShowResult());

    }

    void SetUpGnome(Gnome gnome, ScoreData spawnData)
    {
        gnome.transform.position = spawnData.gnomeSpawn.position;
        spawnData.scoreText.text = "";
        foreach(ArtWork artwork in gnome.StolenArtWork)
        {
            Debug.Log("artwork: " + artwork);
            artwork.transform.parent = null;
            artwork.transform.localScale = Vector3.one * .5f;
            artwork.transform.position = spawnData.artworkSpawn.position;
            artwork.gameObject.AddComponent<Rigidbody>();
            artwork.gameObject.SetActive(false);
        }
    }

    IEnumerator ShowResult()
    {
        yield return StartCoroutine(ScoreForOnePlayer(gnomes[0], spawnDataPlayer1));
        yield return StartCoroutine(ScoreForOnePlayer(gnomes[1], spawnDataPlayer2));
        yield return StartCoroutine(TotalScore(0, spawnDataPlayer1.score + spawnDataPlayer2.score));
        AudioManager.instance?.PlaySound(AudioEffect.congrats, .5f);

        nextButton.SetActive(true);
    }
    public void GoToEndScene()
    {
        Debug.Log("Go to end scene...");
        foreach(Gnome gnome in gnomes)
        {
            foreach(ArtWork art in gnome.StolenArtWork)
            {
                Destroy(art.gameObject);
            }
            Destroy(gnome.pulledObject);
            Destroy(gnome.gameObject);
        }
        GetComponent<SceneLoader>().LoadNewScene("EndScene");
    }

    IEnumerator ScoreForOnePlayer(Gnome gnome, ScoreData spawnData)
    {
        foreach (ArtWork artwork in gnome.StolenArtWork)
        {
            AudioManager.instance?.PlaySound(AudioEffect.statue_steal, .1f);

            artwork.gameObject.SetActive(true);
            spawnData.score++;

            spawnData.scoreText.text = spawnData.score + "";
            yield return new WaitForSeconds(2f / Mathf.Max(1, gnome.StolenArtWork.Count));
        }
    }
    IEnumerator TotalScore(float begin, float end)
    {
        int totalScore = 0;
        float index = 0;
        float showTime = 2f;
        AudioManager.instance?.PlaySound(AudioEffect.painting_steal, .2f);
        while (index < showTime)
        {

            index += Time.deltaTime;
            totalScore = Mathf.RoundToInt(Mathf.Lerp(begin, end, showCurve.Evaluate(index / showTime)));
            totalText.text = totalScore + "";
            yield return new WaitForFixedUpdate();
        }
        totalScore = Mathf.RoundToInt( end);
        totalText.text = totalScore + "";
    }
}

[System.Serializable]
public class ScoreData
{
    public int score = 0;
    public Transform gnomeSpawn;
    public Transform artworkSpawn;
    public Text scoreText;

}
