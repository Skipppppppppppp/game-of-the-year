using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public abstract class Rotation : MonoBehaviour
{
    public static void RotateObjectToTarget(Vector3 targetPosition, Transform transform)
    {
        Quaternion newRotation = GetRotationToTarget(targetPosition, transform);
        transform.rotation = newRotation;
    }

    public static Quaternion GetRotationToTarget(Vector3 targetPosition, Transform transform)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        float angle = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        return targetRotation;
    }


    public static Vector2 RotateVector(Vector2 v, float degrees)
    {
        float radians = degrees * (float)Mathf.PI / 180f;
        float cos = (float)Mathf.Cos(radians);
        float sin = (float)Mathf.Sin(radians);

        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }
}
