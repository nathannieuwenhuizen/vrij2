using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class OffScreenPointer : MonoBehaviour
{
    private Vector3 screenPos;
    private Vector2 onScreenPos;
    private float max;
    private Camera camera;

    private Image img;
    private RectTransform rc;

    public Transform target;

    private float precentage = 0.1f;

    public Image icon;
    private RectTransform iconRC;

    void Start()
    {
        img = GetComponent<Image>();
        rc = GetComponent<RectTransform>();
        camera = Camera.main;

        iconRC = icon.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (target == null)
        {
            Hide();
            return;
        }

        screenPos = camera.WorldToViewportPoint(target.position); //get viewport positions

        if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1)
        {
            Hide();
            return;
        }

        img.enabled = true;
        icon.enabled = true;

        onScreenPos = new Vector2(screenPos.x - 0.5f, screenPos.y - 0.5f) * 2; //2D version, new mapping
        max = Mathf.Max(Mathf.Abs(onScreenPos.x), Mathf.Abs(onScreenPos.y)); //get largest offset
        onScreenPos = (onScreenPos / (max * 2)) + new Vector2(0.5f, 0.5f); //undo mapping
        //Debug.Log(onScreenPos);
        //Debug.Log(screenPos);

        Vector2 delta = new Vector2(screenPos.x, screenPos.y) - onScreenPos;
        //Debug.Log(delta);
        float angle = 0;
        if (delta.y < 0)
        {
            angle = 360 - Vector2.Angle(new Vector2(1, 0), delta);
        } else
        {
            angle =  Vector2.Angle(new Vector2(1, 0), delta);
        }
        //Debug.Log(angle);

        onScreenPos.x *= camera.pixelWidth;
        onScreenPos.y *= camera.pixelHeight;

        onScreenPos.x = Cap(onScreenPos.x, rc.rect.width * precentage, camera.pixelWidth - rc.rect.width * precentage);
        onScreenPos.y = Cap(onScreenPos.y, rc.rect.height * precentage, camera.pixelHeight - rc.rect.height * precentage);


        rc.SetPositionAndRotation(onScreenPos, Quaternion.identity);
        rc.rotation = Quaternion.Euler(0, 0, angle);
        iconRC.localRotation = Quaternion.Euler(0, 0, -angle);
    }

    public float Cap(float val, float min, float max)
    {
        return Mathf.Max(Mathf.Min(max, val), min);
    }

    public void Hide()
    {
        icon.enabled = false;
        img.enabled = false;
    }
}
