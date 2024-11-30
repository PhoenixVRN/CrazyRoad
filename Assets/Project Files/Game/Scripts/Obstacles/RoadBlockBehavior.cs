using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    public class RoadBlockBehavior : MonoBehaviour, IInitialized
    {
        [SerializeField] List<Rigidbody> parts;
        [SerializeField] List<GameObject> visuals;

        [SerializeField] List<Rigidbody> partsToHide;

        [SerializeField] Collider2D trigger;

        private List<Vector3> savePositions;
        private List<Vector3> saveRotations;


        private void Awake()
        {
            savePositions = new List<Vector3>();
            saveRotations = new List<Vector3>();

            partsToHide.ForEach(part => part.gameObject.SetActive(false));

            for (int i = 0; i < parts.Count; i++)
            {
                savePositions.Add(parts[i].transform.localPosition);
                saveRotations.Add(parts[i].transform.localEulerAngles);
            }

        }


        public void Init()
        {
            trigger.enabled = true;

            visuals.ForEach(visual => visual.SetActive(true));

            for (int i = 0; i < parts.Count; i++)
            {
                var cup = parts[i];

                if (!cup.isKinematic)
                {
                    cup.linearVelocity = Vector3.zero;
                    cup.angularVelocity = Vector3.zero;
                }

                cup.isKinematic = true;
                cup.useGravity = false;

                cup.transform.localPosition = savePositions[i];
                cup.transform.localEulerAngles = saveRotations[i];
            }
        }

        public static bool CompareLayer(GameObject obj, PhysicsLayer layer)
        {
            return obj.layer == (int)layer;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (CompareLayer(collision.gameObject, PhysicsLayer.Rover))
            {
                var rover = collision.GetComponent<RoverBehavior>();

                if (rover != null)
                {
                    visuals.ForEach(visual => visual.SetActive(false));

                    var impactVelocity = rover.Rigidbody.linearVelocity;
                    var impactSpeed = impactVelocity.magnitude;

                    var impactPoint = collision.ClosestPoint(rover.Rigidbody.worldCenterOfMass + Vector2.down * 0.5f);

                    var impactOnRover = -impactVelocity.normalized * 5;
                    var speedMultiplier = Mathf.Clamp01(Mathf.InverseLerp(10, 0, impactSpeed));

                    if (speedMultiplier > 0)
                    {
                        impactOnRover *= speedMultiplier;
                        rover.AddForce(impactOnRover, impactPoint, ForceMode2D.Impulse);
                    }


                    foreach (var cup in parts)
                    {
                        cup.gameObject.SetActive(true);
                        cup.isKinematic = false;
                        cup.useGravity = true;

                        var vectorToCup = cup.position - ((Vector3)rover.Rigidbody.worldCenterOfMass + Vector3.down * 0.5f).SetZ(rover.transform.position.z);

                        var forceMagnitude = vectorToCup.normalized * impactSpeed;
                        forceMagnitude *= Mathf.Clamp01(Mathf.InverseLerp(5, 0, vectorToCup.magnitude)) * (Mathf.Lerp(2, 1, speedMultiplier));

                        var forcePosition = ((Vector3)impactPoint).SetZ(rover.transform.position.z) + Random.insideUnitSphere * 0.2f;

                        cup.AddForceAtPosition(forceMagnitude, forcePosition, ForceMode.Impulse);
                    }
                }
            }

        }
    }

}