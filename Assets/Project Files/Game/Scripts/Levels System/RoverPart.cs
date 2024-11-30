using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Rover Part 001", menuName = "Data/Rover Part")]
    public class RoverPart : ScriptableObject
    {
        [SerializeField] RoverPartType type;
        [SerializeField] GameObject partObject;
        [SerializeField] GameObject buildVisuals;

        [Space]
        [SerializeField] bool hasButton;
        [SerializeField] bool isToggle;
        [SerializeField] Sprite buttonSprite;
        [SerializeField] Sprite toggleSprite;
        [SerializeField] Color iconColor;
        [SerializeField] Color shadowColor;
        [SerializeField] Color additionalColor;

        [Space]
        [SerializeField] bool hasCooldown = false;
        [SerializeField] float cooldownTime = 0f;

        [Space]
        [SerializeField] bool isPerSecond;

        public RoverPartType Type => type;
        public GameObject PartObject => partObject;
        public GameObject BuildVisuals => buildVisuals;

        public bool HasButton => hasButton;
        public bool IsToggle => isToggle;
        public Sprite ButtonSprite => buttonSprite;
        public Sprite ToggleSprite => toggleSprite;

        public Color IconColor => iconColor;
        public Color ShadowColor => shadowColor;
        public Color AdditionalColor => additionalColor;

        public bool HasCooldown => hasCooldown;
        public float CooldownTime => cooldownTime;

        public bool IsPerSecond => isPerSecond;
    }
}