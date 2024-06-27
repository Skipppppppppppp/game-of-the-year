using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class watchMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var transform = this.transform;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;
        Quaternion currentRotation = transform.rotation;
        float angle = -Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        // Quaternion newRotation = Quaternion.Lerp(currentRotation, targetRotation, 0.1f);
        // Quaternion newRotation = Quaternion.RotateTowards(currentRotation, targetRotation, 720f * Time.deltaTime);
        Quaternion newRotation = targetRotation;
        transform.rotation = newRotation;
    }
}
