using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    // private Transform bar;
    [SerializeField]
    public Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        // bar = transform.Find("Bar");
    }

    public void SetSize(float sizeNormalized)
    {
        slider.value = sizeNormalized;
        // Debug.Log("SetSize: " + sizeNormalized);
        // sizeNormalized = Mathf.Clamp01(sizeNormalized);
        // sizeNormalized = Mathf.Round(sizeNormalized * 100) / 100;
        // bar.localScale = new Vector3(sizeNormalized, 1f);
        // SetColor(GetColorToSet());
    }

    public void SetColor(Color color)
    {
        // bar.Find("BarSprite").GetComponent<SpriteRenderer>().color = color;
    }

    // private Color GetColorToSet()
    // {
    //     if (bar.localScale.x >= 0.50f)
    //     {
    //         return Color.green;
    //     }
    //     else if (bar.localScale.x >= .25f)
    //     {
    //         return Color.yellow;
    //     }
    //     else
    //     {
    //         return Color.red;
    //     }
    // }
}
