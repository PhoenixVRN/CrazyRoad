using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIGame : UIPage
    {
        [SerializeField] Text levelText;
        [SerializeField] RectTransform canvasRect;

        [SerializeField] List<RoverPartButton> partButtons;

        [SerializeField] RectTransform buttonsParent;

        [SerializeField] GameObject playButton;

        [Space]
        [SerializeField] GameObject replayButton;

        public Vector2 CanvasSize => canvasRect.sizeDelta;

        [BoxGroup("References", "References")]
        [SerializeField] RectTransform safeAreaRectTransform;

        [BoxGroup("Top Panel", "Top Panel")]
        [SerializeField] CurrencyUIPanelSimple coinsPanel;

        public override void Init()
        {
            coinsPanel.Init();

            NotchSaveArea.RegisterRectTransform(safeAreaRectTransform);
        }

#region Show/Hide
        public override void PlayHideAnimation()
        {
            coinsPanel.Disable();

            UILevelNumberText.Hide();

            UIController.OnPageClosed(this);
        }

        public override void PlayShowAnimation()
        {
            coinsPanel.Activate();

            UILevelNumberText.Show();

            UIController.OnPageOpened(this);
        }

        #endregion

        public void HidePartButtonsImmediately()
        {
            buttonsParent.DOAnchoredPosition(Vector2.down * buttonsParent.sizeDelta.y, 0).SetEasing(Ease.Type.SineOut);
            replayButton.gameObject.SetActive(false);
        }


        public void ShowPlayButton()
        {
            playButton.gameObject.SetActive(true);
            replayButton.gameObject.SetActive(false);
        }


        public void HidePlayButton()
        {
            playButton.gameObject.SetActive(false);
        }


        public void InitPartButtons(List<RoverPartBehavior> parts)
        {
            playButton.gameObject.SetActive(false);
            replayButton.gameObject.SetActive(true);

            buttonsParent.DOAnchoredPosition(Vector3.zero, 0.5f).SetEasing(Ease.Type.SineOut);

            var partsWithButtons = parts.FindAll((part) =>
            {
                if (part != null && part.Data != null)
                {
                    return part.Data.HasButton;
                }
                return false;
            });

            var orderedParts = new Dictionary<RoverPart, List<RoverPartBehavior>>();

            for (int i = 0; i < partsWithButtons.Count; i++)
            {
                var part = partsWithButtons[i];
                var data = part.Data;

                if (orderedParts.ContainsKey(data))
                {
                    orderedParts[data].Add(part);
                }
                else
                {
                    var list = new List<RoverPartBehavior>();
                    list.Add(part);

                    orderedParts.Add(data, list);
                }
            }

            int counter = 0;

            foreach (var data in orderedParts.Keys)
            {
                var button = partButtons[counter];

                button.gameObject.SetActive(true);
                button.Init(data, orderedParts[data]);

                counter++;
            }

            for (int i = counter; i < partButtons.Count; i++)
            {
                partButtons[i].gameObject.SetActive(false);
            }
        }

        public void UpdateLevelText(int levelNumber)
        {
            levelText.text = $"LEVEL {levelNumber}";
        }

        #region Buttons
        public void HidePartButtons()
        {
            buttonsParent.DOAnchoredPosition(Vector2.down * buttonsParent.sizeDelta.y, 0.5f).SetEasing(Ease.Type.SineOut);
            replayButton.gameObject.SetActive(false);
        }

        public void PlayButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
            LevelController.LaunchVehicle();
        }

        public void RestartLevel()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
            GameController.ReplayCurrentLevel();
        }


        public void MainMenuButton()
        {
            AudioController.PlaySound(AudioController.AudioClips.buttonSound);
            GameController.ReturnToMainMenu();
        }
        #endregion
    }
}
