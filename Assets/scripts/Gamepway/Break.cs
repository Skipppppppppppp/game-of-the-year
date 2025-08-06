using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;
using Random = UnityEngine.Random;

public class Break : MonoBehaviour, IObjectSelectedHandler
{
    public int amountOfPoints;
    [Range(0,1)] public float distanceBetweenPointCoeff = 0.5f;
    private Transform transPlayer;
    private Vector2? prevMousePos;
    public float speedForBreak = 80;
    private Vector2[] points = Array.Empty<Vector2>();
    private List<Vector2>[] intersections = Array.Empty<List<Vector2>>();
    Line[][] lineequations;
    public bool canBeBroken = true;
    private MeshRenderer meshRenderer;

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

        var sizea = ComponentMult(size, this.transform.localScale);

        var aspectRatio = sizea.y / sizea.x;
        var t = Mathf.Sqrt(amount / aspectRatio);
        var col = Mathf.RoundToInt(t);
        var row = Mathf.RoundToInt(t * aspectRatio);
        var ret = new Vector2[col * row];
        float stepX = size.x / (col + 1);
        float stepY = size.y / (row + 1);
        float maxShiftDistanceX = distanceBetweenPointCoeff * stepX;
        float maxShiftDistanceY = distanceBetweenPointCoeff * stepY;

        for (int colIndex = 0; colIndex < col; colIndex++)
        {
            float xPos = stepX * (colIndex + 1);

            for (int rowIndex = 0; rowIndex < row; rowIndex++)
            {
                ref var v = ref ret[colIndex * row + rowIndex];
                float yPos = stepY * (rowIndex + 1);

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


    // private static Vector2[] GetFixedPoints(Bounds bounds)
    // {
    //     Vector2[] ret = new Vector2[]
    //     {
    //         new(1.0f/6, 1.0f/6),
    //         new(1.0f/6, 5.0f/6),
    //         new(5.0f/6, 1.0f/6),
    //         new(5.0f/6, 5.0f/6),
    //     };
    //     foreach (ref var v in ret.AsSpan())
    //     {
    //         Vector2 size = bounds.size;
    //         Vector2 leftTop = (Vector2)(bounds.center) - size / 2;
    //         v *= size;
    //         v += leftTop; 
    //     }
    //     return ret;
    // }

    public void VeryFunMeshThings()
    {
        var player = Physics2D.OverlapCircle(transform.position, 100, (int) LayerMask.Pwayer);
        transPlayer = player.transform;

        var mesh = GetComponent<MeshFilter>().mesh;
        meshRenderer = GetComponent<MeshRenderer>();

        Material material = meshRenderer.material;

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


        ShardForce BaseForce()
        {
            Vector2 prevMousePos2d;
            Vector2 mouseMotionVector;

            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mousePosition2d = (Vector2)(mousePosition);

            if (prevMousePos != null)
            {
                prevMousePos2d = prevMousePos.Value;
                mouseMotionVector = mousePosition2d - prevMousePos2d;
                var mouseSpeed = mouseMotionVector.magnitude / Time.deltaTime;
                var mouseMotionDirection = mouseMotionVector.normalized;
                float clampedMouseSpeed = Mathf.Clamp(mouseSpeed, 0, 1000);
                clampedMouseSpeed *= 3;
                var ret = new ShardForce(mouseMotionDirection, clampedMouseSpeed);
                return ret;
            }

            prevMousePos2d = transPlayer.position;

            Vector2 differenceBetweenPoints = mousePosition2d - prevMousePos2d;
            float distanceBetwenMouseAndPlayer = differenceBetweenPoints.magnitude;
            float clampedDistanceBetwenMouseAndPlayer = Mathf.Clamp(distanceBetwenMouseAndPlayer, 0, 10);
            Vector2 motionVector = differenceBetweenPoints.normalized;

            var force = new ShardForce(motionVector, clampedDistanceBetwenMouseAndPlayer * 100);

            return force;
        }

        var force = BaseForce();
        var props = GetComponent<Rigidbody2D>().GetInitialProps();

        List<Vector2>[] intersections = new List<Vector2>[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            Line[] linesForPoint = lineequations[i];
            intersections[i] = Voronoi.GetAreaVertices(points[i], linesForPoint, i);
            var newMesh = CreateTriangle.CreateMesh(intersections[i], points[i], material, mesh.bounds.size);
            var myTransform = this.transform;
            var meshTransform = newMesh.transform;
            myTransform.GetPositionAndRotation(out var position, out var rotation);
            meshTransform.SetPositionAndRotation(position, rotation);
            meshTransform.localScale = myTransform.localScale;
            meshTransform.SetParent(myTransform.parent, worldPositionStays: true);
            var rb2d = newMesh.GetComponent<Rigidbody2D>();
            rb2d.angularDamping = props.AngularDamping;
            rb2d.linearDamping = props.LinearDamping;
            rb2d.gravityScale = props.GravityScale;
            rb2d.AddForce(force.Force);

            float randomForceX = Random.Range(100, 500);
            float randomForceY = Random.Range(100, 500);
            Vector2 randomForceVector = new Vector2(randomForceX, randomForceY);
            rb2d.AddForce(randomForceVector);
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

    private Vector2 GetNewMousePosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
        return mousePosition2D;
    }

    public void ProcessBeingSelected()
    {
        if (!canBeBroken)
        {
            return;
        }
        var newMousePos = GetNewMousePosition();
        MaybeMeshThings();
        this.prevMousePos = newMousePos;
        return;
        
        void MaybeMeshThings()
        {
            if (this.prevMousePos is not { } prevMousePos)
            {
                return;
            }
            if (!ShouldBreakObject(prevMousePos))
            {
                return;
            }

            VeryFunMeshThings();
            return;
        }

        bool ShouldBreakObject(Vector2 prevMousePos)
        {
            float distance = Vector2.Distance(transPlayer.position, this.transform.position);
            if (distance > 12.0f)
            {
                return false;
            }

            if (!Input.GetMouseButton(1))
            {
                return false;
            }

            if (WasHoveringOverOtherObjectThanSelf(prevMousePos))
            {
                return false;
            }            
            
            float mouseDistanceTraveled = Vector2.Distance(prevMousePos, newMousePos);
            float mouseVelocity = mouseDistanceTraveled / Time.deltaTime;
            if (mouseVelocity <= speedForBreak)
            {
                return false;
            }
            return true;
        
            bool WasHoveringOverOtherObjectThanSelf(Vector2 prevMousePos)
            {
                RaycastHit2D hit = Physics2D.Raycast(prevMousePos, Vector2.zero);
                if (hit.collider == null)
                {
                    return false;
                }
                if (hit.collider.transform != transform)
                {
                    return true;
                }
                return false;
            }
        }
    }

    public void Deselected()
    {
        prevMousePos = null;
    }
}

public readonly struct ShardForce
{
    public readonly Vector2 Direction;
    public readonly float Magnitude;
    public readonly Vector2 Force => Direction * Magnitude;

    public ShardForce(Vector2 direction, float magnitude)
    {
        Direction = direction;
        Magnitude = magnitude;
    }
}