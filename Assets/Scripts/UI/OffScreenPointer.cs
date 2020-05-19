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

    void Start()
    {
        img = GetComponent<Image>();
        rc = GetComponent<RectTransform>();
        camera = Camera.main;
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

        onScreenPos = new Vector2(screenPos.x - 0.5f, screenPos.y - 0.5f) * 2; //2D version, new mapping
        max = Mathf.Max(Mathf.Abs(onScreenPos.x), Mathf.Abs(onScreenPos.y)); //get largest offset
        onScreenPos = (onScreenPos / (max * 2)) + new Vector2(0.5f, 0.5f); //undo mapping
        Debug.Log(onScreenPos);
        Debug.Log(screenPos);

        Vector2 delta = new Vector2(screenPos.x, screenPos.y) - onScreenPos;
        Debug.Log(delta);
        float angle = 0;
        if (delta.y < 0)
        {
            angle = 360 - Vector2.Angle(new Vector2(1, 0), delta);
        } else
        {
            angle =  Vector2.Angle(new Vector2(1, 0), delta);
        }
        Debug.Log(angle);

        onScreenPos.x *= camera.pixelWidth;
        onScreenPos.y *= camera.pixelHeight;

        onScreenPos.x = Cap(onScreenPos.x, rc.rect.width / 4f, camera.pixelWidth - rc.rect.width / 4f);
        onScreenPos.y = Cap(onScreenPos.y, rc.rect.height / 4f, camera.pixelHeight - rc.rect.height / 4f);


        rc.SetPositionAndRotation(onScreenPos, Quaternion.identity);
        rc.rotation = Quaternion.Euler(0, 0, angle);

    }

    public float Cap(float val, float min, float max)
    {
        return Mathf.Max(Mathf.Min(max, val), min);
    }

    public void Hide()
    {
        img.enabled = false;
    }
}
