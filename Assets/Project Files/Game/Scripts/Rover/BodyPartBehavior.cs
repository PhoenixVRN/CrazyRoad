using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class BodyPartBehavior : RoverPartBehavior
    {
        [SerializeField] List<SnapPoint> snapPoints;

        public List<SnapPoint> SnapPoints => snapPoints;

        [Space]
        [SerializeField] Transform centerOfMass;
        public Transform CenterOfMass => centerOfMass;

        [SerializeField] Collider2D roverCollider;


        public override void Connect(RoverPart data, RoverBehavior master, SnapPoint point)
        {
            base.Connect(data, master, point);

            foreach (var snapPoint in SnapPoints)
            {
                snapPoint.Init();
                snapPoint.MakeRed();
                snapPoint.SetActive(true);
                snapPoint.EnableCollider();
            }
        }


        public override void Disconect(bool immediately)
        {
            base.Disconect(immediately);
        }


        public void DisableSnapPoints()
        {
            foreach (var point in snapPoints)
            {
                point.gameObject.SetActive(false);
            }
        }


        public override void Explode()
        {
            base.Explode();

            if (roverCollider != null)
                roverCollider.enabled = false;
        }


        public void HideSlots()
        {
            SnapPoints.ForEach(snap => snap.DisableCollider());
        }
    }
}