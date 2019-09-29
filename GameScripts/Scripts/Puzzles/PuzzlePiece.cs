using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Sound;
using System.Linq;

namespace Puzzles
{
    public class PuzzlePiece : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private GameObject shadow;
        private Vector3 difference;
        private Vector3 origin;
        private Plane plane;
        private Ray ray;
        private SpriteRenderer spriteRenderer;
        private PolygonCollider2D collider;
        private Rigidbody2D rigidbody;
        private Puzzle puzzle;
        private Vector3 lastPosition;
        private Vector3 velocity;
        private bool dragging = false;
        private bool placed = false;
        private bool recolored = false;
        private bool joined = false;
        private PuzzleMaster puzzleMaster;
        private List<PolygonCollider2D> createdColliders;
        public List<PolygonCollider2D> collectedColliders;

        public bool noMove;
        public float Smooth { get; set; }
        public float TiltAngle { get; set; }

        private const float snapDistance = .33f;
        private const float neighborCheckDistance = .5f;
        public List<PolygonCollider2D> neighboringColliders;

        private void Start()
        {
            createdColliders = new List<PolygonCollider2D>();
            collectedColliders = new List<PolygonCollider2D>();
            puzzleMaster = gameObject.GetComponentInParent<PuzzleMaster>();
            Smooth = puzzleMaster.smooth;
            TiltAngle = puzzleMaster.tiltAngle;
            
            puzzle = GetComponentInParent<Puzzle>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            collider = GetComponent<PolygonCollider2D>();
            rigidbody = GetComponent<Rigidbody2D>();

            collectedColliders.Add(collider);
            plane = new Plane(Vector3.forward, base.transform.position);
            var parent = transform.parent.position;
            origin = new Vector3(parent.x, parent.y, parent.z);
            lastPosition = transform.position;
            spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

            if (IsCloseEnoughToOrigin())
            {
                TriggerPlacementNoSound();
            }
        }


