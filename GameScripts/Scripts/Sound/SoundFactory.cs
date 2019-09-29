using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Sound
{
    public class SoundFactory : MonoBehaviour
    {
        public AudioMixer Mixer;
        public Slider MusicSlider;
        public Slider EffectsSlider;

        public enum Instruments
        {
            Guitar,
            Piano,
            Kalimba,
            Tine,
            Lightbulb,
            Zimbira,
            Choir,
            StringMallet,
            Dulcimer
        }

        public static List<InstrumentScriptableObject> instrumentScriptableObjects;
        public static bool IsInitialized => instruments != null;


        private static Dictionary<Instruments, InstrumentScriptableObject> instruments;
        private static SFXPlayer SFXPlayer { get; set; }

        private void Start()
        {
            SFXPlayer = GetComponent<SFXPlayer>();
            InitializeFactory();
        }
        public static void InitializeFactory()
        {
            if (IsInitialized)
                return;

            instruments = new Dictionary<Instruments, InstrumentScriptableObject>();
            instrumentScriptableObjects = Resources.LoadAll<InstrumentScriptableObject>("Instruments").ToList();
            foreach (InstrumentScriptableObject instrument in instrumentScriptableObjects)
            {
                //Link instrument enum to instrument scriptable objects
                Instruments keyToAdd;
                if (Enum.TryParse(instrument.name, true, out keyToAdd))
                {
                    instruments.Add(keyToAdd, instrument);
                }
                else
                {
                    Debug.LogError("Dictionary pair was not made for InstrumentScriptableObject named " + instrument.name);
                }
            }
        }

        private static int avoidTone = -1;
        public static void PlayRandomTone(Instruments instrument)
        {
            int toneIndex = GetUniqueRandomToneIndex(avoidTone);
            AudioClip toneToPlay = instruments[instrument].audioClips[toneIndex];
            SFXPlayer.PlaySound(toneToPlay);
            avoidTone = toneIndex;
        }

        private static int GetUniqueRandomToneIndex(int intToAvoid)
        {
            int value = UnityEngine.Random.Range(0, instruments.Count());
            if (value != intToAvoid)
            {
                return value;
            }
            else
            {
                return GetUniqueRandomToneIndex(intToAvoid);
            }
        }

        static List<AudioClip> currentClips;
        static Instruments currentInstrument;
        static AudioClip lastTonePlayed;
        public static void PlayUniqueRandomTone(Instruments instrument)
        {
            // Checks to see if a list with the given instrument already has no content in it and if it is of differnet instrument than last called for.
            if (currentClips == null || currentInstrument != instrument || currentClips.Count <= 0)
            {
                // If not, takes the list of tones in a given InstrumentScriptableObject and puts them into a list.
                currentInstrument = instrument;
                currentClips = instruments[instrument].audioClips.ToList<AudioClip>();
                // Randomizes the list.
                Shuffle(currentClips);
            }
            // Plays the tone in the first element, so long as it doesn't match the last tone played.
            if (lastTonePlayed == currentClips[0])
                currentClips.RemoveAt(0);
            SFXPlayer.PlaySound(currentClips[0]);
            // Caches the last tone played.
            lastTonePlayed = currentClips[0];
            // Deletes the first element.
            currentClips.RemoveAt(0);
        }

        static void Shuffle(IList list)
        {
            System.Random random = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        static int currentSequentialIndex;
        public static void PlaySequentialTone(Instruments instrument)
        {
            if(currentSequentialIndex > instruments[instrument].audioClips.Length - 1)
            {
                currentSequentialIndex = 0;
            }
            SFXPlayer.PlaySound(instruments[instrument].audioClips[currentSequentialIndex]);
            currentSequentialIndex++;
        }

        public static void PlayAnySFX(AudioClip SFX)
        {
            SFXPlayer.PlaySound(SFX);
        }


        public void SetMusic()
        {
            float linear = MusicSlider.value;
            float dB;

            if (linear != 0)
                dB = 20.0f * Mathf.Log10(linear);
            else
                dB = -144.0f;

            Mixer.SetFloat("Music", dB);
        }
        public void SetEffects()
        {
            float linear = EffectsSlider.value;
            float dB;

            if (linear != 0)
                dB = 20.0f * Mathf.Log10(linear);
            else
                dB = -144.0f;

            Mixer.SetFloat("Effects", dB);
        }
    }
}