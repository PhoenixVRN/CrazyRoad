using UnityEngine;

namespace Watermelon
{
    public class SpikedWheelsPart : WheelPartBehavior
    {
        [SerializeField] Transform spikes;

        CircleCollider2D circleCollider;

        public bool IsPressed { get; private set; }


        protected override void Awake()
        {
            base.Awake();

            circleCollider = colliderRef as CircleCollider2D;

            spikes.localScale = Vector3.forward;

            IsPressed = false;
        }


        public override void OnButtonPressed()
        {
            base.OnButtonPressed();

            spikes.DOScale(1f, 0.5f);

            IsPressed = true;
        }


        public override void OnButtonReleased()
        {
            base.OnButtonReleased();

            spikes.DOScale(Vector3.forward, 0.5f);

            IsPressed = false;
        }

        public override void DriveUpdate()
        {
            base.DriveUpdate();

            if (IsPressed && Physics2D.OverlapCircle(transform.position, circleCollider.radius * 1.6f, GetLayerMask(PhysicsLayer.Ground)))
            {
                Master.Rigidbody.AddForceAtPosition(-transform.up * (10 + Master.Rigidbody.linearVelocity.sqrMagnitude * 0.5f), transform.position);
            }
        }


        public void OnDisable()
        {
            spikes.localScale = Vector3.forward;

            IsPressed = false;
        }


    }
}