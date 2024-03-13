using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AudioHelper : MonoBehaviour
{
    public static AudioHelper Instance;

    public AudioSource background_source;
    public List<AudioSource> sound_sources;

    Queue<AudioSource> sources_ready = new();
    HashSet<AudioSource> sources_used = new();

    public float master_volume = 0.5f;
    public float bg_volume = 0.5f;
    public float effect_volume = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach(var source in sound_sources)
        {
            sources_ready.Enqueue(source);
        }
        UpdateSources();
    }

    private void Update()
    {
        if (sources_used.Count > 0)
        {
            List<AudioSource> requeue = new();
            foreach (var source in sources_used)
            {
                if(!source.isPlaying)
                {
                    sources_ready.Enqueue(source);
                    requeue.Add(source);
                }
            }
            foreach(var source in requeue)
            {
                sources_used.Remove(source);
            }
        }
    }

    public void PlaySoundEffect(AudioClip effect)
    {
        if (sources_ready.Count > 0)
        {
            AudioSource source = sources_ready.Dequeue();
            sources_used.Add(source);
            source.clip = effect;
            source.Play();
        }
    }

    public void PlayBackgroundMusic(AudioClip bg)
    {
        background_source.clip = bg;
        background_source.Play();
    }    

    public static void StaticEffectRandom(AudioClip[] effect)
    {
        if (effect.Length == 1)
        {
            Instance.PlaySoundEffect(effect[0]);
        }
        else
        {
            Instance.PlaySoundEffect(effect[UnityEngine.Random.Range(0, effect.Length)]);
        }
    }

    public static void StaticEffect(AudioClip effect)
    {
        Instance.PlaySoundEffect(effect);
    }

    public static void StaticBG(AudioClip bg)
    {
        Instance.PlayBackgroundMusic(bg);
    }

    void UpdateSources()
    {
        background_source.volume = master_volume * bg_volume;
        float effect_level = master_volume * effect_volume;
        foreach(var source in sound_sources)
        {
            source.volume = effect_level;
        }
    }

    public void SetMasterLevel(float value)
    {
        master_volume = Mathf.Clamp01(value);
        UpdateSources();
    }
    public void SetEffectLevel(float value)
    {
        effect_volume = Mathf.Clamp01(value);
        UpdateSources();
    }
    public void SetBackgroundLevel(float value)
    {
        bg_volume = Mathf.Clamp01(value);
        UpdateSources();
    }

    public float GetEffectLevel()
    {
        return master_volume * effect_volume;
    }

    public float GetBGLevel()
    {
        return master_volume * bg_volume;
    }
}
