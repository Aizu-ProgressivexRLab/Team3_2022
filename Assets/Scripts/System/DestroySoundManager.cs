using UnityEngine;

namespace System
{
    public class DestroySoundManager : MonoBehaviour
    {
        private AudioSource _audioSource;
        private static AudioSource _cashedAudioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _cashedAudioSource = _audioSource;
        }

        public static void Play(AudioClip audioClip)
        {
            _cashedAudioSource.PlayOneShot(audioClip);
        }
    }
}
