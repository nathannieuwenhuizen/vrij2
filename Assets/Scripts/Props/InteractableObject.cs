using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{

    private AnimationCurve showCurve = AnimationCurve.EaseInOut(0,0,1,1);
    private AnimationCurve hideCurve = AnimationCurve.EaseInOut(0,0,1,1);

    private float endScale = .4f;

    private float interactionRange = 1f;

    private float showTime = .2f;

    protected bool popupIsActive = true;
    private bool isVisible = false;

    [SerializeField]
    private string popUpText = "Wear";

    [SerializeField]
    private bool isHigherUp = false;

    private Sprite leftButton;
    private Sprite rightButton;


    private GameObject popup;

    private void Awake()
    {
        endScale = endScale / transform.localScale.x;

        popup = Instantiate(Resources.Load("UI/popup") as GameObject, transform);

        leftButton = Resources.Load<Sprite>("UI/lb_button_icon") as Sprite;
        rightButton = Resources.Load<Sprite>("UI/rb_button_icon") as Sprite;

        popup.transform.localPosition = Vector3.zero;
    }
    private void Start()
    {
        scale = 0;
    }
    void Update()
    {
        if (popupIsActive)
        {
            Quaternion rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
            popup.transform.rotation = rotation;
            //popup.transform.LookAt(Camera.main.transform);
        }
    }
    public bool CanBeInteracted(Gnome other)
    {
        if (isHigherUp)
        {
            return popupIsActive && other.playerBelowMe != null;
        } else
        {
            return popupIsActive;
        }
    }

    public bool PopupIsActive
    {
        get { return popupIsActive; }
    }
    public void ShowUI(int controllerIndex)
    {
        if (isVisible) return;
        isVisible = true;

        AudioManager.instance?.PlaySound(AudioEffect.popup_show, 1f);
        popup.transform.Find("button").GetComponent<SpriteRenderer>().sprite = controllerIndex == 2 ? rightButton : leftButton;
        popup.GetComponentInChildren<TextMesh>().text = popUpText;
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
    protected float scale
    {
        get {
            return popup.transform.localScale.x;
        }
        set
        {
            popup.transform.localScale = new Vector3(value, value, value);
        }
    }

    public void HideUI()
    {
        if (!isVisible) return;
        isVisible = false;

        StartCoroutine(Animate(hideCurve, endScale, 0));

    }

    public virtual void Interact(Gnome gnome = null)
    {
        HideUI();
        popupIsActive = false;
    }
}
