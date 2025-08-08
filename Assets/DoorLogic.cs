using System.Text.RegularExpressions;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum Axis{x, y}

public class DoorLogic : MonoBehaviour, IObjectSelectedHandler
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
    private Vector2 maxScales;
    private Axis axis;
    private bool isDirectionPositive;
    private Break breakScript;
    private OpenAndClose doorScript;
    private GetOpenedByGuy guyInteractionScript;

    #nullable enable annotations
    
    private Rigidbody2D? RaycastForObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray,float.PositiveInfinity, (int) layerMask);
        var trans = rayHit.transform;
        if (trans == null)
        {
            return null;
        }
        return rayHit.rigidbody;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        initialPos = trans.position;
        rb2d = GetComponent<Rigidbody2D>();
        layerMask |= LayerMask.MoveableStuff;
        doorScript = GetComponent<OpenAndClose>();
        BoxCollider2D[] colliders = GetComponentsInChildren<BoxCollider2D>();
        foreach (var c in colliders)
        {
            if (c.isTrigger == false)
                collider = c;
        }
        initialBoundsX = collider.bounds.extents.x;
        initialScale = transform.localScale;
        breakScript = this.GetComponent<Break>();
        maxScales = doorScript.maxScales;
        isDirectionPositive = doorScript.isDirectionPositive;
        axis = doorScript.axis;
        guyInteractionScript = GetComponentInChildren<GetOpenedByGuy>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        prevMousePosition = mousePosition;
    }

    public (Vector2 NewPos, Vector2 NewScale) FindScaleFromNewPos(float requiredPosOnAxis)
    {
        Vector2 newScale = initialScale;
        Vector2 newPos = initialPos;

        var s = doorScript.FindScale(
            initialScaleOnAxis: doorScript.ComponentRef(ref initialScale),
            maxScale: doorScript.ComponentRef(ref maxScales),
            initialPosOnAxis: doorScript.ComponentRef(ref initialPos),
            newPosOnAxis: requiredPosOnAxis);

        doorScript.ComponentRef(ref newScale) = s;
        
        bool positive = requiredPosOnAxis > doorScript.ComponentRef(ref initialPos) && isDirectionPositive;
        var p = doorScript.FindPosition(
            doorScript.ComponentRef(ref newScale),
            doorScript.ComponentRef(ref initialPos), 
            doorScript.ComponentRef(ref initialScale), 
            positive: positive);
        
        doorScript.ComponentRef(ref newPos) = p;

        return (newPos, newScale);
    }

    public void Rescale(float doorOpennessPercentage)
    {   
        var n = doorScript.ChangeScaleToPercent(doorOpennessPercentage);

        trans.localScale = n.NewScale;
        trans.position = n.NewPos;

        if (collider.isTrigger == false && n.NewScale != initialScale)
        {
            collider.isTrigger = true;
            return;
        }

        bool doorClosed = n.NewScale == initialScale;
        if (collider.isTrigger == true && doorClosed)
        {
            collider.isTrigger = false;
        }
    }

    float FindPercentageFromDistanceToMouse(Vector2 distancesFromObjToMouse)
    {
        float distanceFromObjToMouse = doorScript.ComponentRef(ref distancesFromObjToMouse);

        float maxScaleOnAxis = doorScript.ComponentRef(ref maxScales);

        float percentage = Mathf.Clamp(distanceFromObjToMouse / maxScaleOnAxis * 100, -100, 100);
        return percentage;
    }

    void FixedUpdate()
    {
        if (objectSelected == false)
        {
            return;
        }

        guyInteractionScript.openingState.Reset();

        var distancesFromObjToMouse = MousePositionHelper.FindDistancesToMouse(initialPos);
        float doorOpennessPercentage = FindPercentageFromDistanceToMouse(distancesFromObjToMouse);

        Rescale(doorOpennessPercentage);
    }

    public void ProcessBeingSelected()
    {
        objectSelected = true;
    }

    public void Deselected()
    {
        objectSelected = false;
    }
}
