using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSoundHelper : MonoBehaviour
{
    public AudioClip[] clips;
    public void Play()
    {
        AudioHelper.StaticEffectRandom(clips);
    }
}
