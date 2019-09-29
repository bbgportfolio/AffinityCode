using UnityEngine;
using System.Collections;
using Sound;
using UnityEngine.Events;
using System.Collections.Generic;
using Data;

namespace Puzzles
{
    public class PuzzleMaster : MonoBehaviour
    {
        public static PuzzleMaster instance;
        public Animation puzzleCompleteLightAnimation;
        public SoundFactory.Instruments toneType;
        public AudioClip completeTone;
        public AudioClip incorrectPlacementTone;
        public delegate void PuzzleCompleteHandler();
        public static event PuzzleCompleteHandler onPuzzleComplete;

        // Global puzzle piece variables.
        public float smooth = 2f;
        public float tiltAngle = 30f;
        public float recolorSpeed = 4f;
        public bool playCompletionTone = true;
        public UnityEvent completionEvents;
        private int sortIndex;

        public int numberOfPuzzlePieces { get; set; }

        private void Awake()
        {
            instance = this;
            //Logger.I.LogInitialized(this);
        }

        public void DecrementCheckRemainingPuzzlePieces()
        {
            numberOfPuzzlePieces--;
            Debug.Log("Number of puzzle pieces remaining = " + numberOfPuzzlePieces);
            if(numberOfPuzzlePieces <= 0)
            {
                StartCoroutine(PuzzleComplete());
            }
        }

        IEnumerator PuzzleComplete()
        {
            WaitForSeconds delay = new WaitForSeconds(1f);
            yield return delay;
            if(puzzleCompleteLightAnimation != null)
                puzzleCompleteLightAnimation.Play();
            yield return new WaitForSeconds(.3f);
            PlayLevelCompleteSound();
            yield return delay;
            onPuzzleComplete();
            CheckOldGame();
        }

        public void CheckOldGame()
        {
            if(Progression.CurrentlyLoaded.Current != 1)
            {
                completionEvents.Invoke();
            }
        }

        void PlayLevelCompleteSound()
        {
            if (playCompletionTone && completeTone != null)
                SoundFactory.PlayAnySFX(completeTone);
        }
        
        public void PlayIncorrectPlacementSound()
        {
            if (incorrectPlacementTone)
                SoundFactory.PlayAnySFX(incorrectPlacementTone);
        }
        
        public int GetIndex()
        {
            return ++sortIndex;
        }
    }
}
