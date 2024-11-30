#pragma warning disable 0414

using UnityEngine;
using UnityEngine.EventSystems;

namespace Watermelon
{
    public class UITouchHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private bool isMouseDown = false;

        private Transform selected;
        private SnapPoint activeSnap;

        [SerializeField] SlotsController slotsController;

        bool selectetIsPart = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            isMouseDown = true;

            var ray = CameraController.MainCamera.ScreenPointToRay(eventData.position);

            if (Physics.Raycast(ray, out var hit, 10, PhysicsHelper.GetLayerMask(PhysicsLayer.Pickable)))
            {
                selected = hit.transform;

                if (selected.CompareTag(PhysicsHelper.TAG_VISUALS))
                {
                    slotsController.GetFromSlot(selected);
                }
                else
                {
                    selectetIsPart = true;
                }
            }
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            isMouseDown = false;

            if (selected == null) return;

            if (!selectetIsPart)
            {
                if (activeSnap != null)
                {
                    AudioController.PlaySound(AudioController.AudioClips.assembleSound);
                    slotsController.PlacePart(selected, activeSnap);
                }
                else
                {
                    slotsController.ReturnToSlots(selected);
                }
            }
            else
            {

                if (slotsController != null)
                {
                    slotsController.RemovePart(selected, eventData.position);
                }
                
            }

            selectetIsPart = false;
            selected = null;
            activeSnap = null;
        }


        public void OnDrag(PointerEventData eventData)
        {
            if (selected == null) return;

            bool changed = false;

            if (selectetIsPart)
            {
                selected = slotsController.SwapToVisuals(selected, true);
                selectetIsPart = false;

                changed = true;
            }

            var ray = CameraController.MainCamera.ScreenPointToRay(eventData.position);

            MovePickable(ray, !changed);
            CheckSnapPoint(ray);
        }


        private void MovePickable(Ray cameraRay, bool lerp)
        {
            var k = (cameraRay.origin.z - (CameraController.MainCamera.transform.position.z - 2)) / cameraRay.direction.z;

            var position = cameraRay.origin + cameraRay.direction * k;

            if (lerp)
            {
                selected.position = Vector3.Lerp(selected.position, position, 0.25f);
            }
            else
            {
                selected.position = position;
            }

            selected.rotation = Quaternion.LookRotation((selected.position - CameraController.MainCamera.transform.position).normalized);
        }


        private void CheckSnapPoint(Ray cameraRay)
        {
            if (Physics.Raycast(cameraRay, out var hit, 10, PhysicsHelper.GetLayerMask(PhysicsLayer.Snap)))
            {
                if (activeSnap != null)
                {
                    if (activeSnap.gameObject != hit.transform.gameObject)
                    {
                        slotsController.DisactivateSnap(activeSnap);
                        activeSnap.MakeRed();

                        activeSnap = hit.transform.GetComponent<SnapPoint>();

                        slotsController.ActivateSnap(activeSnap);
                    }
                }
                else
                {
                    activeSnap = hit.transform.GetComponent<SnapPoint>();

                    slotsController.ActivateSnap(activeSnap);
                }
            }
            else
            {
                if (activeSnap != null)
                {
                    slotsController.DisactivateSnap(activeSnap);

                    activeSnap = null;
                }
            }
        }
    }
}