using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    private static Vector2[] MakeRandomPoints(Mesh mesh, int amount)
    {
        var bounds = mesh.bounds;
        var size = bounds.size;
        Vector2 offset = bounds.center;
        Vector2 halfsize = size / 2;
        var ret = new Vector2[amount*2];

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

    private void VeryFunMeshThings()
    {
        Debug.Log("u brok the square :<");
        var mesh = GetComponent<MeshFilter>().mesh;
        Vector2[] randomPoints = MakeRandomPoints(mesh, 8);
        points = randomPoints;
    }
    void OnDrawGizmos()
    {
        foreach (Vector2 v in points)
        {
            // homogenous coordinates
            var v1 = transform.localToWorldMatrix.MultiplyPoint3x4(v);
            Gizmos.DrawSphere(v1, 0.1f);
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
