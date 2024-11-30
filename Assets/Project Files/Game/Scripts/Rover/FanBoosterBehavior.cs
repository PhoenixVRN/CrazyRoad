using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class FanBoosterBehavior : RoverPartBehavior
    {

        [SerializeField] float force = 10;
        [SerializeField] float initialImpulseForce = 20;

        [Space]
        [SerializeField] Transform fan;
        [SerializeField] float maxAngularSpeed = 180;
        [SerializeField] float accelerationMultiplier = 0.5f;
        [SerializeField] float fadeOffMultiplier = 0.1f;

        [Space]
        [SerializeField] List<TrailRenderer> trails;

        private float fanSpeed;
        private float initialForceCooldown = 1.5f;
        private float lastTimeStarted;

        public bool IsActive { get; private set; }


        protected override void Awake()
        {
            base.Awake();

            IsActive = false;
            fanSpeed = 0;
        }


        public override void OnButtonPressed()
        {
            if (Master == null)
                return;

            IsActive = true;

            if (Time.timeSinceLevelLoad > lastTimeStarted + initialForceCooldown)
            {
                Master.AddForce(transform.up * initialImpulseForce, SnapPoint.ForcePoint.position, ForceMode2D.Impulse);
                lastTimeStarted = Time.timeSinceLevelLoad;
            }

            var center = CameraController.MainCamera.WorldToViewportPoint(transform.position);

            center.y *= (1 / CameraController.MainCamera.aspect);

            WaveController.SendWave(center, 1f);

            foreach (var trail in trails)
            {
                trail.emitting = true;
            }
        }


        public override void OnButtonReleased()
        {
            IsActive = false;

            foreach (var trail in trails)
            {
                trail.emitting = false;
            }
        }


        private void FixedUpdate()
        {
            if (IsActive)
            {
                Master.AddForce(transform.up * force, SnapPoint.ForcePoint.position);

                if (fanSpeed != maxAngularSpeed)
                {
                    fanSpeed = Mathf.Lerp(fanSpeed, maxAngularSpeed, accelerationMultiplier * Time.fixedDeltaTime);
                }
            }
            else
            {
                if (fanSpeed != 0)
                {
                    fanSpeed = Mathf.Lerp(fanSpeed, 0, fadeOffMultiplier * Time.fixedDeltaTime);
                }
            }

            if (fanSpeed != 0)
            {
                fan.localEulerAngles += Vector3.forward * fanSpeed * Time.deltaTime;
            }
        }


        private void OnDisable()
        {
            IsActive = false;

            fanSpeed = 0;

            foreach (var trail in trails)
            {
                trail.emitting = false;
            }
        }
    }
}