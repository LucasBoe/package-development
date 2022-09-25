using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SoundSystem
{
    public class Sound : ScriptableObject
    {
        [SerializeField, ReadOnly] private SoundLibrary _myLibrary;

        [SerializeField, OnValueChanged("Rename"), HideIf("useMultipleClipVariants")] AudioClip clip;
        [SerializeField] bool useMultipleClipVariants = false;
        [SerializeField, ShowIf("useMultipleClipVariants"), OnValueChanged("Rename")] List<AudioClip> clips = new List<AudioClip>();
        [SerializeField, Range(0, 1)] float volume = 0.5f;

        [SerializeField, Range(0, 3), HideIf("randomizePitch")] float pitch = 1f;
        [SerializeField, MinMaxSlider(0f, 3f), ShowIf("randomizePitch")] Vector2 pitchRange = new Vector2(0.75f, 1.25f);
        [SerializeField] bool randomizePitch = false;
        public float AudioLength => clip.length;
        public float AudioVolume => volume;

        public SoundLibrary MyLibrary { get => _myLibrary; }
        public string Name { get => clip != null ? clip.name : (clips.Count > 0 && clips[0] != null) ? clips[0].name : "New Sound"; }

        public PlayingSound Play() { return SoundManager.Instance.Play(this); }
        public PlayingSound PlayLoop(float fadeDuration = 0.2f) { return SoundManager.Instance.Play(this, loop: true, fadeDuration); }

        public void PlaySound() => SoundManager.Instance.Play(this);
        public void Stop() => SoundManager.Instance.Stop(this);
        public void StopLoop() => Stop();


        internal float Configure(AudioSource audioSource, bool loop)
        {
            AudioClip c = useMultipleClipVariants ? clips[Random.Range(0, clips.Count)] : clip;
            audioSource.clip = c;
            audioSource.volume = volume;
            audioSource.pitch = randomizePitch ? Random.Range(pitchRange.x, pitchRange.y) : pitch;
            audioSource.loop = loop;
            audioSource.outputAudioMixerGroup = _myLibrary.audioMixerGroup;

            return c.length;
        }

#if UNITY_EDITOR
        public void Initialise(SoundLibrary mryLibrary)
        {
            _myLibrary = mryLibrary;
        }

        [ContextMenu("Rename to name")]
        private void Rename()
        {
            this.name = Name;
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Delete this")]
        [Button]
        private void DeleteThis()
        {
            _myLibrary.Sounds.Remove(this);
            Undo.DestroyObjectImmediate(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}