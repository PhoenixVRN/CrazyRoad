using UnityEngine;

namespace Watermelon.VehicleFactory
{
    [System.Serializable]
    public class EnvironmentSkinData : AbstractSkinData
    {
        [SkinPreview]
        [SerializeField] Sprite previewSprite;
        public Sprite PreviewSprite => previewSprite;

        [SerializeField] Material material;
        public Material Material => material;
    }
}