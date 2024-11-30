using UnityEngine;
using Watermelon.SkinStore;
using Watermelon.VehicleFactory;

namespace Watermelon
{
    [DefaultExecutionOrder(-5)]
    public class GameController : MonoBehaviour
    {
        private static GameController instance;

        public static readonly string LEVEL_NUMBER_HASH = "level_number";

        [SerializeField] LevelDatabase levelDatabase;

        [Space]
        [SerializeField] GameObject finishConfetti;
        [SerializeField] UIController uiController;
        [SerializeField] MusicSource musicSource;

        private static LevelController levelController;
        private static SkinsController skinsController;
        private static SkinStoreController skinStoreController;

        private UIGame gameUI;

        public static LevelDatabase LevelDatabase => instance.levelDatabase;

        private static GameObject FinishConfetti => instance.finishConfetti;
        private static SimpleIntSave currentLevelNumberSave;

        private static int coinsPickedInLastLevel;

        public static int CurrentLevelNumber
        {
            get => currentLevelNumberSave.Value;
            set => currentLevelNumberSave.Value = value;
        }

        public static int Coins
        {
            get => CurrenciesController.Get(CurrencyType.Coins);
            set => CurrenciesController.Set(CurrencyType.Coins, value);
        }

        public void Awake()
        {
            instance = this;

            CacheComponent(out skinsController);
            CacheComponent(out skinStoreController);
            CacheComponent(out levelController);

            uiController.Init();

            musicSource.Init();
            musicSource.Activate();

            levelController.Init();

            skinsController.Init();
            skinStoreController.Init(skinsController);

            currentLevelNumberSave = SaveController.GetSaveObject<SimpleIntSave>(LEVEL_NUMBER_HASH);

            if(currentLevelNumberSave.Value < 1)
            {
                currentLevelNumberSave.Value = 1;
            }

            uiController.InitPages();

            WallsBehaviour.Init();
        }

        public void Start()
        {
            UIController.ShowPage<UIMainMenu>();

            gameUI = UIController.GetPage<UIGame>();

            GameLoading.MarkAsReadyToHide();
        }

        public static void OnPlayButtonPressed()
        {
            UIController.HidePage<UIMainMenu>(() =>
            {
                UIController.ShowPage<UIGame>();
            });

            LoadLevel();
        }

        private static void LoadLevel()
        {
            coinsPickedInLastLevel = 0;

            ResetLevel();

            Level level = LevelDatabase.GetLevel(CurrentLevelNumber);
            // Level level = LevelDatabase.GetLevel(1);

            LevelController.LoadLevel(level);
            instance.gameUI.UpdateLevelText(CurrentLevelNumber);
            instance.gameUI.HidePartButtonsImmediately();

            SavePresets.CreateSave("Level " + CurrentLevelNumber.ToString("000"), "Levels");

            AdsManager.ShowBanner();
        }

        public static void ResetLevel()
        {
            Coins -= coinsPickedInLastLevel;

            coinsPickedInLastLevel = 0;

            LevelController.PoolsHandler.ReturnEverythingToPool();
            LevelController.Rover.ResetRover();

            CameraController.ResetLevel();

            instance.gameUI.HidePartButtons();
            instance.gameUI.HidePlayButton();

        }

        public static void OnLevelCompleted()
        {
            AudioController.PlaySound(AudioController.AudioClips.winSound);
            FinishConfetti.transform.position = LevelController.Finish.transform.position;
            FinishConfetti.SetActive(true);
            FinishConfetti.GetComponent<ParticleSystem>().Play();

            CurrentLevelNumber++;

            instance.gameUI.HidePartButtons();

            Tween.DelayedCall(0.5f, () =>
            {
                UIController.HidePage<UIGame>();
                UIController.ShowPage<UIComplete>();
            });
        }

        public static void OnLevelCompleteClosed()
        {
            LoadLevel();

            AdsManager.RequestInterstitial();
            AdsManager.ShowInterstitial(null);

            UIController.HidePage<UIComplete>(() =>
            {
                UIController.ShowPage<UIGame>();
            });
        }

        public static void SkipLevel()
        {
            CurrentLevelNumber++;
            LoadLevel();
        }

        public static void ReplayCurrentLevel()
        {
            Coins -= coinsPickedInLastLevel;
            coinsPickedInLastLevel = 0;

            LoadLevel();
        }

        public static void ReturnToMainMenu()
        {
            ResetLevel();

            UIController.HidePage<UIGame>();
            UIController.ShowPage<UIMainMenu>();
        }

        public static void OnCoinPicked()
        {
            coinsPickedInLastLevel++;

            Coins++;
        }

        #region Extensions
        public bool CacheComponent<T>(out T component) where T : Component
        {
            Component unboxedComponent = gameObject.GetComponent(typeof(T));

            if (unboxedComponent != null)
            {
                component = (T)unboxedComponent;

                return true;
            }

            Debug.LogError(string.Format("Scripts Holder doesn't have {0} script added to it", typeof(T)));

            component = null;

            return false;
        }
        #endregion
    }
}