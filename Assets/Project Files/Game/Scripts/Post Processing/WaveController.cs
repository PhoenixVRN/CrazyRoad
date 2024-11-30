using UnityEngine;

namespace Watermelon
{
    public class WaveController : MonoBehaviour
    {
        private static WaveController instance;

        private static readonly int DISTANCE_ID = Shader.PropertyToID("_Distance");
        private static readonly int Center_ID = Shader.PropertyToID("_Center");

        [SerializeField] Material waveMaterial;

        public static Material WaveMaterial => instance.waveMaterial;


        private void Awake()
        {
            instance = this;
        }


        public static void SendWave(Vector2 center, float duration)
        {
            WaveMaterial.SetVector(Center_ID, center);

            WaveMaterial.SetFloat(DISTANCE_ID, 0);

            WaveMaterial.DoFloat(DISTANCE_ID, 2, duration);
        }
    }
}