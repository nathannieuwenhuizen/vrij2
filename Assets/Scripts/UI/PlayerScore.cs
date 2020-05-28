using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    [SerializeField]
    private ScoreObject scorePlayer1;
    [SerializeField]
    private ScoreObject scorePlayer2;

    private AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public void UpdateScore(int score1, int score2)
    {
        StopAllCoroutines();

        scorePlayer1.scoreText.text = "" + score1;
        scorePlayer2.scoreText.text = "" + score2;

        float total = score1 + score2;

        float precentage1 = total > 0 ? (float)score1 / total : 1f;
        float precentage2 = total > 0 ? (float)score2 / total : 1f;
        
        StartCoroutine(UpdateUI(scorePlayer1, precentage1));
        StartCoroutine(UpdateUI(scorePlayer2, precentage2));
    }

    public void Start()
    {
        UpdateScore(0, 0);
    }

    private IEnumerator UpdateUI(ScoreObject scoreObj, float endVal)
    {
        float startVal = scoreObj.scaleX;
        float index = 0;

        while (index < 1)
        {
            index += Time.deltaTime;

            scoreObj.scaleX = Mathf.Lerp(startVal, endVal, curve.Evaluate(index));
            yield return new WaitForFixedUpdate();
        }
        scoreObj.scaleX = endVal;
    }

}

[System.Serializable]
public class ScoreObject
{
    public Image slider;
    public Text scoreText;
    public float scaleX
    {
        get
        {
            return slider.rectTransform.localScale.x;
        }
        set
        {
            Vector3 tempScale = slider.rectTransform.localScale;
            tempScale.x = value;
            slider.rectTransform.localScale = tempScale;
        }
    }
}
