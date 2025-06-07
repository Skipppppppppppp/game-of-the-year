using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class CreateTriangle : MonoBehaviour
{
    private class ComparePoints : IComparer<Vector2>
    {
        private readonly Vector2 center;

        public ComparePoints(Vector2 center)
        {
            this.center = center;
        }

        public int Compare(Vector2 a, Vector2 b){
        {
            var va = a - center;
            var vb = b - center;
            var angleA = Mathf.Atan2(va.y, va.x);
            var angleB = Mathf.Atan2(vb.y, vb.x);
            return angleA > angleB ? 1 : -1;
        }
    }
    }
    public static List<Vector2> SortAndRemoveRepeatingVertices(Vector2 center, List<Vector2> intersections)
    {
        var compare = new ComparePoints(center);
        intersections.Sort(compare);
        List<Vector2> newIntersections = new();
        for (int i = 0; i < intersections.Count; i++)
        {
            if (newIntersections.Count == 0 ||
                newIntersections[^1] != intersections[i])
            {
                newIntersections.Add(intersections[i]);
            }
        }
        return newIntersections;
    }

    public static GameObject CreateMesh(List<Vector2> vertices, Vector2 center, Material material, Vector2 size)
    {
        vertices = SortAndRemoveRepeatingVertices(center,vertices);
        var triangle = new GameObject("Triangle"); // those who know 💀💀
        triangle.layer = 3;
        var meshFilter = triangle.AddComponent<MeshFilter>();
        var meshRenderer = triangle.AddComponent<MeshRenderer>();
        var mesh = new Mesh();
        meshFilter.mesh = mesh;
        
        Vector2[] PointsToUVs(Vector2[] points, Vector2 centerPoint)
        {
            int amountOfPoints = points.Length + 1;
            Vector2[] ret = new Vector2[amountOfPoints];
            Vector2 offset = new(0.5f, 0.5f);

            int uvIndex = 0;

            float centerUVx = center.x / size.x;
            float centerUVy = center.y / size.y;
            ret[uvIndex] = new Vector2(centerUVx, centerUVy) + offset;
            uvIndex++;

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 point = points[i];
                float newX = point.x / size.x;
                float newY = point.y / size.y;
                ret[uvIndex] = new Vector2(newX, newY) + offset;
                uvIndex++;
            }


            return ret;
        }

        Vector3[] Vertices()
        {
            var ret = new Vector3[vertices.Count + 1];
            ret[0] = center;
            for (int i = 0; i < vertices.Count; i++){
                ret[i + 1] = vertices[i];
            }
            return ret;
        }
        int[] VerticeIndexes()
        {
            var ret = new int[vertices.Count*3];
            for (int i = 0; i < vertices.Count; i++)
                {
                    ret[i*3] = 0;
                    ret[i*3+1] = i+1;
                    if (i == vertices.Count - 1)
                    {
                        ret[i*3+2] = 1;
                        continue;
                    }
                    ret[i*3+2] = i+2;
                }
            return ret;
        }

        mesh.vertices = Vertices();
        mesh.triangles = VerticeIndexes(); // if you complain about grammar you're gay

        mesh.SetUVs(0, PointsToUVs(vertices.ToArray(), center));
        meshRenderer.sharedMaterial = material;
        // mesh.RecalculateNormals();

        var rb2d = triangle.AddComponent<Rigidbody2D>();
        rb2d.excludeLayers = 1<<6;


        var polygonCollider = triangle.AddComponent<PolygonCollider2D>();
        polygonCollider.points = vertices.ToArray();

        return triangle;
    }
}
