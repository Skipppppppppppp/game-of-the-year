using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.GameUI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class Break : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3[] newVertices;
    public Vector2[] newUV;
    public int[] newTriangles;
    public Transform player;
    private Vector2? prevMousePos;
    public float speedForBreak = 80;
    private Vector2[] points = Array.Empty<Vector2>();
    private List<Vector2>[] intersections = Array.Empty<List<Vector2>>();

    private static Vector2[] MakeRandomPoints(Mesh mesh, int amount)
    {
        var bounds = mesh.bounds;
        var size = bounds.size;
        Vector2 offset = bounds.center;
        Vector2 halfsize = size / 2;
        var ret = new Vector2[amount];

        for (int i = 0; i < ret.Length; i++)
        {
            ref var v = ref ret[i];
            v.x = Random.Range(0, size.x);
            v.y = Random.Range(0, size.y);
            v -= halfsize;
            v += offset;
        }
        return ret;
    }

    public struct Line
    {
        public float a;
        public float b;
        public float c;
    }
    
    // public static Vector2[] FindMultipleIntersections(Line[] array)
    // {
    //     for (int i = 0; i <= array.Length; i++)
    //     {
    //         for (int j = 0; j < array.Length; j++)
    //         {
    //             if (i == j)
    //             {
    //                 continue;
    //             }
    //             Vector2[] intersectionArray;
    //             Vector2 intewsection = FindIntersectionFromLines(array[i],array[j]);

    //         }
    //     }
    // }
    public static Line FindLineThroughTwoPoints(Vector2 A, Vector2 B)
    {
        Line line;
        line.a = B.y - A.y;
        line.b = A.x - B.x;
        line.c = line.a * A.x + line.b * A.y;
        return line; 
    }
    public static Line EquidistantLineBetweenTwoPoints(Vector2 A, Vector2 B)
    {
        Vector2 D = B - A;
        Vector2 O = 0.5f * D + A; 
        Line line;
        line.a = D.x;
        line.b = D.y;
        line.c = line.a * O.x + line.b * O.y;
        return line;
    }

    public static Vector2 FindIntersectionFromLines(Line line1, Line line2)
    {
        Vector2 intersection;
        intersection.x = -(line1.b * line2.c - line2.b * line1.c) / (line1.a * line2.b - line2.a * line1.b);
        intersection.y = -(line1.c * line2.a - line2.c * line1.a) / (line1.a * line2.b - line2.a * line1.b);
        return intersection;
    }

    public static bool ArePointsOnOneSide(Vector2 pointA, Line line, Vector2 pointB)
    {
        float FindPerpProjection(Vector2 point)
        {
            return (line.a * point.x + line.b * point.y + line.c);
        }

        float projectionToPoint = FindPerpProjection(pointA);
        float projectionToIntersection = FindPerpProjection(pointB);
        if (projectionToIntersection * projectionToPoint < 0)
        {
            return false;
        }
        return true;
    }
    public static List<Vector2> GetAreaVertices(Vector2 point, Line[] lines, int ignoredLineIndex)
    {
        var intersections = new List<Vector2>();
        for (int line1Index = 0; line1Index < lines.Length; line1Index++)
        {
            if (ignoredLineIndex == line1Index)
            {
                continue;
            }

            for (int line2Index = 0; line2Index < lines.Length; line2Index++)
            {
                if (line2Index == line1Index || line2Index == ignoredLineIndex)
                {
                    continue;
                }

                Line line1 = lines[line1Index];
                Line line2 = lines[line2Index];
                Vector2 intersection = FindIntersectionFromLines(line1, line2);

                bool IsAllBeforeLine()
                {
                    for (int otherLineIndex = 0; otherLineIndex < lines.Length; otherLineIndex++)
                    {
                        if (otherLineIndex == line1Index || otherLineIndex == line2Index || otherLineIndex == ignoredLineIndex)
                        {
                            continue;
                        }
                        Line otherLine = lines[otherLineIndex];
                        bool m = ArePointsOnOneSide(point,otherLine,intersection);
                        if (m == false)
                        {
                            return m;
                        }
                    }
                    return true;
                }

                if (IsAllBeforeLine())
                {
                    intersections.Add(intersection); 
                }
            }
        }
        return intersections;
    }

    public void VeryFunMeshThings()
    { 
        var mesh = GetComponent<MeshFilter>().mesh;
        Vector2[] randomPoints = MakeRandomPoints(mesh, 2);
        Line[][] lineequations = new Line[randomPoints.Length][];
        for (int i = 0; i < randomPoints.Length; i++)
        {
            lineequations[i] = new Line[randomPoints.Length + 4];
            for (int j = 0; j < randomPoints.Length; j++)
            {
                if (i == j)
                {
                    continue;
                }


                Vector2 A = randomPoints[i];
                Vector2 B = randomPoints[j];
                Line line = EquidistantLineBetweenTwoPoints(A, B);
                // lineequations.ForPoint(i).AndPoint(j) = line;
                lineequations[i][j] = line;
            }
            for (int k = 0; k < 4; k++) // adding lines for mesh's walls so intersections are within mesh bounds
            {
                var bounds = mesh.bounds;
                var size = bounds.size;
                Vector2 offset = bounds.center;
                Vector2 halfsize = size / 2;
                int index = k + randomPoints.Length;
                Line line;
                switch (k)
                {
                    case 0: // left
                    {
                        line.a = 1;
                        line.b = 0;
                        line.c = offset.x - halfsize.x;
                        break;
                    }
                    case 1: // top
                    {
                        line.a = 0;
                        line.b = 1;
                        line.c = offset.y - halfsize.y;
                        break;
                    }
                    case 2: // right
                    {
                        line.a = 1;
                        line.b = 0;
                        line.c = offset.x + halfsize.x;
                        break;
                    }
                    case 3: // bottom
                    {
                        line.a = 0;
                        line.b = 1;
                        line.c = offset.y + halfsize.y;
                        break;
                    }
                    default:
                    {
                        System.Diagnostics.Debug.Fail("Unreachable");
                        return;
                    }
                }
                lineequations[i][index] = line;
            }
        }


        List<Vector2>[] intersections = new List<Vector2>[randomPoints.Length]; 
        for (int i = 0; i < randomPoints.Length; i++)
        {
            Line[] linesForPoint = lineequations[i];
            intersections[i] = GetAreaVertices(randomPoints[i],linesForPoint,i);
        }
        points = randomPoints;
        this.intersections = intersections;
        // Debug.Log(intersections.Length);
        // Debug.Log(intersections[0].Count);
        // Debug.Log(intersections[1].Count);
    }
    void OnDrawGizmos()
    {
        foreach (Vector2 v in points)
        {
            // homogenous coordinates
            var v1 = transform.localToWorldMatrix.MultiplyPoint3x4(v);
            Gizmos.DrawSphere(v1, 0.1f);
        }
        for (int i = 0; i < intersections.Length; i++)
        {
            List<Vector2> x = intersections[i];
            Color color = i switch
            {
                0 => Color.red,
                1 => new Color(255f,140f,0f),
                2 => Color.yellow,
                3 => Color.green,
                _ => Color.blue,
            };
            foreach (Vector2 w in x)
            {
                var w1 = transform.localToWorldMatrix.MultiplyPoint3x4(w);
                Gizmos.color = color;
                Gizmos.DrawSphere(w1, 0.2f);
            }
        }
    }
    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
        var meshBounds = mesh.bounds;
        var collider = this.GetComponent<BoxCollider2D>();
        {
            collider.offset = meshBounds.center;
            collider.size = meshBounds.size;
        }

        VeryFunMeshThings();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(player.position, this.transform.position);
        if (distance > 8.0f)
        {
            return;
        }
        if (!Input.GetMouseButton(1))
        {
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
        if (!prevMousePos.HasValue)
        {
            prevMousePos = mousePosition2D;
            return;
        }

        Vector2 savedPreviousMousePosition = prevMousePos.Value;
        prevMousePos = mousePosition2D;


        {
            RaycastHit2D hit = Physics2D.Raycast(savedPreviousMousePosition, Vector2.zero);
            if (hit.collider == null || hit.collider.transform != transform)
            {
                return;
            }
        }

        float mouseDistanceTraveled = Vector2.Distance(savedPreviousMousePosition, prevMousePos.Value);
        float mouseVelocity = mouseDistanceTraveled / Time.deltaTime;
        if (mouseVelocity <= speedForBreak)
        {
            return;
        }
        VeryFunMeshThings();
    }
}
