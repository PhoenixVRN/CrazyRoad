using UnityEngine;

namespace Watermelon
{

    public class WingsBoosterBehavior : RoverPartBehavior
    {
        private static readonly int START_TRIGGER = Animator.StringToHash("Start");
        private static readonly int END_TRIGGER = Animator.StringToHash("End");

        [SerializeField] Transform leftWing;
        [SerializeField] Transform rightWing;

        [Space]
        [SerializeField] Transform firstGear;
        [SerializeField] Transform secondGear;

        [SerializeField] float impulse;
        [SerializeField] float force;

        [SerializeField] Animator animator;

        public bool IsActive { get; private set; }
        public bool IsForceActive { get; private set; }


        protected override void Awake()
        {
            base.Awake();

            IsForceActive = false;
            IsActive = false;
        }

        public override void OnButtonPressed()
        {
            base.OnButtonPressed();

            animator.SetTrigger(START_TRIGGER);
            IsActive = true;
        }

        public void StartWingDown()
        {
            Master.AddForce(-(transform.up + transform.right).normalized * impulse, ForceMode2D.Impulse);

            IsForceActive = true;
        }

        public void EndWingDown()
        {
            IsForceActive = false;
        }

        private void FixedUpdate()
        {
            if (IsForceActive)
            {
                Master.AddForce(-(transform.up + transform.right * 0.25f).normalized * force, ForceMode2D.Force);
            }

            if (IsActive)
            {
                Master.AddForce(Vector2.up * 3f);
            }
        }

        public void IdleState()
        {
            if (IsActive)
            {
                IsActive = false;
            }
        }

        private void OnDisable()
        {
            IsActive = false;
            IsForceActive = false;
        }
    }
}