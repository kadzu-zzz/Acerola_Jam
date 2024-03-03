using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TransformOverTime : MonoBehaviour
{
    public RectTransform start, end;
    RectTransform rect;

    public bool do_transform = false;
    public float delay = 2;
    float delay_max;
    public float speed = 1.8f;
    float t = 0.0f;

    public Image fadeout_display;

    public FadeIn trigger_on_complete;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        var element = GetComponent<LayoutElement>();
        delay_max = delay;

        if(element)
        {
            element.ignoreLayout = true;
            LerpRectTransform(start, end, t / speed);
        }
    }

    private void OnDestroy()
    {
        var element = GetComponent<LayoutElement>();

        if (element)
        {
            element.ignoreLayout = false;
        }
        if (trigger_on_complete)
            trigger_on_complete.doFadeIn = true;
    }

    void Update()
    {
        if(do_transform)
        {
            if(delay > 0.0f)
            {
                delay -= Time.deltaTime;
                if (delay > 1.25f)
                {
                    Color c = fadeout_display.color;
                    c.a = ((delay - 1.25f) / (delay_max - 1.25f));
                    fadeout_display.color = c;
                }
            }
            else
            {
                t += Time.deltaTime;

                LerpRectTransform(start, end, t / speed);
                if(t >= speed)
                {
                    Destroy(this);
                }
            }
        }        
    }

    void LerpRectTransform(RectTransform a, RectTransform b, float t)
    {
        t = Mathf.Min(1.0f, Mathf.Max(0.0f, t));

        rect.localScale = Vector3.Slerp(a.localScale, b.localScale, t);
        rect.anchoredPosition = Vector2.Lerp(start.anchoredPosition, end.anchoredPosition, t);
    }
}
