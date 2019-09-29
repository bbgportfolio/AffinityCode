using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    public class SFXPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioSource[] audioSources = null;
        private Queue<AudioSource> sourceQueue;

        private void Start()
        {
            sourceQueue = new Queue<AudioSource>();
            for(int i = 0; i < audioSources.Length; i++)
            {
                sourceQueue.Enqueue(audioSources[i]);
            }
        }

        public void PlaySound(AudioClip SFX)
        {
            AudioSource source = GetBestAudioSource();
            source.clip = SFX;
            source.Play();
        }

        private AudioSource GetBestAudioSource()
        {
            AudioSource temp;
            for (int i = 0; i < audioSources.Length; i++)
            {
                if (!sourceQueue.Peek().isPlaying)
                    return sourceQueue.Peek();
                else
                {
                    temp = sourceQueue.Peek();
                    sourceQueue.Dequeue();
                    sourceQueue.Enqueue(temp);
                }
            }
            return sourceQueue.Peek();
        }
    }
}
