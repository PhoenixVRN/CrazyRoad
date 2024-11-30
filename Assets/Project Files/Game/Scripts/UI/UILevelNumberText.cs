using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    [RequireComponent(typeof(Text))]
    public class UILevelNumberText : MonoBehaviour
    {
        private const string LEVEL_LABEL = "LEVEL {0}";
        private static UILevelNumberText instance;

        [SerializeField] UIScaleAnimation uIScalableObject;

        private static UIScaleAnimation UIScalableObject => instance.uIScalableObject;
        private static Text levelNumberText;

        private static bool IsDisplayed = false;

        private void Awake()
        {
            instance = this;
            levelNumberText = GetComponent<Text>();
        }


        public static void Show(bool immediately = true)
        {
            if (IsDisplayed) return;

            IsDisplayed = true;

            levelNumberText.enabled = true;
            UIScalableObject.Show(immediately: immediately, scaleMultiplier: 1.05f);
        }

        public static void Hide(bool immediately = true)
        {
            if (!IsDisplayed) return;

            if (immediately) IsDisplayed = false;

            UIScalableObject.Hide(immediately: immediately, scaleMultiplier: 1.05f, onCompleted: delegate {

                IsDisplayed = false;
                levelNumberText.enabled = false;
            });
        }

    }
}
