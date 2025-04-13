using UnityEngine;

#nullable enable

public class MousePositionHelper : MonoBehaviour
{
    public static Vector2 FindDistancesToMouse(Vector2 objectPos)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float distanceToMouseX = mousePosition.x - objectPos.x;
        float distanceToMouseY = mousePosition.y - objectPos.y;

        return new Vector2(distanceToMouseX, distanceToMouseY);
    }
}