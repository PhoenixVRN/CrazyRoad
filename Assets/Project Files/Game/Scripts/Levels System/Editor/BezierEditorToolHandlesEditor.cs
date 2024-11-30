using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Watermelon;

namespace Watermelon.VehicleFactory
{
    [CustomEditor(typeof(BezierEditorTool))]
    public class BezierEditorToolHandlesEditor : Editor
    {
        static List<Vector3> tempSteps;
        static List<Vector3> tempPoligon;
        static Vector3[] drawPoligons;
        static BezierEditorTool.BezierSlice tempSlice;
        static Vector3 groupPosition;
        static float angle;
        static Quaternion rotation;
        static Vector3 forward;
        static Vector3 point;
        static Vector3 tempPoint0;
        static Vector3 tempPoint1;
        static Vector3 tempPoint2;
        static Vector3 tempPoint3;
        private static BezierEditorTool.BezierSlice bezierSlice;
        private static float length;
        private static int batteriesCount;
        private static Vector3 batteryPosition;
        private static bool editorIsSelected;
        private static bool alreadySubscribed;

        private void OnEnable()
        {
            tempSteps = new List<Vector3>();
            tempPoligon = new List<Vector3>();
            drawPoligons = new Vector3[4];

        }

        private void OnDisable()
        {
            if (!alreadySubscribed)
            {
                SceneView.duringSceneGui += DrawUnselectedHandles;
            }

            editorIsSelected = false;
            alreadySubscribed = true;
        }

        private void DrawUnselectedHandles(SceneView obj)
        {
            if (editorIsSelected)
            {
                return;
            }

            Color backupColor = Handles.color;

            for (int i = 0; i < BezierEditorTool.Instance.Groups.Count; i++)
            {
                groupPosition = BezierEditorTool.Instance.Groups[i].position;

                for (int j = 0; j < BezierEditorTool.Instance.Groups[i].slices.Count; j++)
                {
                    DrawBezierGroup(groupPosition, i, BezierEditorTool.Instance.SelectedBezierColor);
                }
            }

            Handles.color = backupColor;
        }

