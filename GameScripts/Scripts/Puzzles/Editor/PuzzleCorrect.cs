using UnityEditor;
using UnityEngine;

namespace Puzzles
{
    [CustomEditor(typeof(Puzzle))]
    public class PuzzleCorrect : Editor
    {  
        public override void OnInspectorGUI()
        {
            Puzzle thisPuzzle = (Puzzle) target;
            
            DrawDefaultInspector();
            if (GUILayout.Button("Fix Image"))
            {
                thisPuzzle.FixFragment();
            }
            if (GUILayout.Button("Get Neighbors"))
            {
                thisPuzzle.GetNeighbors();
            }
        }
    }
}
