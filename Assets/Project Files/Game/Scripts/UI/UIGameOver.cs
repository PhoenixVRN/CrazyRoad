using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIGameOver : UIPage
    {
        [BoxGroup("References", "References")]
        [SerializeField] UIFadeAnimation backgroundFade;

        [BoxGroup("Content", "Content")]
        [SerializeField] UIScaleAnimation levelFailed;

        [BoxGroup("Buttons", "Buttons")]
        [SerializeField] Button continueButton;

        [BoxGroup("Buttons"), Space]
        [SerializeField] float noThanksDelay;
        [BoxGroup("Buttons")]
        [SerializeField] Button noThanksButton;
        [BoxGroup("Buttons")]
        [SerializeField] TextMeshProUGUI noThanksText;

        private TweenCase continuePingPongCase;
        private UIScaleAnimation continueButtonScalable;

        public override void Init()
        {
            continueButton.onClick.AddListener(ContinueButton);
            noThanksButton.onClick.AddListener(NoThanksButton);

            continueButtonScalable = new UIScaleAnimation(continueButton);
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            levelFailed.Hide(immediately: true);
            continueButtonScalable.Hide(immediately: true);
            HideNoThanksButton();

            float fadeDuration = 0.3f;
            backgroundFade.Show(fadeDuration);

            Tween.DelayedCall(fadeDuration * 0.8f, delegate { 
            
                levelFailed.Show();
                
                ShowNoThanksButton(noThanksDelay);

                continueButtonScalable.Show(scaleMultiplier: 1.05f);

                continuePingPongCase = continueButtonScalable.Transform.DOPingPongScale(1.0f, 1.05f, 0.9f, Ease.Type.QuadIn, Ease.Type.QuadOut, unscaledTime: true);

                UIController.OnPageOpened(this);
            });

        }

        public override void PlayHideAnimation()
        {
            backgroundFade.Hide(0.3f);

            Tween.DelayedCall(0.3f, delegate {

                if (continuePingPongCase != null && continuePingPongCase.IsActive) continuePingPongCase.Kill();

                UIController.OnPageClosed(this);
            });
        }

        #endregion

        #region NoThanks Block

        public void ShowNoThanksButton(float delayToShow = 0.3f, bool immediately = false)
        {
            if (immediately)
            {
                noThanksButton.gameObject.SetActive(true);
                noThanksText.gameObject.SetActive(true);

                return;
            }

            Tween.DelayedCall(delayToShow, delegate { 

                noThanksButton.gameObject.SetActive(true);
                noThanksText.gameObject.SetActive(true);

            });
        }

        public void HideNoThanksButton()
        {
            noThanksButton.gameObject.SetActive(false);
            noThanksText.gameObject.SetActive(false);
        }

        #endregion

        #region Buttons 

        public void ContinueButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            AdsManager.ShowRewardBasedVideo((hasReward) =>
            {
                if (hasReward)
                {
                    GameController.SkipLevel();
                    UIController.HidePage<UIGameOver>();
                    UIController.ShowPage<UIGame>();
                }
                else
                {
                    NoThanksButton();
                }
            });
        }

        public void NoThanksButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound); 
            
            GameController.ReplayCurrentLevel();

            UIController.HidePage<UIGameOver>();
            UIController.ShowPage<UIGame>();

        }

        #endregion
    }
}