#pragma warning disable 649
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [ExecuteInEditMode]
    public class BezierEditorTool : MonoBehaviour
    {
#if UNITY_EDITOR
        private static BezierEditorTool instance;
        private int selectedBezierGroupIndex;
        private int selectedBezierSliceIndex;
        private List<BezierGroup> groups;

        [SerializeField] float bezierLineHalfWidth;
        [SerializeField] Color selectedSliceColor;
        [SerializeField] Color selectedBezierColor;
        [SerializeField] Color unselectedBezierColor;
        [SerializeField] GroundRenderer groundRendererPrefab;
        [SerializeField] Vector3 groupPositionHandlesOffset;

        [Header("Coin configuartion")]
        [SerializeField] float coinRadius;
        [SerializeField] Color coinColor;
        [SerializeField] Vector3 coinOffset;



        public static BezierEditorTool Instance { get => instance; }
        public List<BezierGroup> Groups { get => groups; set => groups = value; }
        public int SelectedBezierGroupIndex { get => selectedBezierGroupIndex; set => selectedBezierGroupIndex = value; }
        public int SelectedBezierSliceIndex { get => selectedBezierSliceIndex; set => selectedBezierSliceIndex = value; }
        public float BezierLineHalfWidth => bezierLineHalfWidth;
        public Color SelectedSliceColor => selectedSliceColor;
        public Color SelectedBezierColor => selectedBezierColor;
        public Color UnselectedBezierColor => unselectedBezierColor;

        public GroundRenderer GroundRendererPrefab => groundRendererPrefab;

        public float CoinRadius => coinRadius;
        public Color CoinColor => coinColor;
        public Vector3 CoinOffset => coinOffset;

        public Vector3 GroupPositionHandlesOffset => groupPositionHandlesOffset;

        public BezierEditorTool()
        {
            instance = this;
            Groups = new List<BezierGroup>();
        }

        public void Clear()
        {
            Groups.Clear();
            selectedBezierGroupIndex = -1;
            selectedBezierSliceIndex = -1;
        }


        public class BezierGroup
        {
            public Vector3 position;
            public List<BezierSlice> slices;
            public bool startTape;
            public bool endTape;

            public BezierGroup()
            {
            }

            public BezierGroup(Vector3 position)
            {
                this.position = position;
                slices = new List<BezierSlice>();
            }
        }

        public class BezierSlice
        {
            public Vector3 point0;
            public Vector3 point1;
            public Vector3 point2;
            public Vector3 point3;
            public bool battery;

            public BezierSlice()
            {
            }

            public BezierSlice(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3, bool battery = false)
            {
                this.point0 = point0;
                this.point1 = point1;
                this.point2 = point2;
                this.point3 = point3;
                this.battery = battery;
            }
        }


#endif
    }
}
