using TMPro;
using UnityEngine;

public class RotateAsObject : Rotation
{
    private Transform trans;
    public MovingObjects movingObjectScript;
    public Transform defaultObjectTrans;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movingObjectScript != null && movingObjectScript.movingObject != null)
        {
            var objectRb2d = movingObjectScript.movingObject;
            Vector3 itemPosition = CenterOfMassFinder.FindObjectPosition(objectRb2d);
            Rotate(itemPosition, trans);
            return;
        }

        if (defaultObjectTrans != null)
        {
            Vector2 objectPos = defaultObjectTrans.position;
            Rotate(objectPos, trans);
            return;
        }


        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Rotate(mousePosition, trans);
    }
}
