using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private bool hover = false;

    private RectTransform rt;
    private Vector3 min = Vector3.one;
    private Vector3 max = Vector3.one * 1.1f;
    private float sizeSpeed = 5f;
    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }
    private void Update()
    {
        rt.localScale = Vector3.Lerp(rt.localScale, hover ? max : min , Time.deltaTime * sizeSpeed);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        hover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hover = false;
    }
}