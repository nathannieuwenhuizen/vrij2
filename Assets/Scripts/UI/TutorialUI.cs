using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField]
    private Sprite movement_Controller;
    [SerializeField]
    private Sprite movement_Keyboard;
    [SerializeField]
    private Sprite jump_Controller;
    [SerializeField]
    private Sprite jump_Keyboard;
    [SerializeField]
    private Image sprite_jump;
    [SerializeField]
    private Image sprite_movement;

    private bool jumpShown = false;
    private bool movementShown = false;

    private bool jumpHidden = false;
    private bool movementHidden = false;

    public void ShowMovement()
    {
        if (movementShown || movementHidden) return;
        movementShown = true;

        Hide();
        sprite_movement.sprite = Data.ControllerConnected() ? movement_Controller : movement_Keyboard;

        StopAllCoroutines();
        StartCoroutine(FadingTo(0f, 1f, 1f, sprite_movement));
    }
    public void ShowJump()
    {
        if (jumpShown || jumpHidden) return;
        jumpShown = true;

        Hide();
        sprite_jump.sprite = Data.ControllerConnected() ? jump_Controller : jump_Keyboard;

        StopAllCoroutines();
        StartCoroutine(FadingTo(0f, 1f, 1f, sprite_jump));
    }
    public void FadeJump()
    {
        if (jumpHidden || !jumpShown) return;
        jumpHidden = true;

        StopAllCoroutines();
        StartCoroutine(FadingTo(1f, 0f, 1f, sprite_jump));
    }
    public void FadeMovement()
    {
        if (movementHidden || !movementShown) return;
        movementHidden = true;

        StopAllCoroutines();
        StartCoroutine(FadingTo(1f, 0f, 1f, sprite_movement));
    }

    public void Hide()
    {
        Color col = sprite_movement.color;
        col.a = 0;
        sprite_movement.color = sprite_jump.color = col;
    }


    public IEnumerator FadingTo(float _startAlpha, float _endAlpha, float _duration, Image renderer)
    {
        Color color = renderer.color;

        float startAlpha = _startAlpha;
        float index = 0;

        while (index < _duration)
        {
            index += Time.deltaTime;
            yield return new WaitForFixedUpdate();


            color.a = Mathf.Lerp(startAlpha, _endAlpha, index);
            renderer.color = color;
        }
        color.a = _endAlpha;
        renderer.color = color;
    }
}
 