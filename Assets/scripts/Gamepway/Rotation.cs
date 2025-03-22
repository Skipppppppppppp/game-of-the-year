using UnityEngine;

public abstract class Rotation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    protected void Rotate(Vector3 targetPosition, Transform transform)
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
