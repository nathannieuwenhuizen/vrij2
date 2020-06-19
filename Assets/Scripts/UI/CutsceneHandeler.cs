using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneHandeler : MonoBehaviour
{
    [SerializeField]
    private float letterPause = 0.02f;

    [SerializeField]
    private Transform cameraFocus;

    [SerializeField]
    private Text dialogueText;
    [SerializeField]
    private Text fromText;
    [SerializeField]
    private Image nextButton_Controller;
    [SerializeField]
    private Image nextButton_Keyboard;

    [SerializeField]
    private RectTransform blackBarUp;
    [SerializeField]
    private RectTransform blackBarDown;

    private Vector3 oldPos;
    private Quaternion oldRot;

    private AnimationCurve cameraCurve = AnimationCurve.EaseInOut(0,0,1,1);

    private bool skip = false;

    public bool inCutscene = false;

    // Use this for initialization
    void Start()
    {
        BlackBarHeight = 0;
        HideText();
    }
    public void Update()
    {
        if (NextButtonDown())
        {
            skip = true;
        }
    }
    public void StartCutscene(SpokenLine[] dialogue, Transform cameraPos = null, bool flipCamera = false)
    {
        HideText();
        StartCoroutine(InDialogue(dialogue, cameraPos, flipCamera));
    }

    public IEnumerator AnimateBlackBars(bool show)
    {
        float desiredHeight = show ? 70f : 0;
        while (Mathf.Abs(BlackBarHeight - desiredHeight) > 1f)
        {
            BlackBarHeight = Mathf.Lerp(BlackBarHeight, desiredHeight, Time.deltaTime * 4f);
            yield return new WaitForFixedUpdate();
        }
        BlackBarHeight = desiredHeight;
    }

    public float BlackBarHeight
    {
        get { return blackBarDown.sizeDelta.y; }
        set
        {
            blackBarDown.sizeDelta = new Vector2(blackBarDown.sizeDelta.x, value);
            blackBarUp.sizeDelta = new Vector2(blackBarUp.sizeDelta.x, value);
        }
    }

    public IEnumerator MoveCamera(Vector3 pos, Quaternion rot)
    {
        Vector3 startPos = Camera.main.transform.position;
        Quaternion startRot = Camera.main.transform.rotation;
        float index = 0;
        while (index < 1f)
        {
            index += Time.deltaTime * 0.5f;
            Camera.main.transform.position = Vector3.Lerp(startPos, pos, cameraCurve.Evaluate(index));
            Camera.main.transform.rotation = Quaternion.Slerp(startRot, rot, cameraCurve.Evaluate(index));
            yield return new WaitForFixedUpdate();
        }
        Camera.main.transform.position = pos;
        Camera.main.transform.rotation = rot;
    }

    public void EnableCameraFollowTargets(bool val)
    {
        if (Camera.main.GetComponent<MultipleTargetsAverageFollow>() != null)
        {
            Camera.main.GetComponent<MultipleTargetsAverageFollow>().enabled = val;
        }
    }

    public IEnumerator InDialogue(SpokenLine[] dialogue, Transform cameraPos, bool flipCamera = false)
    {
        inCutscene = true;
        EnableCameraFollowTargets(false);

        StartCoroutine(AnimateBlackBars(true));

        oldPos = Camera.main.transform.position;
        oldRot = Camera.main.transform.rotation;

        if (flipCamera)
        {
            MultipleTargetsAverageFollow mf = Camera.main.GetComponent<MultipleTargetsAverageFollow>();
            Debug.Log("z offset: " + mf.offset.z);
            float delta = mf.offset.z * 2f;
            oldPos.z -= delta;
            mf.offset.z *= -1;

            oldRot = Quaternion.Euler(54f, 180f, oldRot.z);
        }

        if (cameraPos != null)
        {
            yield return StartCoroutine(MoveCamera(cameraPos.position, cameraPos.rotation));
        }

        bool nextLine = false;
        foreach (SpokenLine line in dialogue)
        {
            fromText.text = line.Name;
            nextLine = false;
            yield return StartCoroutine(TypeText(line.Line));

            if (Data.ControllerConnected())
            {
                nextButton_Controller.gameObject.SetActive(true);
                nextButton_Keyboard.gameObject.SetActive(false);
            }
            else
            {
                nextButton_Controller.gameObject.SetActive(false);
                nextButton_Keyboard.gameObject.SetActive(true);
            }

            while (nextLine == false)
            {
                if(NextButtonDown())
                {
                    yield return new WaitForEndOfFrame();
                    skip = false;
                    nextLine = true;
                }
                yield return new WaitForFixedUpdate();
            }
            HideText();
        }
        StartCoroutine(AnimateBlackBars(false));
        yield return StartCoroutine(MoveCamera(oldPos, oldRot));
        EnableCameraFollowTargets(true);


        inCutscene = false;
    }
    public bool NextButtonDown()
    {
        return Input.GetButtonDown("Interact_P1") || Input.GetButtonDown("Interact_P2") || Input.GetMouseButtonDown(0);
    }

    public bool NextButtonUp()
    {
        return Input.GetButtonUp("Interact_P1") || Input.GetButtonUp("Interact_P2") || Input.GetMouseButtonUp(0);
    }

    public void HideText()
    {
        nextButton_Controller.gameObject.SetActive(false);
        nextButton_Keyboard.gameObject.SetActive(false);
        fromText.text = "";
        dialogueText.text = "";
    }

    IEnumerator TypeText(string line)
    {
        skip = false;
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            if (!skip)
            {
                yield return new WaitForFixedUpdate();
                //yield return new WaitForSeconds(letterPause);
            }
        }
    }
}

public struct SpokenLine
{
    public string Line;
    public string Name;

    public SpokenLine(string name, string line)
    {
        this.Line = line;
        this.Name = name;
    }
}