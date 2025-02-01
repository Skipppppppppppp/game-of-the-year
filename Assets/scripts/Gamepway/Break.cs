using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Codice.Client.Common.EventTracking;
using Codice.Client.Common.GameUI;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class Break : MonoBehaviour
{
    // Start is called before the first frame update
    public int amountOfPoints;
    [Range(0,1)] public float distanceBetweenPointCoeff = 0.5f;
    public Transform player;
    private Vector2? prevMousePos;
    public float speedForBreak = 80;
    private Vector2[] points = Array.Empty<Vector2>();
    private List<Vector2>[] intersections = Array.Empty<List<Vector2>>();
    public CreateTriangle createTriangle;

    Line[][] lineequations;

    private Vector2[] MakeRandomPoints(Mesh mesh, int amount)
    {
        var bounds = mesh.bounds;
        var size = bounds.size;
        Vector2 offset = bounds.center;
        Vector2 halfsize = size / 2;

        // t * a * t = count = t^2 * a
        // t * a = x
        // t = y
        // size.y/size.x

        // count = 15
        // size.x = 1
        // size.y = 2
        // a = sqrt(2/1) = 1.4
        // t = 3.9
        // col = 3.9 / a = 3.9 / 1.4 = 2.8
        // row = 3.9 * a = 5.5

        Vector2 ComponentMult(Vector2 a, Vector2 b)
        {
            return new(a.x * b.x, a.y * b.y);
        }
        
        Vector2 ComponentDiv(Vector2 a, Vector2 b)
        {
            return new(a.x * b.x, a.y * b.y);
        }

        var sizea = ComponentMult(size, this.transform.localScale);

        var aspectRatio = sizea.y/sizea.x;
        var t = Mathf.Sqrt(amount / aspectRatio);
        var col = Mathf.RoundToInt(t);
        var row = Mathf.RoundToInt(t * aspectRatio);
        var ret = new Vector2[col * row];
        float stepX = size.x/(col+1);
        float stepY = size.y/(row+1);
        float maxShiftDistanceX = distanceBetweenPointCoeff * stepX;
        float maxShiftDistanceY = distanceBetweenPointCoeff * stepY;

        for (int colIndex = 0; colIndex < col; colIndex++)
        {
            float xPos = stepX * (colIndex+1);

            for (int rowIndex = 0; rowIndex < row; rowIndex++)
            {
                ref var v = ref ret[colIndex*row+rowIndex];
                float yPos = stepY * (rowIndex+1);

                float shiftX = Random.Range(-maxShiftDistanceX, maxShiftDistanceX);
                float shiftY = Random.Range(-maxShiftDistanceY, maxShiftDistanceY);

                v.x = xPos + shiftX;
                v.y = yPos + shiftY;

                v -= halfsize;
                v += offset;
            }

        }
        return ret;
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

    private static Vector2[] GetFixedPoints(Bounds bounds)
    {
        Vector2[] ret = new Vector2[]
        {
            new(1.0f/6, 1.0f/6),
            new(1.0f/6, 5.0f/6),
            new(5.0f/6, 1.0f/6),
            new(5.0f/6, 5.0f/6),
        };
        foreach (ref var v in ret.AsSpan())
        {
            Vector2 size = bounds.size;
            Vector2 leftTop = (Vector2)(bounds.center) - size / 2;
            v *= size;
            v += leftTop; 
        }
        return ret;
    }

    public void VeryFunMeshThings()
    { 
        var mesh = GetComponent<MeshFilter>().mesh;
        
        Vector2[] points = MakeRandomPoints(mesh, amountOfPoints);
        // Vector2[] points = GetFixedPoints(mesh.bounds);

        Line[][] lineequations = new Line[points.Length][];
        for (int i = 0; i < points.Length; i++)
        {
            lineequations[i] = new Line[points.Length + 4];
            for (int j = 0; j < points.Length; j++)
            {
                if (i == j)
                {
                    continue;
                }


                Vector2 A = points[i];
                Vector2 B = points[j];
                Line line = Voronoi.EquidistantLineBetweenTwoPoints(A, B);
                // lineequations.ForPoint(i).AndPoint(j) = line;
                lineequations[i][j] = line;
            }

            var boundLines = Voronoi.GetBoundsLines(mesh.bounds);
            for (int k = 0; k < boundLines.Length; k++)
            {
                int index = k + points.Length;
                lineequations[i][index] = boundLines[k];
            }          
        }


        List<Vector2>[] intersections = new List<Vector2>[points.Length]; 
        for (int i = 0; i < points.Length; i++)
        {
            Line[] linesForPoint = lineequations[i];
            intersections[i] = Voronoi.GetAreaVertices(points[i],linesForPoint,i);
            var newMesh = createTriangle.CreateMesh(intersections[i], points[i]);
            var myTransform = this.transform;
            var meshTransform = newMesh.transform;
            myTransform.GetPositionAndRotation(out var position, out var rotation);
            meshTransform.SetPositionAndRotation(position, rotation);
            meshTransform.localScale = myTransform.localScale;
            meshTransform.SetParent(myTransform.parent, worldPositionStays:true);
            var rb2d = newMesh.GetComponent<Rigidbody2D>();
            var prevMousePos2d = prevMousePos.Value;
            // Camera.main.ScreenToWorldPoint;
            var mousePosition = Input.mousePosition;
            var mousePosition2d = (Vector2)(mousePosition);
            var mouseMotionVector = mousePosition2d - prevMousePos2d;
            var mouseSpeed = mouseMotionVector.magnitude;
        }
        this.points = points;
        this.intersections = intersections;
        this.lineequations = lineequations;
        Destroy(this.gameObject);
        // Debug.Log(intersections.Length);
        // Debug.Log(intersections[0].Count);
        // Debug.Log(intersections[1].Count);
    }
    void OnDrawGizmos()
    {
        for (int pointIndex = 0; pointIndex < points.Length; pointIndex++)
        {
            var iters = intersections[pointIndex];
            var center = points[pointIndex];
            iters.Sort((a, b) =>
            {
                var va = a - center;
                var vb = b - center;
                var angleA = Mathf.Atan2(va.y, va.x);
                var angleB = Mathf.Atan2(vb.y, vb.x);
                return angleA > angleB ? 1 : -1;
            });

            Color color = pointIndex switch
            {
                0 => Color.red,
                1 => new Color(0f,0f,0f),
                2 => Color.yellow,
                3 => Color.green,
                _ => Color.blue,
            };
                Gizmos.color = color;


            for (int vertexIndex = 0; vertexIndex < iters.Count; vertexIndex++)
            {
                var a = iters[vertexIndex];
                var b = iters[(vertexIndex + 1) % iters.Count];
                Gizmos.DrawLine(transform.localToWorldMatrix.MultiplyPoint3x4(a), transform.localToWorldMatrix.MultiplyPoint3x4(b));
            }
        }
    }
    void Start()
    {

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
