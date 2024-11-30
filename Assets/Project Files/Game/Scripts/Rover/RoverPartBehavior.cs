using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class RoverPartBehavior : MonoBehaviour
    {
        public RoverBehavior Master { get; private set; }
        public RoverPart Data { get; private set; }
        public SnapPoint SnapPoint { get; private set; }

        [SerializeField] Rigidbody2D rigidbodyRef;

        [SerializeField] AnimationCurve partSpawnCurve;
        [SerializeField] AnimationCurve partDissapearCurve;

        [SerializeField] Collider pickableCollidier;

        [SerializeField] List<GameObject> visualsObjects;

        public Rigidbody2D Rigidbody => rigidbodyRef;

        private Vector3 rigidbodyStartPosition;
        private Quaternion rigidbodyStartRotation;
        private Transform rigidbodyParent;

        protected Vector3 startPos;


        protected virtual void Awake()
        {
            rigidbodyStartPosition = Rigidbody.transform.localPosition;
            rigidbodyStartRotation = Rigidbody.transform.localRotation;
            rigidbodyParent = Rigidbody.transform.parent;
        }


        public virtual void Connect(RoverPart data, RoverBehavior master, SnapPoint point)
        {
            Data = data;
            Master = master;
            SnapPoint = point;

            if (point != null)
            {
                transform.position = point.transform.position - point.transform.up * 0.2f;
                transform.rotation = point.transform.rotation;
                transform.localScale = Vector3.zero;

                transform.DOScale(1, 0.4f).SetCurveEasing(partSpawnCurve);
                transform.DOMove(point.transform.position, 0.4f).SetEasing(Ease.Type.SineOut);
            }

            {
                Rigidbody.simulated = false;
                rigidbodyRef.transform.SetParent(rigidbodyParent);
                rigidbodyRef.transform.localPosition = rigidbodyStartPosition;
                rigidbodyRef.transform.localRotation = rigidbodyStartRotation;
                rigidbodyRef.linearVelocity = Vector2.zero;
            }
        }


        public virtual void Disconect(bool immediately)
        {
            if (Master != null)
            {
                Master.Disconect(this);
            }

            Master = null;
            Data = null;
            SnapPoint = null;

            transform.SetParent(null);


            if (immediately)
            {
                gameObject.SetActive(false);
            }
            else
            {
                transform.DOScale(0, 0.4f).SetCurveEasing(partDissapearCurve).OnComplete(() =>
                {
                    transform.localScale = Vector3.zero;
                    gameObject.SetActive(false);
                });
            }
        }


        public void SetLayer(PhysicsLayer layer)
        {
            foreach (var obj in visualsObjects)
            {
                obj.layer = (int)layer;
            }
        }


        public void DisablePickableCollider()
        {
            pickableCollidier.enabled = false;
        }


        public virtual void OnButtonPressed()
        {

        }


        public virtual void OnButtonReleased()
        {

        }


        public virtual void OnButtonClick()
        {

        }


        public virtual void Explode()
        {
            if (Rigidbody != null)
            {
                Rigidbody.simulated = true;
                transform.parent = null;
                Rigidbody.AddForce((transform.position - Master.transform.position).normalized * Rigidbody.mass * 2f, ForceMode2D.Impulse);
            }
        }


    }
}