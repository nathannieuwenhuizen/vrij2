using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Condition
{
    all,
    justOne
}
public class Door : MonoBehaviour
{
    private AnimationCurve anim = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private float showTime = .3f;

    [SerializeField]
    private Transform[] doorParts;
    [SerializeField]
    private Condition openCondition;

    [SerializeField]
    private FloorButton[] buttons;

    private bool open = false;
    [SerializeField]
    private bool prenamentOpen = false;
    [SerializeField]
    private bool reverse = false;


    public void Update()
    {
        if (IsActivated())
        {
            if (reverse) Close();
            else Open();
        } else
        {
            if (reverse)Open();
            else Close();
        }
    }

    private bool IsActivated()
    {
        if (buttons.Length == 0) return false;
        
        if (openCondition == Condition.justOne)
        {
            foreach(FloorButton button in buttons)
            {
                if (button.Pushed)
                {
                    return true;
                }
            }
        } else if (openCondition == Condition.all)
        {
            foreach (FloorButton button in buttons)
            {
                if (!button.Pushed)
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }

    public void Open()
    {
        if (open) return;

        open = true;
        StopAllCoroutines();
        StartCoroutine(Animate(1f, 0f));
    }

    public void Close()
    {
        if (!open || prenamentOpen) return;

        open = false;
        StopAllCoroutines();
        StartCoroutine(Animate(0f, 1f));
    }


    IEnumerator Animate(float beginScale, float endScale)
    {
        float index = 0;
        while (index < showTime)
        {
            index += Time.deltaTime;
            scale = Mathf.Lerp(beginScale, endScale, anim.Evaluate(index / showTime));
            yield return new WaitForFixedUpdate();
        }
        scale = endScale;
    }
    protected float scale
    {
        get
        {
            if (doorParts.Length > 0)
                return doorParts[0].transform.localScale.x;
            else
                return 0;
        }
        set
        {
            for(int i = 0; i < doorParts.Length; i++)
            {
                Vector3 temp = doorParts[i].localScale;
                doorParts[i].localScale = new Vector3(value, temp.y, temp.z);
            }
        }
    }
}
