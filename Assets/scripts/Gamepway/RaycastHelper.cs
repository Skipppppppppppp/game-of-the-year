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

        RaycastHit2D obstacle = Physics2D.Raycast(playerPos, direction, distanceToObject, obstacleLayerMask);

        if (obstacle.transform != null)
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

            RaycastHit2D obstacle = Physics2D.Raycast(playerPos, direction, distanceToObject, obstacleLayerMask);
            if (obstacle.transform != null)
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
}
