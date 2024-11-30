using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Watermelon.VehicleFactory
{
    public class CameraAspectSetuper : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] List<AspectRatioSettings> settings = new List<AspectRatioSettings>();
        private static List<AspectRatioSettings> Settings { get => instance.settings; set => instance.settings = value; }

        private static CameraAspectSetuper instance;
        public static AspectRatioSettings CurrentSettings { get; private set; }


        private void Awake()
        {
            instance = this;
        }

        public void OnEnable()
        {
            CurrentSettings = GetSettings();
            //ApplyAspectRatioSettings(GetSettings());
        }

        public static AspectRatioSettings GetSettings()
        {
            Settings = Settings.OrderByDescending(x => x.AspectRatio).ToList();

            float currentAspectRatio = Screen.width / (float)Screen.height;

            for (int i = 0; i < Settings.Count; i++)
            {
                // if our aspect ratio is spesified at settings - perfect - applying settings
                if (currentAspectRatio == Settings[i].AspectRatio)
                {
                    return Settings[i];
                }
                // if current aspect is bigger  than current saved settings aspect - we have not this aspect saved - interpolating beetween current and previous settings
                else if (currentAspectRatio > Settings[i].AspectRatio)
                {
                    // if biggest saved setting aspect is smaller than we need - anyway applying this settings
                    if (i == 0)
                    {
                        return Settings[i];
                    }
                    // otherwise interpolating settings
                    else
                    {
                        float interpolationCoef = (currentAspectRatio - Settings[i].AspectRatio) / (Settings[i - 1].AspectRatio - Settings[i].AspectRatio);

                        AspectRatioSettings interpolatedSettings = AspectRatioSettings.GetInterpolatedSettings(Settings[i], Settings[i - 1], interpolationCoef);

                        return interpolatedSettings;
                    }
                }
                // if current aspect is smaller then smallest saved settings - applying smallest settings
                else if (i == Settings.Count - 1)
                {
                    return Settings[i];
                }
                // otherwise moving to next settings
            }

            return Settings[0];
        }


        // applying saved or calculated aspect settings
        private void ApplyAspectRatioSettings(AspectRatioSettings settings)
        {
            // [add here initializing of saved camera settings]
        }


        [System.Serializable]
        public class AspectRatioSettings
        {
            // standart fields
            [SerializeField] string ratioLabel;
            public string RatioLabel { get { return ratioLabel; } private set { ratioLabel = value; } }
            [SerializeField] float aspectRatio;
            public float AspectRatio { get { return aspectRatio; } private set { aspectRatio = value; } }

            [Space(5)]
            // [add here custom settings depending on project]
            [SerializeField] Vector3 startPosition;
            [SerializeField] Vector3 startRotation;

            [Space(5)]
            [SerializeField] Vector3 targetOffset;
            [SerializeField] Vector3 targetRotationOffset;


            public Vector3 StartPosition => startPosition;
            public Vector3 StartRotation => startRotation;
            public Vector3 TargetOffset => targetOffset;
            public Vector3 TargetRotationOffset => targetRotationOffset;

            public AspectRatioSettings()
            {
                ratioLabel = "new ratio";
                aspectRatio = 0f;
            }

            public static AspectRatioSettings GetInterpolatedSettings(AspectRatioSettings smallerSettings, AspectRatioSettings biggerSettings, float interpolationCoef)
            {
                AspectRatioSettings newSettins = new AspectRatioSettings();
                newSettins.ratioLabel = "interpolated";

                // [initialize newSettings with interpolated values]
                newSettins.startPosition = Vector3.Lerp(smallerSettings.startPosition, biggerSettings.startPosition, interpolationCoef);
                newSettins.startRotation = Vector3.Lerp(smallerSettings.startRotation, biggerSettings.startRotation, interpolationCoef);
                newSettins.targetOffset = Vector3.Lerp(smallerSettings.targetOffset, biggerSettings.targetOffset, interpolationCoef);
                newSettins.targetRotationOffset = Vector3.Lerp(smallerSettings.targetRotationOffset, biggerSettings.targetRotationOffset, interpolationCoef);

                return newSettins;
            }
        }
    }
}