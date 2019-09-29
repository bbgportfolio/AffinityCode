using System.Collections.Generic;
using System.Linq;
using Sound;
using UnityEngine;

namespace Puzzles
{
    public class Puzzle : MonoBehaviour
    {
        public Material SpriteMat;
        public bool CenterImage;
        private List<PuzzlePiece> pieces;
        private SoundFactory.Instruments toneType;

        private void Start()
        {
            if(pieces == null)
                InitializePuzzle();
            toneType = GetComponent<PuzzleMaster>().toneType;
        }

        public void FixFragment()
        {
            int count = 0;
            foreach (Transform frag in transform)
            {
                //Center image
                if(CenterImage)
                    frag.transform.localPosition = Vector3.zero;
                
                //Sprite renderer
                var rend = frag.GetComponent<SpriteRenderer>();
                rend.material = SpriteMat;
                
                //PuzzlePiece
                var piece = frag.gameObject.GetComponent<PuzzlePiece>();
                if(piece == null)
                    piece = frag.gameObject.AddComponent<PuzzlePiece>();
                
                //RigidBody
                var rigid = frag.gameObject.GetComponent<Rigidbody2D>();
                if (rigid == null)
                    rigid = frag.gameObject.AddComponent<Rigidbody2D>();
                rigid.gravityScale = 0;
                rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
                
                //PolygonCollider2D
                var poly = frag.gameObject.GetComponent<PolygonCollider2D>();
                if(poly == null)
                    poly = frag.gameObject.AddComponent<PolygonCollider2D>();

                //Layer
                frag.gameObject.layer = 8;

                //Sort Orders
                if (rend != null && piece != null)
                {
                    rend.sortingOrder = count;
                }
                count--;
            }
        }

        public void GetNeighbors()
        {
            foreach (Transform frag in transform)
            {
                var piece = frag.gameObject.GetComponent<PuzzlePiece>();
                piece.GetAllNeighboringColliders();
            }
        }

        public void PlaySound()
        {
            SoundFactory.PlayUniqueRandomTone(toneType);
        }

        public void Correct(PuzzlePiece piece)
        {
            PlaySound();
            RemovePiece(piece);
            PuzzleMaster.instance.DecrementCheckRemainingPuzzlePieces();
        }

        public void CorrectNoTone(PuzzlePiece piece)
        {
            RemovePiece(piece);
            PuzzleMaster.instance.DecrementCheckRemainingPuzzlePieces();
        }

        void InitializePuzzle()
        {
            pieces = GetComponentsInChildren<PuzzlePiece>().ToList();
            PuzzleMaster.instance.numberOfPuzzlePieces += pieces.Count;
            Debug.Log("Number of puzzle pieces = " + PuzzleMaster.instance.numberOfPuzzlePieces);
            Debug.Log("Puzzle Initialized.");
        }

        void RemovePiece(PuzzlePiece piece)
        {
            if(pieces == null)
            {
                InitializePuzzle();
                pieces.Remove(piece);
            }
            else
            {
                pieces.Remove(piece);
            }
        }

        // Unused.
        float GetRandomPitch()
        {
            int num = Random.Range(6, 14);
            num += num % 0 > 0 ? 1 : 0;
            float result = num;
            result /= 10;
            return result;
        }
    }
}