        private void FixedUpdate()
        {
            if (dragging)
            {
                velocity = transform.position - lastPosition;
                Quaternion targetRotation = Quaternion.Euler(velocity.y * TiltAngle, velocity.x * -TiltAngle, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * Smooth);
                if (shadow != null)
                {
                    shadow.transform.rotation = transform.rotation;
                    shadow.transform.localScale = transform.localScale;
                }
                lastPosition = transform.position;
            }
            
            if (placed && !recolored)
            {
                var smooth = puzzleMaster.recolorSpeed;
                var value = spriteRenderer.material.GetFloat("_GrayScale");
                if (value <= 0)
                    recolored = true;
                else
                    spriteRenderer.material.SetFloat("_GrayScale", value - (Time.deltaTime * smooth));
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!placed)
            {
                int newOrder = puzzleMaster.GetIndex();
                puzzle.PlaySound();
                collider.enabled = false;
                if (createdColliders.Count > 0)
                {
                    foreach(PolygonCollider2D childCollider in createdColliders)
                    {
                        childCollider.enabled = false;
                    }
                }
                ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);
                if (plane.Raycast(ray, out var distance))
                {
                    shadow = new GameObject();
                    var rend = shadow.AddComponent<SpriteRenderer>();
                    rend.sprite = spriteRenderer.sprite;
                    rend.color = new Color(0,0,0, 0.5f);
                    shadow.transform.localPosition = Vector3.zero;
                    shadow.transform.localScale = transform.localScale;
                    shadow.transform.parent = transform;
                    difference = ray.origin + ray.direction * distance - transform.position;
                    spriteRenderer.sortingOrder = newOrder + 1;
                    rend.sortingOrder = newOrder;
                    if (transform.childCount > 1)
                    {
                        foreach (Transform child in transform)
                        {
                            if (child.GetComponent<PuzzlePiece>())
                            {
                                child.GetComponent<SpriteRenderer>().sortingOrder = newOrder + 1;
                            }
                        }
                    }
                }
                dragging = true;   
            }
        }

    
        public void OnDrag(PointerEventData eventData)
        {
            if (!placed)
            {
                ray = eventData.pressEventCamera.ScreenPointToRay(eventData.position);
                if(plane.Raycast(ray, out var distance))
                {
                    transform.position = new Vector3(0,0,-0.25f) +
                                         Vector3.Scale(ray.origin + ray.direction * distance - difference,
                                             new Vector3(1,1,0));
                    shadow.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                }   
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            if (!placed)
            {
                int newOrder = puzzleMaster.GetIndex();
                collider.enabled = true;
                spriteRenderer.sortingOrder = newOrder;
                DestroyImmediate(shadow.gameObject);
                shadow = null;
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                dragging = false;
                if (IsCloseEnoughToOrigin())
                {
                    TriggerCorrectPlacement();
                    rigidbody.isKinematic = true;
                    TriggerPlaceAllChildren();
                }
                else
                {
                    puzzleMaster.PlayIncorrectPlacementSound();

                    if (transform.childCount > 0)
                    {
                        foreach (Transform child in transform)
                        {
                            PuzzlePiece piece = child.GetComponent<PuzzlePiece>();
                            SpriteRenderer rend = child.GetComponent<SpriteRenderer>();
                            rend.sortingOrder = newOrder;
                        }
                        foreach (PolygonCollider2D childCollider in createdColliders)
                        {
                            childCollider.enabled = true;
                        }
                    }
                }
            }
            if (!placed)
            {
                Transform neighborGrandParentPiece = AttemptToJoinPieces();
                if (neighborGrandParentPiece != null)
                {
                    JoinPiece(neighborGrandParentPiece);
                    neighborGrandParentPiece.GetComponent<PuzzlePiece>().CombineColliders(collectedColliders);
                }
                else
                {
                    if (transform.childCount > 0)
                    {
                        foreach (Transform child in transform)
                        {
                            neighborGrandParentPiece = child.GetComponent<PuzzlePiece>().AttemptToJoinPieces();
                            if (neighborGrandParentPiece != null)
                            {
                                JoinPiece(neighborGrandParentPiece);
                                neighborGrandParentPiece.GetComponent<PuzzlePiece>().CombineColliders(collectedColliders);
                            }
                        }
                    }
                }
                if (neighborGrandParentPiece != null)
                {
                    neighborGrandParentPiece.GetComponent<PuzzlePiece>().RemoveCollectedCollidersFromNeighborsOnChildren();
                }
            }
        }

        public Transform AttemptToJoinPieces()
        {
            Transform result = null;
            foreach (PolygonCollider2D neighbor in neighboringColliders.ToList())
            {
                if (Vector3.Distance(transform.position, neighbor.transform.position) < neighborCheckDistance)
                {
                    PuzzlePiece neighborPiece = neighbor.GetComponent<PuzzlePiece>();
                    if (!neighborPiece.noMove && !neighborPiece.placed)
                    {
                        result = neighborPiece.GetGrandestParentPiece(neighborPiece);
                        RemoveNeighbor(neighbor);
                        neighborPiece.RemoveNeighbor(collider);
                        break;
                    }
                }
            }
            return result;
        }

        public void RemoveCollectedCollidersFromNeighborsOnChildren()
        {
            if(transform.childCount > 0)
            {
                foreach(Transform child in transform)
                {
                    foreach(PolygonCollider2D coll in collectedColliders)
                    {
                        child.GetComponent<PuzzlePiece>().neighboringColliders.Remove(coll);
                    }
                }
            }
        }

        void RemoveNeighbor(PolygonCollider2D neighbor)
        { 
            neighboringColliders.Remove(neighbor);
        }

        void RemoveNeighbors()
        {
            foreach(PolygonCollider2D collider in collectedColliders)
            {
                RemoveNeighbor(collider);
            }
        }

        void TriggerPlaceAllChildren()
        {
            if (transform.childCount > 0)
            {
                foreach (Transform child in transform)
                {
                    PuzzlePiece childPuzzlePiece = child.GetComponent<PuzzlePiece>();
                    childPuzzlePiece.TriggerPlacementNoSound();
                }
            }
        }

        public void JoinPiece(Transform parentTransform)
        {
            joined = true;
            collider.enabled = false;
            if(createdColliders.Count > 0)
            {
                foreach(PolygonCollider2D coll in createdColliders)
                {
                    coll.enabled = false;
                }
            }
            transform.parent = parentTransform;
            transform.localPosition = Vector3.zero;
            rigidbody.isKinematic = true;
            while(transform.childCount > 0)
            {
                var temp = transform.childCount;
                foreach (Transform child in transform)
                {
                    child.transform.parent = parentTransform;
                }
                if (transform.childCount == temp)
                    break;
            }
            var parentPiece = parentTransform.GetComponent<PuzzlePiece>();
            parentPiece.collectedColliders.AddRange(collectedColliders);
            parentPiece.RemoveNeighbors();
        }

        bool IsCloseEnoughToOrigin()
        {
            return Vector3.Distance(transform.position, origin) < snapDistance;
        }

        void TriggerCorrectPlacement()
        {
            placed = true;
            puzzle.Correct(this);
            transform.position = origin;
            spriteRenderer.sortingOrder = -100;
        }

        public void TriggerPlacementNoSound()
        {
            placed = true;
            puzzle.CorrectNoTone(this);
            transform.position = origin;
            spriteRenderer.sortingOrder = -100;
        }

        public void GetAllNeighboringColliders()
        {
            neighboringColliders = new List<PolygonCollider2D>();
            collider = GetComponent<PolygonCollider2D>();
            PolygonCollider2D[] colliders = new PolygonCollider2D[2000];
            // For each collider point...
            for(int i = 0; i < collider.points.Length; i++)
            {
                // Search for all colliders found in a circle, get an array/list.
                Physics2D.OverlapCircleNonAlloc(collider.points[i], .1f, colliders);
                // Add them to a public list.
                for (int j = 0; j < collider.points.Length; j++)
                {
                    if (!neighboringColliders.Contains(colliders[j]) && colliders[j] != null && colliders[j] != collider)
                        neighboringColliders.Add(colliders[j]);
                }
            }
        }
        
        private void CombineColliders(List<PolygonCollider2D> other)
        {
            foreach (PolygonCollider2D collider in other)
            {
                var coll = gameObject.AddComponent<PolygonCollider2D>();
                coll.points = collider.points;
                createdColliders.Add(coll);
            }
        }

        private Transform GetGrandestParentPiece(PuzzlePiece piece)
        {
            PuzzlePiece parentPiece = piece.transform.parent.GetComponent<PuzzlePiece>();
            return parentPiece != null ?
                parentPiece.GetGrandestParentPiece(parentPiece) :
                piece.transform;
        }
    }
}
