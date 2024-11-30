using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [DefaultExecutionOrder(1)]
    public class RoverBehavior : MonoBehaviour
    {

        [SerializeField] float airResistanceMultiplier = 0.1f;
        [SerializeField] float torqueResistanceMultiplier = 0.1f;

        [Space]
        [SerializeField] UIGame gameUI;

        private  Rigidbody2D rigidbodyRef;
        public Rigidbody2D Rigidbody => rigidbodyRef;

        public BodyPartBehavior Body { get; private set; }

        public List<WheelPartBehavior> wheels = new List<WheelPartBehavior>();
        public List<RoverPartBehavior> parts = new List<RoverPartBehavior>();


        public bool IsRunning { get; private set; }

        public Vector2 Acceleration { get; private set; }

        public Vector2 Gravity { get; set; }

        Vector3 nearest;

        private List<SnapPoint> availableSnapPoints = new List<SnapPoint>();
        public List<SnapPoint> AvailableSnapPoints => availableSnapPoints;
        private List<ForceData> forces = new List<ForceData>();

        public float AirResistanceMult { get; set; }


        private void Awake()
        {
            rigidbodyRef = GetComponent<Rigidbody2D>();

            IsRunning = false;

            Gravity = Vector2.down * 9.81f;

            AirResistanceMult = 1;
        }


        public void AddForce(Vector2 force, Vector2 position, ForceMode2D mode = ForceMode2D.Force)
        {
            forces.Add(new ForceData { force = force, withPosition = true, position = position, mode = mode });
        }


        public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force)
        {
            forces.Add(new ForceData { force = force, withPosition = false, position = (Vector2)transform.position + Rigidbody.centerOfMass, mode = mode });
        }


        private void FixedUpdate()
        {
            if (IsRunning)
            {
                foreach (var wheel in wheels)
                {
                    wheel.DriveUpdate();
                }

                var direction = Rigidbody.linearVelocity.normalized;
                var speedSquare = Rigidbody.linearVelocity.sqrMagnitude;

                var forcesMagnitudeSum = 0f;

                foreach (var force in forces)
                {
                    forcesMagnitudeSum += force.force.magnitude;
                }

                var airResistance = -direction * speedSquare * airResistanceMultiplier * AirResistanceMult;

                AddForce(airResistance);

                var airTorqueResistance = -Rigidbody.angularVelocity * torqueResistanceMultiplier;

                AddForce(Gravity * Rigidbody.mass);

                for (int i = 0; i < forces.Count; i++)
                {
                    var force = forces[i];

                    if (force.withPosition)
                    {
                        Rigidbody.AddForceAtPosition(force.force, force.position, force.mode);
                    }
                    else
                    {
                        Rigidbody.AddForce(force.force, force.mode);
                    }


                }

                forces.Clear();

                Rigidbody.AddTorque(airTorqueResistance);


                var groundCollider = Physics2D.OverlapCircle(transform.position, 2f, PhysicsHelper.GetLayerMask(PhysicsLayer.Ground));
                if (groundCollider != null)
                {
                    var ground = groundCollider.GetComponent<GroundRenderer>();

                    if (ground != null)
                    {
                        nearest = ground.GetNearestPoint(transform.position);

                        transform.position = transform.position.SetZ(nearest.z);

                    }
                }

                CameraController.SetOffset(Mathf.InverseLerp(0, 200, Rigidbody.linearVelocity.sqrMagnitude) * 0.1f + 1);

            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(nearest, 0.3f);
        }


        public void SetBody(BodyPartBehavior bodyPart, RoverPart data, int[] activeSlots)
        {
            transform.rotation = Quaternion.identity;

            Body = bodyPart;

            Body.SetLayer(PhysicsLayer.Default);

            Body.transform.SetParent(transform);

            Body.transform.localPosition = Vector3.zero;
            Body.transform.localRotation = Quaternion.identity;

            Body.Connect(data, this, null);

            rigidbodyRef.centerOfMass = Body.CenterOfMass.localPosition;

            AvailableSnapPoints.Clear();

            availableSnapPoints.AddRange(Body.SnapPoints);

            for (int i = 0; i < availableSnapPoints.Count; i++)
            {
                var point = availableSnapPoints[i];
                point.Id = i;
                point.MakeRed();
                point.EnableCollider();
                point.SetActive(true);
            }

            for (int i = 0; i < availableSnapPoints.Count; i++)
            {
                var point = availableSnapPoints[i];

                bool shouldDelete = true;
                for (int j = 0; j < activeSlots.Length; j++)
                {
                    if (point.Id == activeSlots[j])
                    {
                        shouldDelete = false;
                        break;
                    }
                }

                if (shouldDelete)
                {
                    availableSnapPoints.Remove(point);
                    point.DisableCollider();
                    point.SetActive(false);
                    i--;
                }
            }
        }


        public RoverPartBehavior PlacePart(RoverPart partData, SnapPoint snap)
        {
            var part = LevelController.PoolsHandler.GetRoverPartBehavior(partData);

            snap.SetActive(false);

            part.transform.SetParent(transform);

            part.Connect(partData, this, snap);

            if (part.Data.Type == RoverPartType.Wheel)
            {
                wheels.Add(part as WheelPartBehavior);
            }

            parts.Add(part);

            part.SetLayer(PhysicsLayer.Default);

            return part;
        }


        public void PlacePartPermanent(RoverPart partData, int pointId)
        {
            var part = LevelController.PoolsHandler.GetRoverPartBehavior(partData);

            var snap = Body.SnapPoints[pointId];

            snap.DisableCollider();
            snap.SetActive(false);

            part.transform.SetParent(transform);

            part.Connect(partData, this, snap);

            part.SetLayer(PhysicsLayer.Default);

            if (part.Data.Type == RoverPartType.Wheel)
            {
                wheels.Add(part as WheelPartBehavior);
            }

            parts.Add(part);

            part.DisablePickableCollider();

            availableSnapPoints.Remove(snap);
        }


        public void Disconect(RoverPartBehavior part)
        {
            if (part.Data.Type == RoverPartType.Wheel)
            {
                var wheel = part as WheelPartBehavior;

                if (wheels.Contains(wheel))
                {
                    wheels.Remove(wheel);
                }

                parts.Remove(part);
            }
        }


        public void Drive()
        {
            rigidbodyRef.simulated = true;
            rigidbodyRef.linearVelocity = Vector2.zero;
            rigidbodyRef.angularVelocity = 0;

            foreach (var wheel in wheels)
            {
                wheel.Rigidbody.simulated = true;
            }

            IsRunning = true;

            forces.Clear();

            AddForce(transform.right * 5f, ForceMode2D.Impulse);

            foreach (var part in parts)
            {
                part.SetLayer(PhysicsLayer.RoverVisuals);
            }

            Body.SetLayer(PhysicsLayer.RoverVisuals);
            Body.HideSlots();

            AirResistanceMult = 1;
        }


        public void Finish(Transform finishPoint)
        {
            rigidbodyRef.simulated = false;

            foreach (var wheel in wheels)
            {
                wheel.Rigidbody.simulated = false;
            }

            transform.DOMove(finishPoint.position, 0.5f, 0, false, UpdateMethod.FixedUpdate).SetEasing(Ease.Type.SineOut);
            transform.DORotate(Quaternion.identity, 0.5f, 0, false, UpdateMethod.FixedUpdate).SetEasing(Ease.Type.SineOut);

            IsRunning = false;
        }


        public void ResetRover()
        {
            rigidbodyRef.simulated = false;

            foreach (var wheel in wheels)
            {
                wheel.Rigidbody.simulated = false;
            }

            wheels.Clear();

            RoverPartBehavior[] temporaryList = new RoverPartBehavior[parts.Count];
            parts.CopyTo(temporaryList);

            foreach (var part in temporaryList)
            {
                part.Disconect(true);
            }

            parts.Clear();

            transform.position = Vector3.zero;
        }


        public void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == (int)PhysicsLayer.Floor)
            {
                Rigidbody.simulated = false;
                foreach (var part in parts)
                {
                    part.Explode();
                }

                Body.Explode();

#if MODULE_HAPTIC
                Haptic.Play(Haptic.HAPTIC_MEDIUM);
#endif

                AudioController.PlaySound(AudioController.AudioClips.loseSound);

                UIController.HidePage<UIGame>();
                UIController.ShowPage<UIGameOver>();

                gameUI.HidePartButtons();
            }
        }
    }

    public class ForceData
    {
        public Vector2 force;
        public bool withPosition;
        public Vector2 position;
        public ForceMode2D mode;
    }
}