using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonImageAnimator : MonoBehaviour
{
    public Image target;
    public Sprite[] sprites;
    public float anim_speed = 0.5f;
    float t = 0.0f;
    void Update()
    {
        t += Time.deltaTime;
        target.sprite = sprites[Mathf.RoundToInt(t / anim_speed) % sprites.Length];
    }
}
