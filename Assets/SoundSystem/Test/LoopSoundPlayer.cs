using NaughtyAttributes;
using SoundSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopSoundPlayer : MonoBehaviour
{
    [SerializeField, Expandable] Sound sound;

    private PlayingSound playing;

    [Button]
    public void Play()
    {
        playing = sound.PlayLoop();
    }

    [Button]
    public void Stop()
    {
        sound.StopLoop();
    }

    [Button]
    public void StopInstance()
    {
        if (playing != null) playing.Stop();
    }
}
