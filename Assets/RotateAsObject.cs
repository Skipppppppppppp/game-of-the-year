using TMPro;
using UnityEngine;

public class RotateAsObject : Rotation
{
    private Transform transform;
    public MovingObjects movingObjectScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movingObjectScript.movingObject == null || movingObjectScript == null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Rotate (mousePosition, transform);
            return;
        }
        var objectRb2d = movingObjectScript.movingObject;
        Vector3 itemPosition = CenterOfMassFinder.FindObjectPosition(objectRb2d);
        Rotate(itemPosition, transform);
    }
}
