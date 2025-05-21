using System;
using UnityEngine;
using UnityEngine.Rendering;

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
    public Vector2 maxScales;
    public Axis axis;
    public bool isDirectionPositive;
    private Break breakScript;
    public float openingIncrement = 1f;
    private DoorLogic doorLogicScript;
    private float scaleOnAxis;
    private float maxScaleOnAxis;
    private float openness = 0;
    private float maxPosOnAxis;

    public DoorOpeningStatus ContinueOpening()
    {
        if (scaleOnAxis >= maxScaleOnAxis)
        {
            return DoorOpeningStatus.Done;
        }

        openness += openingIncrement;
        doorLogicScript.Rescale(openness);

        return DoorOpeningStatus.InProcess;
    }

    public DoorOpeningStatus ContinueOpening(Vector2 maxPosition)
    {
        if (scaleOnAxis >= maxScaleOnAxis)
        {
            return DoorOpeningStatus.Done;
        }


        return DoorOpeningStatus.InProcess;
    }

    public DoorOpeningStatus ContinueClosing()
    {
        float differenceBetweenScales = Mathf.Abs(scaleOnAxis - ComponentRef(ref initialScale));
        if (differenceBetweenScales <= 0.1)
        {
            return DoorOpeningStatus.Done;
        }

        openness -= openingIncrement;
        doorLogicScript.Rescale(openness);

        return DoorOpeningStatus.InProcess;
    }

    public (Vector2 NewPos, Vector2 NewScale) ChangeScaleToPercent(float openness)
    {
        (Vector2 NewPos, Vector2 NewScale) ret;
        float initialPosOnAxis = ComponentRef(ref initialPos);
        float t = Mathf.Abs(openness/100);
        this.openness = openness;

        float newPosOnAxis = Mathf.Lerp(initialPosOnAxis, maxPosOnAxis, t);

        if (openness < 0 && isDirectionPositive || openness > 0 && !isDirectionPositive)
        {
            ret = doorLogicScript.FindScaleFromNewPos(initialPosOnAxis);
        }
        else
        {
            ret = doorLogicScript.FindScaleFromNewPos(newPosOnAxis);
        }

        return ret;
    }

    public float FindScale(float initialScaleOnAxis, float maxScale, float initialPosOnAxis, float newPosOnAxis)
    {
        float neededScale = Mathf.Abs(newPosOnAxis - initialPosOnAxis)*2;

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

    public float FindPosition(float scaleOnAxis, float initialCoord, float initialScaleOnAxis, bool positive)
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

    public ref float ComponentRef(ref Vector2 v)
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
        trans = transform;
        initialPos = trans.position;
        initialScale = trans.localScale;
        doorLogicScript = GetComponent<DoorLogic>();
        maxScaleOnAxis = ComponentRef(ref maxScales);
        maxPosOnAxis = ComponentRef(ref initialPos) + maxScaleOnAxis/2;
        breakScript = GetComponent<Break>();
        if (isDirectionPositive == false)
        {
            openingIncrement = -openingIncrement;
        }
    }

    void FixedUpdate()
    {
        Vector2 scale = trans.localScale;
        scaleOnAxis = ComponentRef(ref scale);
    }
}


public enum DoorOpeningStatus
{
    Done,
    InProcess,
}