        private void DrawHandles()
        {
            Color backupColor = Handles.color;

            for (int i = 0; i < BezierEditorTool.Instance.Groups.Count; i++)
            {
                rotation = Quaternion.identity;
                groupPosition = BezierEditorTool.Instance.Groups[i].position;

                for (int j = 0; j < BezierEditorTool.Instance.Groups[i].slices.Count; j++)
                {
                    tempSlice = BezierEditorTool.Instance.Groups[i].slices[j];


                    if (BezierEditorTool.Instance.SelectedBezierGroupIndex == i)
                    {
                        var pos = Handles.PositionHandle(BezierEditorTool.Instance.Groups[i].position + BezierEditorTool.Instance.GroupPositionHandlesOffset, Quaternion.identity) - BezierEditorTool.Instance.GroupPositionHandlesOffset;

                        pos.x = Mathf.Round(pos.x * 10) / 10;
                        pos.y = Mathf.Round(pos.y * 10) / 10;
                        pos.z = Mathf.Round(pos.z * 10) / 10;

                        BezierEditorTool.Instance.Groups[i].position = pos;

                        Handles.Label(BezierEditorTool.Instance.Groups[i].position + BezierEditorTool.Instance.GroupPositionHandlesOffset, "Group Position");
                        Handles.Label(BezierEditorTool.Instance.Groups[i].position + BezierEditorTool.Instance.GroupPositionHandlesOffset + Vector3.down * 0.25f, BezierEditorTool.Instance.Groups[i].position.ToString());
                        groupPosition = BezierEditorTool.Instance.Groups[i].position;

                        if (BezierEditorTool.Instance.SelectedBezierSliceIndex == j)
                        {

                            tempPoint0 = Handles.PositionHandle(tempSlice.point0 + groupPosition, Quaternion.identity) - groupPosition;
                            tempPoint0.x = Mathf.Round(tempPoint0.x * 10) / 10;
                            tempPoint0.y = Mathf.Round(tempPoint0.y * 10) / 10;
                            tempPoint0.z = Mathf.Round(tempPoint0.z * 10) / 10;
                            tempSlice.point0 = tempPoint0;

                            tempPoint1 = Handles.PositionHandle(tempSlice.point1 + groupPosition, Quaternion.identity) - groupPosition;
                            tempPoint1.x = Mathf.Round(tempPoint1.x * 10) / 10;
                            tempPoint1.y = Mathf.Round(tempPoint1.y * 10) / 10;
                            tempPoint1.z = Mathf.Round(tempPoint1.z * 10) / 10;
                            tempSlice.point1 = tempPoint1;

                            tempPoint2 = Handles.PositionHandle(tempSlice.point2 + groupPosition, Quaternion.identity) - groupPosition;
                            tempPoint2.x = Mathf.Round(tempPoint2.x * 10) / 10;
                            tempPoint2.y = Mathf.Round(tempPoint2.y * 10) / 10;
                            tempPoint2.z = Mathf.Round(tempPoint2.z * 10) / 10;
                            tempSlice.point2 = tempPoint2;

                            tempPoint3 = Handles.PositionHandle(tempSlice.point3 + groupPosition, Quaternion.identity) - groupPosition;
                            tempPoint3.x = Mathf.Round(tempPoint3.x * 10) / 10;
                            tempPoint3.y = Mathf.Round(tempPoint3.y * 10) / 10;
                            tempPoint3.z = Mathf.Round(tempPoint3.z * 10) / 10;
                            tempSlice.point3 = tempPoint3;
                            Handles.Label(tempSlice.point0 + groupPosition, "Point 0");
                            Handles.Label(tempSlice.point1 + groupPosition, "Point 1");
                            Handles.Label(tempSlice.point2 + groupPosition, "Point 2");
                            Handles.Label(tempSlice.point3 + groupPosition, "Point 3");

                            Handles.Label(tempSlice.point0 + groupPosition + Vector3.down * 0.5f, tempSlice.point0.ToString());
                            Handles.Label(tempSlice.point1 + groupPosition + Vector3.down * 0.5f, tempSlice.point1.ToString());
                            Handles.Label(tempSlice.point2 + groupPosition + Vector3.down * 0.5f, tempSlice.point2.ToString());
                            Handles.Label(tempSlice.point3 + groupPosition + Vector3.down * 0.5f, tempSlice.point3.ToString());


                            //in slices we reuse start and finish point so we need to set them as well
                            if (j > 0)
                            {
                                BezierEditorTool.Instance.Groups[i].slices[j - 1].point3 = tempSlice.point0;
                            }

                            if (j < BezierEditorTool.Instance.Groups[i].slices.Count - 1)
                            {
                                BezierEditorTool.Instance.Groups[i].slices[j + 1].point0 = tempSlice.point3;
                            }
                        }
                    }

                    if (tempSlice.battery)
                    {
                        DrawBattery(i, j);
                    }
                }

                DrawBezierGroup(groupPosition, i);
            }

            Handles.color = backupColor;
        }



