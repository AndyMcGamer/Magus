using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Magus.Game
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        [HideInInspector]
        public AudioSource source;
        public AudioClip clip;
        public float volume;
        public float pitch;
        public bool loop;
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [SerializeField] private AudioMixerGroup mixer;
        [SerializeField] private Sound[] sounds;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.outputAudioMixerGroup = mixer;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }

        private void Start()
        {
            StopAllAudio();
            Play("Theme");
        }

        public void Play(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found");
                return;
            }

            s.source.Play();
        }

        public void StopAllAudio()
        {
            foreach (var s in sounds)
            {
                s.source.Stop();
            }
        }
    }
}
