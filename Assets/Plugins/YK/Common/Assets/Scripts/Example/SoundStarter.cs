using moveen.utils;
using UnityEngine;

namespace moveen.example {
    /// <summary>
    /// </summary>
    public class SoundStarter : MonoBehaviour, Startable {
        [BindOrLocalWarning]
        public AudioSource audioSource;
        [Tooltip("Audio clip to play")]
        [BindWarning]
        public AudioClip audioClip;
        [Tooltip("Play from position (seconds)")]
        public float startAt;
        [Tooltip("Finish at position (seconds)")]
        public float finishAt = -1;
        [Range(0, 1)]
        public float volume = 1;

        private void OnEnable() {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
        }

        public void start() {
            if (audioSource == null) return;
            if (audioClip == null) return;
            if (audioSource.isPlaying) audioSource.Stop();
            audioSource.clip = audioClip;
            audioSource.time = startAt;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }
}