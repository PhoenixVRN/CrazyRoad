using UnityEngine;
using Watermelon.IAPStore;
using Watermelon.SkinStore;

namespace Watermelon
{
    public class UIMainMenu : UIPage
    {
        public readonly float STORE_AD_RIGHT_OFFSET_X = 300F;

        [BoxGroup("References", "References")]
        [SerializeField] RectTransform safeAreaRectTransform;
        [BoxGroup("References")]
        [SerializeField] RectTransform tapToPlayRect;
        [BoxGroup("References")]
        [SerializeField] UINoAdsPopUp noAdsPopUp;

        [BoxGroup("Top Panel", "Top Panel")]
        [SerializeField] CurrencyUIPanelSimple coinsPanel;

        [BoxGroup("Side Buttons", "Side Buttons")]
        [SerializeField] UIMainMenuButton iapStoreButton;
        [BoxGroup("Side Buttons")]
        [SerializeField] UIMainMenuButton noAdsButton;
        [BoxGroup("Side Buttons")]
        [SerializeField] UIMainMenuButton skinsButton;
                
        private TweenCase tapToPlayPingPong;
        private TweenCase showHideStoreAdButtonDelayTweenCase;

        private void OnEnable()
        {
            AdsManager.ForcedAdDisabled += ForceAdPurchased;
        }

        private void OnDisable()
        {
            AdsManager.ForcedAdDisabled -= ForceAdPurchased;
        }

        public override void Init()
        {
            coinsPanel.Init();

            iapStoreButton.Init(STORE_AD_RIGHT_OFFSET_X);
            noAdsButton.Init(STORE_AD_RIGHT_OFFSET_X);
            skinsButton.Init(STORE_AD_RIGHT_OFFSET_X);

            iapStoreButton.Button.onClick.AddListener(IAPStoreButton);
            noAdsButton.Button.onClick.AddListener(NoAdButton);
            skinsButton.Button.onClick.AddListener(SkinsStoreButton);
            coinsPanel.AddButton.onClick.AddListener(AddCoinsButton);

            noAdsPopUp.Init();

            NotchSaveArea.RegisterRectTransform(safeAreaRectTransform);
        }

        #region Show/Hide

        public override void PlayShowAnimation()
        {
            showHideStoreAdButtonDelayTweenCase?.Kill();

            HideAdButton(true);
            iapStoreButton.Hide(true);
            skinsButton.Hide(true);
            ShowTapToPlay();
                        
            UILevelNumberText.Show();

            showHideStoreAdButtonDelayTweenCase = Tween.DelayedCall(0.12f, delegate
            {
                ShowAdButton();
                iapStoreButton.Show();
                skinsButton.Show();
            });

            UIController.OnPageOpened(this);
        }

        public override void PlayHideAnimation()
        {
            showHideStoreAdButtonDelayTweenCase?.Kill();

            HideTapToPlayText();

            HideAdButton();

            showHideStoreAdButtonDelayTweenCase = Tween.DelayedCall(0.1f, delegate
            {
                iapStoreButton.Hide();
                skinsButton.Hide();
            });

            Tween.DelayedCall(0.5f, delegate
            {
                UIController.OnPageClosed(this);
            });
        }

        #endregion

        #region Tap To Play Label

        public void ShowTapToPlay(bool immediately = false)
        {
            if (tapToPlayPingPong != null && tapToPlayPingPong.IsActive)
                tapToPlayPingPong.Kill();

            if (immediately)
            {
                tapToPlayRect.localScale = Vector3.one;

                tapToPlayPingPong = tapToPlayRect.transform.DOPingPongScale(1.0f, 1.05f, 0.9f, Ease.Type.QuadIn, Ease.Type.QuadOut, unscaledTime: true);

                return;
            }

            // RESET
            tapToPlayRect.localScale = Vector3.zero;

            tapToPlayRect.DOPushScale(Vector3.one * 1.2f, Vector3.one, 0.35f, 0.2f, Ease.Type.CubicOut, Ease.Type.CubicIn).OnComplete(delegate
            {

                tapToPlayPingPong = tapToPlayRect.transform.DOPingPongScale(1.0f, 1.05f, 0.9f, Ease.Type.QuadIn, Ease.Type.QuadOut, unscaledTime: true);

            });

        }

        public void HideTapToPlayText(bool immediately = false)
        {
            if (tapToPlayPingPong != null && tapToPlayPingPong.IsActive)
                tapToPlayPingPong.Kill();

            if (immediately)
            {
                tapToPlayRect.localScale = Vector3.zero;

                return;
            }

            tapToPlayRect.DOPushScale(Vector3.one * 1.2f, Vector3.zero, 0.2f, 0.35f, Ease.Type.CubicOut, Ease.Type.CubicIn);
        }

        #endregion

        #region Ad Button Label

        private void ShowAdButton(bool immediately = false)
        {
            if (AdsManager.IsForcedAdEnabled())
            {
                noAdsButton.Show(immediately);
            }
            else
            {
                noAdsButton.Hide(immediately: true);
            }
        }

        private void HideAdButton(bool immediately = false)
        {
            if(AdsManager.IsForcedAdEnabled())
            {
                noAdsButton.Hide(immediately);
            }
        }

        private void ForceAdPurchased()
        {
            noAdsButton.Hide(true);
        }

        #endregion

        #region Buttons

        public void TapToPlayButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);

            GameController.OnPlayButtonPressed();

            UIController.HidePage<UIMainMenu>(() =>
            {
                UIController.ShowPage<UIGame>();
            });
        }

        public void IAPStoreButton()
        {
            if (UIController.GetPage<UIStore>().IsPageDisplayed)
                return;

            UILevelNumberText.Hide(true);

            UIController.HidePage<UIMainMenu>();
            UIController.ShowPage<UIStore>();

            // reopening main menu only after store page was opened throug main menu
            UIController.PageClosed += OnIapOrSkinsStoreClosed;


            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        private void OnIapOrSkinsStoreClosed(UIPage page, System.Type pageType)
        {
            if (pageType.Equals(typeof(UIStore)) || pageType.Equals(typeof(UISkinStore)))
            {
                UIController.PageClosed -= OnIapOrSkinsStoreClosed;

                UIController.ShowPage<UIMainMenu>();
            }
        }

        public void SkinsStoreButton()
        {
            if (UIController.GetPage<UISkinStore>().IsPageDisplayed)
                return;

            UILevelNumberText.Hide(true);

            UIController.HidePage<UIMainMenu>();
            UIController.ShowPage<UISkinStore>();

            // reopening main menu only after store page was opened throug main menu
            UIController.PageClosed += OnIapOrSkinsStoreClosed;

            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }


        public void NoAdButton()
        {
            noAdsPopUp.Show();
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
        }

        public void AddCoinsButton()
        {
            UIController.ShowPage<UIStore>();
        }

        #endregion
    }
}
