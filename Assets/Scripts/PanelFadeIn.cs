using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelFadeIn : MonoBehaviour
{
    private Image image;
    private float maxAlpha;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        Color color = image.color;
        maxAlpha = color.a;
        color.a = 0;
        image.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        if (image.color.a < maxAlpha)
        {
            Color color = image.color;
            color.a += .05f;
            image.color = color;
        }
        else if (image.color.a > maxAlpha)
        {
            Color color = image.color;
            color.a = maxAlpha;
            image.color = color;
        }
        Debug.Log($"{name} alpha: {image.color.a}");
    }
}
