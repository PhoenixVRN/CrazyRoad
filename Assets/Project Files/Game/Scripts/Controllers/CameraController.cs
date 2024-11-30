using UnityEngine;
using Watermelon.VehicleFactory;

namespace Watermelon
{
    public class CameraController : MonoBehaviour
    {
        private static CameraController instance;

        public static Camera MainCamera { get; private set; }


        public static Transform Target { get; private set; }

        private static Vector3 offset;
        private static Vector3 rotationOffset;
        private static float offsetMultiplier;

        public static Transform Transform => MainCamera.transform;

        private static CameraAspectSetuper.AspectRatioSettings aspectSettings;
        private CameraAspectSetuper.AspectRatioSettings aspectSettings3;
        static TweenCase offsetCase;


        private void Awake()
        {
            instance = this;

            MainCamera = GetComponent<Camera>();

            offsetMultiplier = 1;
        }

        private void Start()
        {
            aspectSettings = CameraAspectSetuper.CurrentSettings;
            aspectSettings3 = CameraAspectSetuper.CurrentSettings;
        }

        public static void Follow(Transform target)
        {
            Target = target;

            var startOffset = Transform.position - target.position;
            var startRotationOffset = Transform.eulerAngles - target.eulerAngles;

            offset = startOffset;
            rotationOffset = startRotationOffset;

            Tween.DoFloat(0, 1, 1f, (value) =>
            {
                offset = Vector3.Lerp(startOffset, aspectSettings.TargetOffset, value);
                rotationOffset = Vector3.Lerp(startRotationOffset, aspectSettings.TargetRotationOffset, value);
            }).SetEasing(Ease.Type.SineInOut);
        }

        public static void ResetLevel()
        {
            Target = null;
            MainCamera.transform.position = aspectSettings.StartPosition;
            MainCamera.transform.eulerAngles = aspectSettings.StartRotation;
        }

        public static void DoOffset(float value, float duration)
        {
            if (offsetCase != null && offsetCase.IsActive) offsetCase.Kill();

            Tween.DoFloat(offsetMultiplier, value, duration, (v) => offsetMultiplier = v);
        }

        public static void SetOffset(float value)
        {
            offsetMultiplier = value;
            MainCamera.fieldOfView = 60 + 40 * (value - 1);
        }

        private void FixedUpdate()
        {
            if (Target != null)
            {
                transform.position = Vector3.Lerp(transform.position, Target.transform.position + offset * offsetMultiplier, 0.5f);

                var rotation = Target.transform.eulerAngles + rotationOffset;

                var angleDifference = transform.eulerAngles - rotation;

                if (angleDifference.x > 180) rotation.x += 360;
                if (angleDifference.x < -180) rotation.x -= 360;

                if (angleDifference.y > 180) rotation.y += 360;
                if (angleDifference.y < -180) rotation.y -= 360;

                if (angleDifference.z > 180) rotation.z += 360;
                if (angleDifference.z < -180) rotation.z -= 360;

                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, rotation, 0.5f).SetZ(0);
            }
        }
    }
}