        public static void DrawBezierGroup(Vector3 groupPosition, int groupIndex)
        {
            tempSteps.Clear();
            tempPoligon.Clear();
            drawPoligons = new Vector3[4];

            if (groupIndex != BezierEditorTool.Instance.SelectedBezierGroupIndex)
            {
                Handles.color = BezierEditorTool.Instance.UnselectedBezierColor;
            }

            for (int sliceIndex = 0; sliceIndex < BezierEditorTool.Instance.Groups[groupIndex].slices.Count; sliceIndex++)
            {
                tempSlice = BezierEditorTool.Instance.Groups[groupIndex].slices[sliceIndex];

                for (int stepIndex = 0; stepIndex < BezierEditorTool.Instance.GroundRendererPrefab.Steps; stepIndex++)
                {
                    tempSteps.Add(groupPosition + Bezier.EvaluateCubic(tempSlice.point0, tempSlice.point1, tempSlice.point2, tempSlice.point3,
                        stepIndex / BezierEditorTool.Instance.GroundRendererPrefab.Steps));
                }
            }

            for (int i = 0; i < tempSteps.Count; i++)
            {
                HandleColorChange(i, groupIndex);
                point = tempSteps[i];

                rotation = Quaternion.identity;

                if (i != 0 && i != tempSteps.Count - 1)
                {
                    if (i == 0)
                    {
                        forward = tempSteps[1] - point;
                    }
                    else if (i == tempSteps.Count - 1)
                    {
                        forward = tempSteps[i + 1];
                    }
                    else
                    {
                        forward = tempSteps[i + 1] - tempSteps[i - 1];
                    }

                    //forward = i == 0 ? (tempSteps[1] - point) : i == tempSteps.Count - 1 ? (point - tempSteps[i - 1]) : (tempSteps[i + 1] - tempSteps[i - 1]);
                    angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
                    rotation = Quaternion.Euler(Vector3.forward * angle);
                }

                tempPoligon.Add(point + rotation * new Vector3(0, 0, -BezierEditorTool.Instance.BezierLineHalfWidth));
                tempPoligon.Add(point + rotation * new Vector3(0, -BezierEditorTool.Instance.GroundRendererPrefab.Height, -BezierEditorTool.Instance.BezierLineHalfWidth));
                tempPoligon.Add(point + rotation * new Vector3(0, -BezierEditorTool.Instance.GroundRendererPrefab.Height, BezierEditorTool.Instance.BezierLineHalfWidth));
                tempPoligon.Add(point + rotation * new Vector3(0, 0, BezierEditorTool.Instance.BezierLineHalfWidth));
                drawPoligons[0] = tempPoligon[tempPoligon.Count - 4];
                drawPoligons[1] = tempPoligon[tempPoligon.Count - 3];
                drawPoligons[2] = tempPoligon[tempPoligon.Count - 2];
                drawPoligons[3] = tempPoligon[tempPoligon.Count - 1];

                Handles.DrawAAConvexPolygon(drawPoligons);
            }

            for (int i = 0; i < tempSteps.Count - 1; i++)
            {
                HandleColorChange(i, groupIndex);
                drawPoligons[0] = tempPoligon[i * 4];
                drawPoligons[1] = tempPoligon[i * 4 + 3];
                drawPoligons[2] = tempPoligon[i * 4 + 7];
                drawPoligons[3] = tempPoligon[i * 4 + 4];
                Handles.DrawAAConvexPolygon(drawPoligons);

                drawPoligons[0] = tempPoligon[i * 4 + 1];
                drawPoligons[1] = tempPoligon[i * 4 + 2];
                drawPoligons[2] = tempPoligon[i * 4 + 6];
                drawPoligons[3] = tempPoligon[i * 4 + 5];
                Handles.DrawAAConvexPolygon(drawPoligons);
            }
        }

