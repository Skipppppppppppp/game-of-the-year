using UnityEngine;
using UnityEngine.Analytics;

public class FacePlayer : MonoBehaviour
{
    public Transform pwayerTrans;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pwayerTrans != null)
        {            var transform = this.transform;
            Vector3 pwayerPosition = pwayerTrans.position;
            Vector2 direction = (pwayerPosition - transform.position).normalized;
            Quaternion currentRotation = transform.rotation;
            float angle = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            // Quaternion newRotation = Quaternion.Lerp(currentRotation, targetRotation, 0.1f);
            // Quaternion newRotation = Quaternion.RotateTowards(currentRotation, targetRotation, 720f * Time.deltaTime);
            Quaternion newRotation = targetRotation;
            transform.rotation = newRotation;
        }
    }
}
