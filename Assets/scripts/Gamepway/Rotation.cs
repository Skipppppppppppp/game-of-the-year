using UnityEngine;

public abstract class Rotation : MonoBehaviour
{
    protected void RotateObjectToTarget(Vector3 targetPosition, Transform transform)
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        Quaternion currentRotation = transform.rotation;
        float angle = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        // Quaternion newRotation = Quaternion.Lerp(currentRotation, targetRotation, 0.1f);
        // Quaternion newRotation = Quaternion.RotateTowards(currentRotation, targetRotation, 720f * Time.deltaTime);
        Quaternion newRotation = targetRotation;
        transform.rotation = newRotation;
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
