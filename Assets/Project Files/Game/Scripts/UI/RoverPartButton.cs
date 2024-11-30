using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class RoverPartButton : MonoBehaviour
    {
        [SerializeField] Image iconImage;
        [SerializeField] Shadow buttonShadow;
        [SerializeField] Button button;
        [SerializeField] Image toggleBottomImage;
        [SerializeField] Image progressImage;

        public List<RoverPartBehavior> Parts { get; private set; }
        public RoverPart Data { get; private set; }

        public int partCount;

        public bool IsPressed { get; private set; }


        public void Init(RoverPart data, List<RoverPartBehavior> roverParts)
        {

            Parts = roverParts;

            Data = data;

            iconImage.sprite = Data.ButtonSprite;
            iconImage.color = Data.IconColor;
            buttonShadow.effectColor = Data.ShadowColor;

            if (Data.IsToggle)
            {
                toggleBottomImage.enabled = true;
                toggleBottomImage.color = Data.AdditionalColor;

                button.image.rectTransform.anchoredPosition = Vector2.up * 15;

                progressImage.enabled = false;
            }
            else
            {
                toggleBottomImage.enabled = false;
                button.image.rectTransform.anchoredPosition = Vector2.zero;

                progressImage.enabled = false;
                progressImage.color = Data.AdditionalColor;
            }

            IsPressed = false;

            partCount = Parts.Count;

        }


        public void OnClick()
        {
            if (!Data.IsToggle)
            {
                foreach (var part in Parts)
                {
                    part.OnButtonClick();
                }
            }
        }


        public void OnPressed()
        {
            if (!button.enabled)
                return;


            if (Data.IsToggle)
            {
                IsPressed = !IsPressed;

                if (IsPressed)
                {
                    AudioController.PlaySound(AudioController.AudioClips.powerupSound);

                    buttonShadow.enabled = false;
                    button.image.rectTransform.anchoredPosition = Vector2.zero;

                    foreach (var part in Parts)
                    {
                        part.OnButtonPressed();
                    }
                }
                else
                {
                    buttonShadow.enabled = true;
                    button.image.rectTransform.anchoredPosition = Vector2.up * 15;

                    foreach (var part in Parts)
                    {
                        part.OnButtonReleased();
                    }
                }
            }
            else
            {
                AudioController.PlaySound(AudioController.AudioClips.powerupSound);

                buttonShadow.enabled = false;
                button.image.rectTransform.anchoredPosition = Vector2.zero;

                foreach (var part in Parts)
                {
                    part.OnButtonPressed();
                }
            }
        }


        public void OnReleased()
        {
            if (!button.enabled)
                return;

            if (!Data.IsToggle)
            {
                buttonShadow.enabled = true;
                button.image.rectTransform.anchoredPosition = Vector2.up * 15;

                if (Data.HasCooldown)
                {
                    button.enabled = false;
                    progressImage.enabled = true;
                    progressImage.fillAmount = 1;

                    progressImage.DOFillAmount(0, Data.CooldownTime).OnComplete(() =>
                    {
                        button.enabled = true;
                        progressImage.enabled = false;
                    });
                }

                foreach (var part in Parts)
                {
                    part.OnButtonReleased();
                }
            }


        }
    }
}