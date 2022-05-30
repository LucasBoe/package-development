using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

public class SimpleSoundPlayer : MonoBehaviour
{
    [SerializeField, Expandable] Sound sound;

    [Button]
    public void Play ()
    {
        sound.Play();
    }
}
