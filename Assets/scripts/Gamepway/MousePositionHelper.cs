using System;
using Unity.VisualScripting;
using UnityEngine;

#nullable enable

public class MousePositionHelper : MonoBehaviour
{
    public static Vector2 FindDistancesToMouse(Transform trans)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 objectPos = trans.position;
        float distanceToMouseX = Mathf.Abs(mousePosition.x - objectPosition.x);
        float distanceToMouseY = Mathf.Abs(mousePosition.y - objectPosition.y);

        return new Vector2(distanceToMouseX, distanceToMouseY);
    }
}