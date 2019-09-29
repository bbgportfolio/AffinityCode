using System.Collections;
using Puzzles;
using UnityEngine;

namespace SceneManagement
{
    public class LevelAdvancer : SceneTransitioner
    {
        ///
        /// Subscribe to an event in another script that tells us when the puzzle is complete.
        /// When this event fires, run a coroutine that does the following:
        /// - Calls and waits for another coroutine to finish playing the level complete sequence.
        /// - Calls the LoadSceneAsync, passing in the next sequential scene based on the sceneManager's count.
        ///
        int sceneIndex = 0;

        //Subscribe to an event in another script that tells us when the puzzle is complete.
        protected override void Start()
        {
            base.Start();
            PuzzleMaster.onPuzzleComplete += BeginAdvanceToNextLevel;

        }

        void BeginAdvanceToNextLevel()
        {
            StartCoroutine(AdvanceToNextLevel());
            PuzzleMaster.onPuzzleComplete -= BeginAdvanceToNextLevel;
        }

        IEnumerator AdvanceToNextLevel()
        {
            sceneIndex++;
            yield return new WaitForEndOfFrame(); // Call another coroutine elsewhere to play the level complete sequence.
            LoadSceneAsync("TestLevel" + sceneIndex);
        }

    }
}
