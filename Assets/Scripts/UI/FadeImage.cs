using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeImage : MonoBehaviour
{

    private Image img;
    private Color color;
    void Start()
    {
        img = GetComponent<Image>();
    }

    public float alpha
    {
        get
        {
            return img.color.a;
        }
        set
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, value);
        }
    }

    public IEnumerator AlarmLoop(int amountOfLoops = 5)
    {
        StopAllCoroutines();
        float index = 0;
        float maxAlpha = .1f;
        float loopDuration = .5f;
        img.color = new Color(1,0,0,0);
        while (index < amountOfLoops * Mathf.PI * 2)
        {
            index +=  Time.deltaTime / ( loopDuration / Mathf.PI);
            yield return new WaitForFixedUpdate();
            alpha = Mathf.Sin(index) * maxAlpha;
        }
    }

    public IEnumerator FadeTo(float _startAlpha, float _endAlpha, float _duration)
    {
        color = Color.black;
        StopAllCoroutines();

        float startAlpha = _startAlpha;
        float index = 0;

        while (index < _duration)
        {
            index += Time.deltaTime;
            yield return new WaitForFixedUpdate();
            color.a = Mathf.Lerp(startAlpha, _endAlpha, index);
            img.color = color;
        }
        color.a = _endAlpha;
        img.color = color;
    }

}
