using System.Collections;
using UnityEngine;

namespace Watermelon
{
    public class CoinBehavior : MonoBehaviour
    {
        public static readonly int IDLE_ANIM = Animator.StringToHash("Idle");

        [SerializeField] Transform coinVisuals;

        private BoxCollider2D colliderRef;
        private Coroutine rotationCoroutine;

        private IPool coinsParticlePool;

        private void Awake()
        {
            colliderRef = GetComponent<BoxCollider2D>();
            coinsParticlePool = PoolManager.GetPoolByName("CoinPickUpParticle");
        }

        private void OnEnable()
        {
            colliderRef.isTrigger = true;
            colliderRef.enabled = true;

            rotationCoroutine = StartCoroutine(RotationCoroutine());
        }

        public void Init(Vector3 position, float time)
        {
            transform.localScale = Vector3.one;
            transform.position = position;
        }


        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == (int)PhysicsLayer.Rover)
            {
                var rover = collision.GetComponent<RoverBehavior>();

                if (rover != null)
                {
#if MODULE_HAPTIC
                    Haptic.Play(Haptic.HAPTIC_LIGHT);
#endif

                    coinsParticlePool.GetPooledObject().SetPosition(transform.position);
                    OnCoinPicked();
                }
            }
        }


        private void OnCoinPicked()
        {
            colliderRef.isTrigger = false;
            colliderRef.enabled = false;

            AudioController.PlaySound(AudioController.AudioClips.coinSound);

            transform.DOPushScale(Vector3.one * 1.2f, Vector3.zero, 0.05f, 0.15f, Ease.Type.BackOut, Ease.Type.BackIn).OnComplete(delegate
            {
                if (rotationCoroutine != null)
                {
                    StopCoroutine(rotationCoroutine);
                    rotationCoroutine = null;
                }

                gameObject.SetActive(false);
            });

            GameController.OnCoinPicked();
        }


        private IEnumerator RotationCoroutine()
        {
            float axisY = transform.localRotation.y;

            while (true)
            {
                if (axisY >= 360f)
                    axisY = 0;

                axisY++;

                coinVisuals.localRotation = Quaternion.Euler(0f, axisY, 0f);

                yield return Time.deltaTime;
            }
        }
    }
}