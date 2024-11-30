#pragma warning disable 649
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Watermelon
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    public class SpawnerTool : MonoBehaviour
    {
        private const float SIMULATION_STEP_TIME = 0.02f;
        [SerializeField] private GameObject container;
        [SerializeField] private SpawnData[] spawnData;

        public void Spawn()
        {
            float value = UnityEngine.Random.Range(0f, 1f);

            for (int i = 0; i < spawnData.Length; i++)
            {
                if (spawnData[i].chance >= value)
                {
                    GameObject gameObject = Instantiate(spawnData[i].prefab, transform.position.SetZ(0), Quaternion.identity, container.transform);
                    gameObject.name = spawnData[i].prefab.name + " ( Child # " + container.transform.childCount + ")";
                    return;
                }
                else
                {
                    value = value - spawnData[i].chance;
                }
            }


            if (spawnData.Length != 0)
            {
                AdjustChances();
                Spawn();
            }
        }

        public void AdjustChances()
        {
            float maxValue = 0;

            for (int i = 0; i < spawnData.Length; i++)
            {
                maxValue += spawnData[i].chance;
            }

            if (maxValue == 0)
            {
                for (int i = 0; i < spawnData.Length; i++)
                {
                    spawnData[i].chance = 1f / spawnData.Length;
                }
            }
            else
            {
                for (int i = 0; i < spawnData.Length; i++)
                {
                    spawnData[i].chance = spawnData[i].chance / maxValue;
                }
            }
        }

        public void StartPhysics()
        {
            Physics.simulationMode = SimulationMode.Script;
            StartCoroutine(PhysicsCoroutine());
        }

        IEnumerator PhysicsCoroutine()
        {
            while (Physics.simulationMode == SimulationMode.Script)
            {
                Physics.Simulate(SIMULATION_STEP_TIME);
                yield return new WaitForSeconds(SIMULATION_STEP_TIME);
            }
        }



        public void StopPhysics()
        {
            Physics.simulationMode = SimulationMode.FixedUpdate;
        }

        private void OnDrawGizmos()
        {
            if ((!Application.isPlaying) && (Physics.simulationMode == SimulationMode.Script))
            {
                EditorApplication.QueuePlayerLoopUpdate();
                SceneView.RepaintAll();
            }
        }

        private void OnDestroy()
        {
            StopPhysics();
        }

        [System.Serializable]
        public class SpawnData
        {
            [SerializeField] public GameObject prefab;
            [SerializeField] public float chance;
        }
    }
#endif
}
