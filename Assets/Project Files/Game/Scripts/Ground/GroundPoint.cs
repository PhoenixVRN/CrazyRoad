using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class GroundPoint
    {
        public Vector3 position;
        public Vector3 leftKey;
        public Vector3 rightKey;

        public bool battery = false;
    }
}