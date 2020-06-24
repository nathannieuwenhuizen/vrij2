using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalanceUI : MonoBehaviour
{
    [Header("Sprite elements")]
    [SerializeField]
    private GameObject pointer_pivot;
    [SerializeField]
    private SpriteRenderer pointer;
    private Color pointerColor;

    private float val = 0;
    private float velocity = 0f;
    private float acceleration = 0f;


    private float endScale = 1f;
    private float showTime = .2f;
    private bool isVisible = false;
    private AnimationCurve showCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private AnimationCurve hideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public float Val
    {
        get { return val; }
        set {
            val = value;
            UpdatePointerPivot();
        }
    }

    void Start()
    {
        scale = 0;
    }

    public void UpdatePointerPivot()
    {
        pointer_pivot.transform.localRotation = Quaternion.Euler( new Vector3(0, 0, Val * 90));
        pointerColor = Color.Lerp(Color.green, Color.red, Mathf.Abs(Val));
        pointer.color = pointerColor;
    }

    void Update()
    {
        Quaternion rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
        transform.rotation = rotation;
    }

    public bool InBalance(float input)
    {
        velocity -= input * acceleration;
        velocity += acceleration * Val;
        Val += velocity * 0.1f;

        if (Mathf.Abs(Val) > 1f)
        {
            return false;
        }
        //make it harder
        acceleration += 0.0001f; 
        return true;
    }

    public void ShowUI()
    {
        if (isVisible) return;
        isVisible = true;

        velocity = 0f;
        acceleration = .005f;
        Val = 0.1f;
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

}
