using Unity.VisualScripting;
using UnityEngine;

public class StayOnObject : MonoBehaviour
{
    private Transform trans;
    public Transform playerTrans;
    public MovingObjects movingObjectScript;
    private float maxDistance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        maxDistance = movingObjectScript.maxDistance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movingObjectScript.movingObject == null || movingObjectScript == null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 distancesToMouse = MousePositionHelper.FindDistancesToMouse(playerTrans.position);
            float newXDistanceToMouse = Mathf.Clamp(distancesToMouse.x, -maxDistance, maxDistance);
            float newYDistanceToMouse = Mathf.Clamp(distancesToMouse.y, -maxDistance, maxDistance);

            trans.position = new Vector2(playerTrans.position.x + newXDistanceToMouse, playerTrans.position.y + newYDistanceToMouse);
            return;
        }
        var objectRb2d = movingObjectScript.movingObject;
        Vector3 itemPosition = CenterOfMassFinder.FindObjectPosition(objectRb2d);
        trans.position = itemPosition;
    }
}
