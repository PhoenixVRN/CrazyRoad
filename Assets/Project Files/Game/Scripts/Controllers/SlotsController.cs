using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class SlotsController : MonoBehaviour
    {
        [SerializeField] RectTransform slotsParent;
        [SerializeField] List<Image> slots;

        private List<GameObject> partVisuals = new List<GameObject>();
        private List<RoverPartData> parts = new List<RoverPartData>();

        private Dictionary<SnapPoint, Transform> activeSnaps = new Dictionary<SnapPoint, Transform>();

        TweenCase scaleCase;
        TweenCase moveCase;
        TweenCase rotationCase;
        Transform tweenVisuals;


        public void HideSlots()
        {
            foreach (var slot in slots)
            {
                slot.gameObject.SetActive(false);
            }

            foreach (var visual in partVisuals)
            {
                visual.gameObject.SetActive(false);
            }
        }


        public void InitSlots(RoverPartData[] partsData)
        {
            parts.Clear();

            for (int i = 0; i < partsData.Length; i++)
            {
                var partData = partsData[i];

                if (partData.IsPlacedAlready)
                {
                    LevelController.Rover.PlacePartPermanent(partData.Part, partData.Slot);
                }
                else
                {
                    parts.Add(partData);
                }
            }

            partVisuals.Clear();

            for (int i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];

                var isActive = i < parts.Count;

                slot.gameObject.SetActive(isActive);
            }

            Tween.DelayedCall(0.55f, () =>
            {
                for (int i = 0; i < parts.Count; i++)
                {
                    var visuals = LevelController.PoolsHandler.GetRoverpartVisuals(parts[i].Part);

                    visuals.transform.position = GetPositionInSlot(i);

                    visuals.transform.rotation = Quaternion.LookRotation((visuals.transform.position - CameraController.MainCamera.transform.position).normalized);

                    partVisuals.Add(visuals);
                }
            });

            activeSnaps.Clear();
        }


        public Transform SwapToVisuals(Transform pickablePart, bool immediately)
        {
            var part = pickablePart.GetComponent<RoverPartBehavior>();

            for (int i = 0; i < parts.Count; i++)
            {
                var data = parts[i].Part;
                var visuals = partVisuals[i];

                if (part.Data == data && !visuals.activeSelf)
                {
                    visuals.SetActive(true);

                    part.SnapPoint.SetActive(true);
                    part.SnapPoint.MakeRed();

                    activeSnaps.Remove(part.SnapPoint);

                    part.Disconect(immediately);

                    SetUpAvailableSlots(visuals.transform);

                    return visuals.transform;
                }
            }

            return pickablePart;
        }


        public void ReturnToSlots(Transform pickable)
        {
            if (tweenVisuals == pickable)
            {
                if (scaleCase != null && scaleCase.IsActive)
                    scaleCase.Kill();
                if (moveCase != null && moveCase.IsActive)
                    moveCase.Kill();
                if (rotationCase != null && rotationCase.IsActive)
                    rotationCase.Kill();
            }
            tweenVisuals = pickable;

            var index = partVisuals.IndexOf(pickable.gameObject);
            var slotPos = GetPositionInSlot(index);
            var rotation = Quaternion.LookRotation((slotPos - CameraController.MainCamera.transform.position).normalized);

            moveCase = pickable.DOMove(slotPos, 0.5f).SetEasing(Ease.Type.SineInOut);
            scaleCase = pickable.DOScale(1f, 0.5f).SetEasing(Ease.Type.SineInOut);
            rotationCase = pickable.DORotate(rotation, 0.5f).SetEasing(Ease.Type.SineInOut);

            ResetAvailableSnaps();
        }


        public void GetFromSlot(Transform pickable)
        {
            if (tweenVisuals == pickable)
            {
                if (scaleCase != null && scaleCase.IsActive)
                    scaleCase.Kill();
                if (moveCase != null && moveCase.IsActive)
                    moveCase.Kill();
                if (rotationCase != null && rotationCase.IsActive)
                    rotationCase.Kill();
            }
            tweenVisuals = pickable;

            scaleCase = pickable.DOScale(1.2f, 0.5f).SetEasing(Ease.Type.SineInOut);

            SetUpAvailableSlots(pickable);
        }


        public void SetUpAvailableSlots(Transform pickable)
        {
            float delay = 0f;

            if (parts.Count != partVisuals.Count)
                delay = 0.56f;

            Tween.DelayedCall(delay, () =>
            {
                var index = partVisuals.IndexOf(pickable.gameObject);

                var data = parts[index];

                for (int i = 0; i < LevelController.Rover.AvailableSnapPoints.Count; i++)
                {
                    var snap = LevelController.Rover.AvailableSnapPoints[i];

                    if (data.AlowedSlots.Contains(snap.Id))
                    {
                        if (!activeSnaps.ContainsKey(snap))
                        {
                            snap.EnableCollider();
                            snap.SetActive(true);
                            snap.MakeRed();
                        }
                        else
                        {
                            snap.EnableCollider();
                        }

                    }
                    else
                    {
                        if (!activeSnaps.ContainsKey(snap))
                        {
                            snap.DisableCollider();
                            snap.SetActive(false);
                        }
                        else
                        {
                            snap.DisableCollider();
                        }
                    }
                }
            });

        }


        public void ResetAvailableSnaps()
        {
            for (int i = 0; i < LevelController.Rover.AvailableSnapPoints.Count; i++)
            {
                var snap = LevelController.Rover.AvailableSnapPoints[i];

                if (!activeSnaps.ContainsKey(snap))
                {
                    snap.EnableCollider();
                    snap.SetActive(true);
                    snap.MakeRed();
                }

                if (activeSnaps.Count == LevelController.Rover.AvailableSnapPoints.Count)
                {
                    UIGame gameUI = UIController.GetPage<UIGame>();
                    gameUI.ShowPlayButton();
                }
            }
        }


        public void PlacePart(Transform pickable, SnapPoint snap)
        {
            var index = partVisuals.IndexOf(pickable.gameObject);

            var data = parts[index].Part;

            if (activeSnaps.ContainsKey(snap))
            {
                var visuals = SwapToVisuals(activeSnaps[snap], true);

                visuals.transform.position = pickable.transform.position;
                visuals.transform.rotation = pickable.transform.rotation;
                visuals.transform.localScale = pickable.transform.localScale;

                ReturnToSlots(visuals);

                activeSnaps.Remove(snap);

                snap.SetActive(false);
            }

            pickable.gameObject.SetActive(false);

            var part = LevelController.Rover.PlacePart(data, snap);

            activeSnaps.Add(snap, part.transform);

            ResetAvailableSnaps();
        }


        public void RemovePart(Transform part, Vector2 presPosition)
        {
            SnapPoint snap = null;

            foreach (var key in activeSnaps.Keys)
            {
                if (activeSnaps[key] == part)
                {
                    snap = key;
                    break;
                }
            }

            if (snap != null)
            {
                var visuals = SwapToVisuals(activeSnaps[snap], false);

                var cameraRay = CameraController.MainCamera.ScreenPointToRay(presPosition);

                var k = (cameraRay.origin.z - (CameraController.MainCamera.transform.position.z - 2)) / cameraRay.direction.z;

                var position = cameraRay.origin + cameraRay.direction * k;

                visuals.transform.position = position;
                visuals.transform.rotation = Quaternion.LookRotation((position - CameraController.MainCamera.transform.position).normalized);
                visuals.transform.localScale = Vector3.zero;

                visuals.transform.DOScale(1.2f, 0.4f).SetEasing(Ease.Type.SineOut).OnComplete(() =>
                {
                    ReturnToSlots(visuals);

                    activeSnaps.Remove(snap);

                    snap.SetActive(true);
                    snap.MakeRed();
                });
            }

            ResetAvailableSnaps();
        }


        private Vector3 GetPositionInSlot(int slotId)
        {
            var parentHeight = slotsParent.sizeDelta.y;

            var slotCenterPosition = slots[slotId].rectTransform.anchoredPosition + new Vector2(0, parentHeight + slots[slotId].rectTransform.rect.height / 2);

            var viewportPoint = slotCenterPosition / UIController.MainCanvas.GetComponent<RectTransform>().sizeDelta;

            var ray = CameraController.MainCamera.ViewportPointToRay(viewportPoint);

            var k = (ray.origin.z - (CameraController.MainCamera.transform.position.z - 2)) / ray.direction.z;

            var positionInSlot = ray.origin + ray.direction * k;

            return positionInSlot;
        }


        public void ActivateSnap(SnapPoint snap)
        {
            snap.MakeGreen();

            if (activeSnaps.ContainsKey(snap))
            {
                snap.SetActive(true);
                activeSnaps[snap].gameObject.SetActive(false);
            }
        }


        public void DisactivateSnap(SnapPoint snap)
        {
            snap.MakeRed();

            if (activeSnaps.ContainsKey(snap))
            {
                snap.SetActive(false);
                activeSnaps[snap].gameObject.SetActive(true);
            }
        }
    }
}