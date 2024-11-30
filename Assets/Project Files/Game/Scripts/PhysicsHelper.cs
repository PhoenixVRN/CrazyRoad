using UnityEngine;

namespace Watermelon
{
    public static class PhysicsHelper
    {
        public static readonly int LAYER_DEFAULT = LayerMask.NameToLayer("Default");
        public static readonly int LAYER_PLAYER = LayerMask.NameToLayer("Player");

        public const string TAG_PLAYER = "Player";

        public static readonly string TAG_VISUALS = "Visuals";
        public static readonly string TAG_PART = "Part";


        public static int GetLayerMask(PhysicsLayer layer)
        {
            return (int)Mathf.Pow(2, (int)layer);
        }


        public static bool CompareLayer(MonoBehaviour behavior, PhysicsLayer layer)
        {
            return behavior.gameObject.layer == (int)layer;
        }


        public static bool CompareLayer(GameObject obj, PhysicsLayer layer)
        {
            return obj.layer == (int)layer;
        }

        public static void Init()
        {

        }
    }
}