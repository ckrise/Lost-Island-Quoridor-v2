using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private float scale = 1.25f;
    private RectTransform rectTransform;
    public float normalWidth = 200;
    public float normalHeight = 25;
    private float HoverWidth { get => normalWidth * scale; }
    private float HoverHeight { get => normalHeight * scale; }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, HoverWidth);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, HoverHeight);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, normalWidth);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, normalHeight);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, normalWidth);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, normalHeight);
    }
}