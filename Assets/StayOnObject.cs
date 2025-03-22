using UnityEngine;

public class StayOnObject : MonoBehaviour
{
    private Transform transform;
    public MovingObjects movingObjectScript;
    private Transform objectTransform;

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
            transform.position = mousePosition;
            return;
        }
        objectTransform = movingObjectScript.movingObject.transform;
        Vector3 itemPosition = objectTransform.position;
        transform.position = itemPosition;
    }
}
