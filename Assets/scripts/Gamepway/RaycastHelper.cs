using System.Linq;
using UnityEngine;
using System.Collections.Generic;
// I HATE SYSTEM NUMERICS AND I WANT THEM TO DIE

public class RaycastHelper : MonoBehaviour
{
    public static Rigidbody2D? TrySelectObject(Vector2 playerPos, float maxDistance, int layerMask, int obstacleLayerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray, float.PositiveInfinity, layerMask);
        var trans = rayHit.transform;
        if (trans == null)
        {
            return null;
        }

        Vector2 mouseDistances = MousePositionHelper.FindDistancesToMouse(playerPos);

        if (mouseDistances.magnitude > maxDistance)
        {
            return null;
        }

        var obstacleHit = Physics2D.Raycast(playerPos, mouseDistances.normalized, mouseDistances.magnitude, obstacleLayerMask);
        if (obstacleHit.transform != null)
        {
            return null;
        }
        return rayHit.rigidbody;
    }

    public static Rigidbody2D? TryRaycastToMouse(Vector2 playerPos, float maxDistance, int layerMask, int obstacleLayerMask)
    {
        Vector2 distancesToMouse = MousePositionHelper.FindDistancesToMouse(playerPos);
        float distanceToMouse = distancesToMouse.magnitude;

        Vector2 direction = distancesToMouse.normalized;

        RaycastHit2D hitThing = Physics2D.Raycast(playerPos, direction, maxDistance, layerMask);
        Rigidbody2D ret = hitThing.rigidbody;

        if (ret is null)
        {
            return null;
        }

        Vector2 hitThingPos = hitThing.transform.position;
        float distanceToObject = (playerPos - hitThingPos).magnitude;

        if (PathObstructed(playerPos, hitThingPos, obstacleLayerMask) == true)
        {
            return null;
        }

        return ret;
    }

    public static Rigidbody2D[]? TryRaycastAllToMouse(Vector2 playerPos, float maxDistance, int layerMask, int obstacleLayerMask)
    {
        Vector2 distancesToMouse = MousePositionHelper.FindDistancesToMouse(playerPos);
        float distanceToMouse = distancesToMouse.magnitude;

        Vector2 direction = distancesToMouse.normalized;

        RaycastHit2D[] hitThings = Physics2D.RaycastAll(playerPos, direction, maxDistance, layerMask);

        if (hitThings.Length == 0)
        {
            return null;
        }

        List<Rigidbody2D> hitBodies = new List<Rigidbody2D>();

        for (int i = 0; i < hitThings.Length; i++)
        {
            RaycastHit2D hitThing = hitThings[i];

            Vector2 hitThingPos = hitThing.transform.position;
            float distanceToObject = (playerPos - hitThingPos).magnitude;

            if (PathObstructed(playerPos, hitThingPos, obstacleLayerMask) == true)
            {
                continue;
            }

            hitBodies.Add(hitThing.rigidbody);
        }

        var ret = hitBodies.ToArray();

        if (ret.Length == 0)
        {
            return null;
        }

        return ret;
    }

    public static Rigidbody2D[] RotatedOverlapBox(
        Vector2 playerPos,
        float boxWidth,
        float boxHeight,
        int layerMask,
        int obstacleLayerMask)
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Direction and angle
        Vector2 direction = (mousePos - playerPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        Vector2 boxCenter = playerPos + direction * (boxWidth / 2f);

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            point: boxCenter,
            size: new Vector2(boxWidth, boxHeight),
            angle: angle,
            layerMask: layerMask
        );
        Debug.DrawLine(boxCenter - Vector2.right * boxWidth / 2f, boxCenter + Vector2.right * boxWidth / 2f, Color.green);
Debug.DrawLine(boxCenter - Vector2.up * boxHeight / 2f, boxCenter + Vector2.up * boxHeight / 2f, Color.blue);

        List<Rigidbody2D> validRigidbodies = new List<Rigidbody2D>();

        foreach (Collider2D hit in hits)
        {
            Vector2 objectPos = hit.transform.position;

            if (!PathObstructed(playerPos, objectPos, obstacleLayerMask))
            {
                Rigidbody2D rb = hit.attachedRigidbody;
                if (rb != null && !validRigidbodies.Contains(rb))
                    validRigidbodies.Add(rb);
            }
        }

        return validRigidbodies.ToArray();
    }


    private static bool PathObstructed(Vector2 firstPos, Vector2 secondPos, int obstacleLayerMask)
    {
        Vector2 direction = (secondPos - firstPos).normalized;
        float distanceToObject = (secondPos - firstPos).magnitude;
        RaycastHit2D obstacle = Physics2D.Raycast(firstPos, direction, distanceToObject, obstacleLayerMask);

        return obstacle.transform != null;
    }

    public static bool OnGround(BoxCollider2D collider)
    {
        Transform trans = collider.transform;
        Vector2 pos = trans.position;

        var floor = Physics2D.BoxCast(pos, collider.bounds.size, 0, new Vector2(0, -1), 0.1f, (int)LayerMask.Default);
        return floor.transform != null;
    }
}
