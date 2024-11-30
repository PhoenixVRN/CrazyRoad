using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D))]
    public class GroundRenderer : MonoBehaviour
    {
        [SerializeField] GroundPoint[] groundPoints;
        public GroundPoint[] GroundPoints
        {
            get => groundPoints;
            set => groundPoints = value;
        }

        [SerializeField, HideInInspector] List<Vector3> points = new List<Vector3>();
        public List<Vector3> Points => points;

        [SerializeField, HideInInspector] public int handlesPosition = 0;

        private MeshFilter meshFilter;
        private MeshFilter MeshFilter
        {
            get
            {
                if (meshFilter == null)
                {
                    meshFilter = GetComponent<MeshFilter>();
                }

                return meshFilter;
            }
        }

        private PolygonCollider2D polygonCollider;
        private PolygonCollider2D PolygonCollider
        {
            get
            {
                if (polygonCollider == null)
                {
                    polygonCollider = GetComponent<PolygonCollider2D>();
                }

                return polygonCollider;
            }
        }

        [SerializeField] float width;
        [SerializeField] float height;

        [Space]
        [SerializeField] float uScale;
        [SerializeField] float initialU;
        [SerializeField] float steps;

        [Space]
        [SerializeField] bool startTape = false;
        [SerializeField] bool endTape = false;

        public bool StartTape
        {
            get => startTape;
            set => startTape = value;
        }

        public bool EndTape
        {
            get => endTape;
            set => endTape = value;
        }

        public float InitialU
        {
            get => initialU;
            set => initialU = value;
        }
        public float Width => width;
        public float Height => height;
        public float Steps => steps;

        [SerializeField, HideInInspector] float lengthU;

        private Mesh mesh;


        public void OnDrawGizmos()
        {
            foreach (var point in groundPoints)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(point.position + transform.position, 0.2f);

                Gizmos.color = Color.green;
                Gizmos.DrawCube(transform.position + point.leftKey + point.position, Vector3.one * 0.1f);

                Gizmos.color = Color.blue;
                Gizmos.DrawCube(transform.position + point.rightKey + point.position, Vector3.one * 0.1f);
            }
        }


        public void RecalculateGround(GroundPoint[] groundPoints)
        {
            this.groundPoints = groundPoints;

            RecalculateGround();
        }


        public void RecalculateGround()
        {
            points = new List<Vector3>();

            for (int i = 0; i < GroundPoints.Length - 1; i++)
            {
                for (int j = 0; j < steps; j++)
                {
                    var point = Bezier.EvaluateCubic(
                        GroundPoints[i].position,
                        GroundPoints[i].position + GroundPoints[i].rightKey,
                        GroundPoints[i + 1].position + GroundPoints[i + 1].leftKey,
                        GroundPoints[i + 1].position,
                        j / steps);

                    points.Add(point);
                }
            }

            points.Add(GroundPoints[GroundPoints.Length - 1].position);

            RecalculateGround(points);
        }


        public void RecalculateGround(List<Vector3> points)
        {
            var vertices = new Vector3[points.Count * 4 + 8];
            var uvs = new Vector2[points.Count * 4 + 8];
            var indices = new int[(points.Count - 1) * 3 * 6 + 12];

            var colliderPoints = new Vector2[points.Count * 2];

            lengthU = initialU;

            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];

                int index = i * 4;

                var rotation = Quaternion.identity;

                if (i != 0 && i != points.Count - 1)
                {
                    var forward = i == 0 ? (points[1] - point) : i == points.Count - 1 ? (point - points[i - 1]) : (points[i + 1] - points[i - 1]);
                    var angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
                    rotation = Quaternion.Euler(Vector3.forward * angle);
                }

                vertices[index] = point + rotation * new Vector3(0, 0, -width / 2f);
                vertices[index + 1] = point + rotation * new Vector3(0, -height, -width / 2f);
                vertices[index + 2] = point + rotation * new Vector3(0, -height, width / 2f);
                vertices[index + 3] = point + rotation * new Vector3(0, 0, width / 2f);

                if (i != 0)
                {
                    lengthU += Vector3.Distance(points[i - 1], point);
                }

                uvs[index] = new Vector2(lengthU / uScale, 0.5f);
                uvs[index + 1] = new Vector2(lengthU / uScale, 0.25f);
                uvs[index + 2] = new Vector2(lengthU / uScale, 0f);
                uvs[index + 3] = new Vector2(lengthU / uScale, 1f);


                colliderPoints[i] = point;
                colliderPoints[points.Count * 2 - 1 - i] = point + rotation * Vector3.down * height;

            }

            for (int i = 0; i < points.Count - 1; i++)
            {
                int index = i * 4;
                int triangleIndex = i * 3 * 6;

                indices[triangleIndex++] = index;
                indices[triangleIndex++] = index + 7;
                indices[triangleIndex++] = index + 4;

                indices[triangleIndex++] = index + 7;
                indices[triangleIndex++] = index;
                indices[triangleIndex++] = index + 3;


                indices[triangleIndex++] = index;
                indices[triangleIndex++] = index + 4;
                indices[triangleIndex++] = index + 5;

                indices[triangleIndex++] = index + 5;
                indices[triangleIndex++] = index + 1;
                indices[triangleIndex++] = index;


                indices[triangleIndex++] = index + 2;
                indices[triangleIndex++] = index + 1;
                indices[triangleIndex++] = index + 6;

                indices[triangleIndex++] = index + 1;
                indices[triangleIndex++] = index + 5;
                indices[triangleIndex++] = index + 6;
            }

            vertices[vertices.Length - 8] = vertices[0];
            vertices[vertices.Length - 7] = vertices[1];
            vertices[vertices.Length - 6] = vertices[2];
            vertices[vertices.Length - 5] = vertices[3];

            uvs[vertices.Length - 8] = new Vector2(initialU, 0.5f);
            uvs[vertices.Length - 7] = new Vector2(initialU, 0.25f);
            uvs[vertices.Length - 6] = new Vector2(initialU, 0.25f);
            uvs[vertices.Length - 5] = new Vector2(initialU, 0.5f);

            vertices[vertices.Length - 4] = vertices[vertices.Length - 12];
            vertices[vertices.Length - 3] = vertices[vertices.Length - 11];
            vertices[vertices.Length - 2] = vertices[vertices.Length - 10];
            vertices[vertices.Length - 1] = vertices[vertices.Length - 9];

            uvs[vertices.Length - 4] = new Vector2(lengthU / uScale, 0.5f);
            uvs[vertices.Length - 3] = new Vector2(lengthU / uScale, 0.25f);
            uvs[vertices.Length - 2] = new Vector2(lengthU / uScale, 0.25f);
            uvs[vertices.Length - 1] = new Vector2(lengthU / uScale, 0.5f);

            indices[indices.Length - 12] = vertices.Length - 8;
            indices[indices.Length - 11] = vertices.Length - 7;
            indices[indices.Length - 10] = vertices.Length - 6;

            indices[indices.Length - 9] = vertices.Length - 6;
            indices[indices.Length - 8] = vertices.Length - 5;
            indices[indices.Length - 7] = vertices.Length - 8;

            indices[indices.Length - 6] = vertices.Length - 3;
            indices[indices.Length - 5] = vertices.Length - 4;
            indices[indices.Length - 4] = vertices.Length - 2;

            indices[indices.Length - 3] = vertices.Length - 1;
            indices[indices.Length - 2] = vertices.Length - 2;
            indices[indices.Length - 1] = vertices.Length - 4;


            mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = indices;

            mesh.RecalculateNormals();

            MeshFilter.mesh = mesh;


            PolygonCollider.points = colliderPoints;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }


        public Vector3 CalculateNormal(int groundPointIndex, float t)
        {
            var prev = Bezier.EvaluateCubic(
                GetGroundPointPos(groundPointIndex),
                GetGroundPointRight(groundPointIndex),
                GetGroundPointLeft(groundPointIndex + 1),
                GetGroundPointPos(groundPointIndex + 1),
                t - 0.05f);

            var next = Bezier.EvaluateCubic(
                GetGroundPointPos(groundPointIndex),
                GetGroundPointRight(groundPointIndex),
                GetGroundPointLeft(groundPointIndex + 1),
                GetGroundPointPos(groundPointIndex + 1),
                t + 0.05f);

            var direction = next - prev;

            var normal = new Vector3(-direction.y, direction.x, 0);

            return normal.normalized;
        }


        public Vector3 GetNearestPoint(Vector3 point)
        {
            float distance = float.PositiveInfinity;
            var closest = Vector3.positiveInfinity;

            for (int i = 0; i < points.Count - 1; i++)
            {
                var posibleClosest = FindNearestPointOnLine(transform.position + points[i], transform.position + points[i + 1], point);

                var newDistance = Vector3.Distance(point, posibleClosest);

                if (distance > newDistance)
                {
                    distance = newDistance;

                    closest = posibleClosest;
                }
            }

            return closest;
        }


        private Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
        {
            //Get heading
            Vector3 heading = (end - origin);
            float magnitudeMax = heading.magnitude;
            heading.Normalize();

            //Do projection from the point but clamp it
            Vector3 lhs = point - origin;
            float dotP = Vector3.Dot(lhs, heading);
            dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
            return origin + heading * dotP;
        }


        public Vector3 GetGroundPointPos(int index)
        {
            return transform.position + GroundPoints[index].position;
        }


        public Vector3 GetGroundPointLeft(int index)
        {
            return transform.position + GroundPoints[index].position + GroundPoints[index].leftKey;
        }


        public Vector3 GetGroundPointRight(int index)
        {
            return transform.position + GroundPoints[index].position + GroundPoints[index].rightKey;
        }


    }

}