using UnityEngine;

namespace Watermelon
{
    public class RocketBoosterBehavior : RoverPartBehavior
    {

        [SerializeField] float force;
        [SerializeField] float impulse;

        [SerializeField] ParticleSystem trailParticle;

        public bool IsActive { get; private set; }


        public override void OnButtonPressed()
        {
            base.OnButtonPressed();

            if (Master == null) return;

            IsActive = true;

            trailParticle.Play();

            Master.AddForce(-transform.right * impulse, SnapPoint.ForcePoint.position, ForceMode2D.Impulse);

            Tween.DelayedCall(2f, () =>
            {
                if (Master != null)
                {
                    trailParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    IsActive = false;

                    Master.AirResistanceMult = 0.3f;

                    Tween.DelayedCall(1f, () =>
                    {
                        if (Master != null)
                        {
                            Master.AirResistanceMult = 1;
                        }
                    });
                }
            });

        }


        private void FixedUpdate()
        {
            if (IsActive)
            {
                Master.AddForce((-transform.right) * force, SnapPoint.ForcePoint.position);
            }
        }


        private void OnDisable()
        {
            IsActive = false;

            trailParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);

            if (Master != null) Master.AirResistanceMult = 1f;
        }
    }
}
