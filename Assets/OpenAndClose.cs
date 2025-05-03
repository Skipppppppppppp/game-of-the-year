using UnityEngine;

public class OpenAndClose : MonoBehaviour
{

    private Transform trans;
    private Vector2 initialPos;
    private Rigidbody2D rb2d;
    private Vector2 mousePosition;
    private Vector2 prevMousePosition;
    private LayerMask layerMask;
    private bool objectSelected;
    private float initialBoundsX;
    private Vector2 initialScale;
    private BoxCollider2D collider;
    private GameObject colliderObj;
    public float maxScaleX;
    public float maxScaleY;
    public Axis axis;
    public bool isDirectionPositive;
    private Break breakScript;

    public DoorOpeningStatus ContinueOpening()
    {

        return DoorOpeningStatus.InProcess;
    }

    public DoorOpeningStatus ContinueClosing()
    {
        return DoorOpeningStatus.InProcess;
    }

    float FindScale(float initialScaleOnAxis, float maxScale, float distanceToMouse)
    {
        Vector2 currentScale = trans.localScale;
        float neededScale = distanceToMouse / ComponentRef(ref initialScale);
        neededScale = Mathf.Clamp(neededScale, initialScaleOnAxis, maxScale);

        if (Mathf.Abs(neededScale - initialScaleOnAxis) <= 0.1f)
        {
            neededScale = initialScaleOnAxis;
        }

        if (neededScale == maxScale || neededScale == initialScaleOnAxis)
        {
            breakScript.canBeBroken = true;
        }
        else
        {
            breakScript.canBeBroken = false;
        }

        return neededScale;
    }

    float FindPosition(float scaleOnAxis, float initialCoord, float initialScaleOnAxis, bool positive)
    {
        float newCoord;

        if (!positive)
        {
            scaleOnAxis = -scaleOnAxis;
            initialScaleOnAxis = -initialScaleOnAxis;
        }
        
        newCoord = scaleOnAxis/2 + initialCoord - initialScaleOnAxis/2;
        return newCoord;
    }

    private ref float ComponentRef(ref Vector2 v)
    {
        if (axis == Axis.x)
        {
            return ref v.x;
        }
        if (axis == Axis.y)
        {
            return ref v.y;
        }
        System.Diagnostics.Debug.Fail("No such axis");
        throw null!;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
}


public enum DoorOpeningStatus
{
    Done,
    InProcess,
}