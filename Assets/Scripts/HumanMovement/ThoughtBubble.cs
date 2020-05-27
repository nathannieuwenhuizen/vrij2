using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum Thought
{
    noticing,
    alert
}
public class ThoughtBubble : MonoBehaviour
{

    private AnimationCurve showCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private AnimationCurve hideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float endScale = .1f;
    private float showTime = .2f;
    private bool isVisible = false;

    [SerializeField]
    private SpriteRenderer front;
    [SerializeField]
    private SpriteRenderer back;
    [SerializeField]
    private SpriteMask mask;

    [SerializeField]
    private Sprite questionSprite;
    [SerializeField]
    private Sprite alertSprite;

    private void Awake() 
    {
        endScale = endScale / transform.localScale.x;

        transform.localPosition = Vector3.zero;
    }
    private void Start()
    {
        scale = 0;
    }
    void Update()
    {
        Quaternion rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
        transform.rotation = rotation;
    }


    public void ShowUI()
    {
        if (isVisible) return;
        isVisible = true;

        StartCoroutine(Animate(showCurve, 0, endScale));
    }
    IEnumerator Animate(AnimationCurve curve, float beginScale, float endScale)
    {
        float index = 0;
        while (index < showTime)
        {
            index += Time.deltaTime;
            scale = Mathf.Lerp(beginScale, endScale, curve.Evaluate(index / showTime));
            yield return new WaitForFixedUpdate();
        }
        scale = endScale;
    }
    private float scale
    {
        get
        {
            return transform.localScale.x;
        }
        set
        {
            transform.localScale = new Vector3(value, value, value);
        }
    }

    public void HideUI()
    {
        if (!isVisible) return;
        isVisible = false;

        StartCoroutine(Animate(hideCurve, endScale, 0));
    }

    public Color FillColor
    {
        set
        {
            front.color = value;
        }
    }
    public Color BackColor
    {
        set
        {
            back.color = value;
        }
    }
    public float FillAmount
    {
        set {
            front.transform.localScale =new Vector3(back.transform.localScale.x, back.transform.localScale.y * value, back.transform.localScale.z);
            //Debug.Log(back.sprite.rect.height);
            front.transform.localPosition = back.transform.localPosition - new Vector3(0, 1.5f, 0 ) * (1- value);
        }
    }
    public void SetSymbol( Thought thought)
    {
        if (thought == Thought.alert)
        {
            mask.sprite = alertSprite;
        } else
        {
            mask.sprite = questionSprite;
        }
        FillAmount = 1;
    }
}
