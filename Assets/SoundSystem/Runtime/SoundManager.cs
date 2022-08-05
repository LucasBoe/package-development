using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundSystem
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance => instance;
        private static SoundManager instance;

        Dictionary<PlayingSound, Sound> playingSounds = new Dictionary<PlayingSound, Sound>();

        [SerializeField] bool log;

        protected virtual void Awake()
        {
            if (instance != null)
                Destroy(gameObject);
            else
                instance = this;
        }
        internal PlayingSound Play(Sound soundData, bool loop = false, float fadeDuration = 0.2f)
        {
            PlayingSound playingAudio = new PlayingSound();
            playingAudio.FadeDuration = fadeDuration;
            playingAudio.SoundData = soundData;
            playingAudio.AudioSource = gameObject.AddComponent<AudioSource>();
            playingAudio.Coroutine = StartCoroutine(PlaySoundRoutine(soundData, playingAudio, loop));
            playingSounds.Add(playingAudio, soundData);

            Log("Play sound " + soundData.Name + " looping: " + loop);

            if (loop)
                FadeIn(playingAudio.AudioSource, soundData.AudioVolume, fadeDuration);

            return playingAudio;
        }

        private void Log(string message)
        {
            if (log) Debug.Log(message);
        }

        internal PlayingSound Play(Sound soundData, Vector3 position, bool loop = false, float fadeDuration = 0.2f)
        {
            return Play(soundData, loop);
        }

        internal void Stop(Sound sound)
        {
            Log("stop sound " + sound.Name);
            List<PlayingSound> toStop = new List<PlayingSound>();

            foreach (KeyValuePair<PlayingSound, Sound> playingSound in playingSounds)
            {
                if (playingSound.Value == sound)
                    toStop.Add(playingSound.Key);
            }

            for (int i = toStop.Count - 1; i >= 0; i--)
            {
                toStop[i].Stop();
            }
        }
        internal void Stop(PlayingSound sound)
        {
            sound.Stop();
        }

        private IEnumerator PlaySoundRoutine(Sound data, PlayingSound playing, bool loop)
        {
            float length = data.Configure(playing.AudioSource, loop);
            Log("sound " + data.Name + " configured, length: " + length);
            playing.AudioSource.Play();

            if (!loop)
            {
                yield return new WaitForSeconds(length);
                playing.Stop();
            }
        }
        internal void FadeIn(AudioSource audioSource, float volume, float fadeDuration) => StartCoroutine(VolumeBlendRoutine(audioSource, 0f, volume, fadeDuration, null));
        internal void FadeOut(AudioSource audioSource, float volume, float fadeDuration, Action onFinished) => StartCoroutine(VolumeBlendRoutine(audioSource, volume, 0f, fadeDuration, onFinished));
        private IEnumerator VolumeBlendRoutine(AudioSource audioSource, float from, float to, float fadeDuration, Action onFinished)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / fadeDuration;
                float volume = Mathf.Lerp(from, to, t);
                audioSource.volume = volume;
                yield return null;
            }

            audioSource.volume = to;
            onFinished?.Invoke();
        }
        internal void RemoveFromPlaying(PlayingSound playingAudio) => playingSounds.Remove(playingAudio);
    }
    public class PlayingSound
    {
        public Sound SoundData;
        public AudioSource AudioSource;
        public Coroutine Coroutine;
        public float FadeDuration;

        public void Stop()
        {
            SoundManager soundManager = SoundManager.Instance;

            if (Coroutine != null)
                soundManager.StopCoroutine(Coroutine);

            soundManager.RemoveFromPlaying(this);
            soundManager.FadeOut(AudioSource, SoundData.AudioVolume, FadeDuration, () => UnityEngine.Object.Destroy(AudioSource));
        }
    }
}
