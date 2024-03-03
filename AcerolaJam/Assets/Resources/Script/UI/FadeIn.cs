using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    public Color endColour = Color.white;
    public bool doFadeIn = false;
    public float speed = 1.0f;
    float t = 0.0f;

    void Update()
    {
        if(doFadeIn)
        {
            t += Time.deltaTime;
            text.color = new Color(endColour.r, endColour.g, endColour.b, endColour.a * (t / speed));
            if (t >= speed)
            {
                text.color = endColour;
                doFadeIn = false;
                Destroy(this);
            }
        }
    }
}