        public static void DrawBezierGroup(Vector3 groupPosition, int groupIndex, Color color)
        {
            tempSteps.Clear();
            tempPoligon.Clear();
            drawPoligons = new Vector3[4];
            Handles.color = color;

            for (int sliceIndex = 0; sliceIndex < BezierEditorTool.Instance.Groups[groupIndex].slices.Count; sliceIndex++)
            {
                tempSlice = BezierEditorTool.Instance.Groups[groupIndex].slices[sliceIndex];

                for (int stepIndex = 0; stepIndex < BezierEditorTool.Instance.GroundRendererPrefab.Steps; stepIndex++)
                {
                    tempSteps.Add(groupPosition + Bezier.EvaluateCubic(tempSlice.point0, tempSlice.point1, tempSlice.point2, tempSlice.point3,
                        stepIndex / BezierEditorTool.Instance.GroundRendererPrefab.Steps));
                }
            }

            for (int i = 0; i < tempSteps.Count; i++)
            {
                point = tempSteps[i];

                rotation = Quaternion.identity;

                if (i != 0 && i != tempSteps.Count - 1)
                {
                    if (i == 0)
                    {
                        forward = tempSteps[1] - point;
                    }
                    else if (i == tempSteps.Count - 1)
                    {
                        forward = tempSteps[i + 1];
                    }
                    else
                    {
                        forward = tempSteps[i + 1] - tempSteps[i - 1];
                    }

                    //forward = i == 0 ? (tempSteps[1] - point) : i == tempSteps.Count - 1 ? (point - tempSteps[i - 1]) : (tempSteps[i + 1] - tempSteps[i - 1]);
                    angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
                    rotation = Quaternion.Euler(Vector3.forward * angle);
                }

                tempPoligon.Add(point + rotation * new Vector3(0, 0, -BezierEditorTool.Instance.BezierLineHalfWidth));
                tempPoligon.Add(point + rotation * new Vector3(0, -BezierEditorTool.Instance.GroundRendererPrefab.Height, -BezierEditorTool.Instance.BezierLineHalfWidth));
                tempPoligon.Add(point + rotation * new Vector3(0, -BezierEditorTool.Instance.GroundRendererPrefab.Height, BezierEditorTool.Instance.BezierLineHalfWidth));
                tempPoligon.Add(point + rotation * new Vector3(0, 0, BezierEditorTool.Instance.BezierLineHalfWidth));
                drawPoligons[0] = tempPoligon[tempPoligon.Count - 4];
                drawPoligons[1] = tempPoligon[tempPoligon.Count - 3];
                drawPoligons[2] = tempPoligon[tempPoligon.Count - 2];
                drawPoligons[3] = tempPoligon[tempPoligon.Count - 1];

                Handles.DrawAAConvexPolygon(drawPoligons);
            }

            for (int i = 0; i < tempSteps.Count - 1; i++)
            {
                drawPoligons[0] = tempPoligon[i * 4];
                drawPoligons[1] = tempPoligon[i * 4 + 3];
                drawPoligons[2] = tempPoligon[i * 4 + 7];
                drawPoligons[3] = tempPoligon[i * 4 + 4];
                Handles.DrawAAConvexPolygon(drawPoligons);

                drawPoligons[0] = tempPoligon[i * 4 + 1];
                drawPoligons[1] = tempPoligon[i * 4 + 2];
                drawPoligons[2] = tempPoligon[i * 4 + 6];
                drawPoligons[3] = tempPoligon[i * 4 + 5];
                Handles.DrawAAConvexPolygon(drawPoligons);
            }
        }

        private static void HandleColorChange(int tempStepIndex, int groupIndex)
        {
            if (tempStepIndex % BezierEditorTool.Instance.GroundRendererPrefab.Steps != 0)
            {
                return;
            }

            if (groupIndex == BezierEditorTool.Instance.SelectedBezierGroupIndex)
            {
                if (tempStepIndex / BezierEditorTool.Instance.GroundRendererPrefab.Steps == BezierEditorTool.Instance.SelectedBezierSliceIndex)
                {
                    Handles.color = BezierEditorTool.Instance.SelectedSliceColor;
                }
                else
                {
                    Handles.color = BezierEditorTool.Instance.SelectedBezierColor;
                }
            }
        }

        public static void DrawBattery(int groupIndex, int sliceIndex)
        {
            bezierSlice = BezierEditorTool.Instance.Groups[groupIndex].slices[sliceIndex];
            length = Bezier.AproximateLength(bezierSlice.point0, bezierSlice.point1, bezierSlice.point2, bezierSlice.point3);
            batteriesCount = Mathf.FloorToInt(length / 2f);
            Handles.color = BezierEditorTool.Instance.CoinColor;

            for (int j = 0; j < batteriesCount; j++)
            {
                batteryPosition = Bezier.EvaluateCubic(bezierSlice.point0, bezierSlice.point1, bezierSlice.point2, bezierSlice.point3, j) + BezierEditorTool.Instance.Groups[groupIndex].position;
                Handles.DrawSolidDisc(batteryPosition + BezierEditorTool.Instance.CoinOffset, Vector3.forward, BezierEditorTool.Instance.CoinRadius);
            }
        }

        private void OnSceneGUI()
        {
            editorIsSelected = true;
            DrawHandles();

        }
    }
}