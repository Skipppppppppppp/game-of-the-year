using Unity.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum Axis{x, y}

public class Door : MonoBehaviour
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

    #nullable enable annotations
    private Rigidbody2D? RaycastForObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray,float.PositiveInfinity,layerMask);
        var trans = rayHit.transform;
        if (trans == null)
        {
            return null;
        }
        return rayHit.rigidbody;
    }

    float FindScale(Axis axis, float initialScaleOnAxis, float maxScale, float distanceToMouse)
    {
        Vector2 currentScale = trans.localScale;
        float neededScale = 0;

        if (axis == Axis.x)
        {
            neededScale = distanceToMouse/initialScale.x;
        }

        if (axis == Axis.y)
        {
            neededScale = distanceToMouse/initialScale.y;
        }

        neededScale = Mathf.Clamp(neededScale, initialScaleOnAxis, maxScale);

        if (Mathf.Abs(neededScale - initialScaleOnAxis) <= 0.1)
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = GetComponent<Transform>();
        initialPos = trans.position;
        rb2d = GetComponent<Rigidbody2D>();
        layerMask |= 1 << UnityEngine.LayerMask.NameToLayer("Moveable Stuff");
        collider = GetComponent<BoxCollider2D>();
        var cols = GetComponentsInChildren<BoxCollider2D>();
        int i = 0;
        foreach (BoxCollider2D col in cols)
        {
            if (i == 0)
            {
                i++;
                continue;
            }
            colliderObj = col.gameObject;
            i++;
        }
        initialBoundsX = collider.bounds.extents.x;
        initialScale = transform.localScale;
        breakScript = this.GetComponent<Break>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(1))
        {
            if (RaycastForObject() == rb2d)
            {
                objectSelected = true;
            }
        }
        if (Input.GetMouseButtonUp(1) && objectSelected == true)
        {
            objectSelected = false;
        }
        prevMousePosition = mousePosition;
    }

    void FixedUpdate()
    {
        if (objectSelected == false)
        {
            return;
        }

        Vector2 newScale = initialScale;
        Vector2 newPos = initialPos;

        var distancesFromObjToMouse = MousePositionHelper.FindDistancesToMouse(initialPos);

        if (!isDirectionPositive)
        {
            distancesFromObjToMouse = -distancesFromObjToMouse;
        }

        if (axis == Axis.x)
        {
            var s = FindScale(
                axis: Axis.x,
                initialScaleOnAxis: initialScale.x,
                maxScale: maxScaleX,
                distanceToMouse: distancesFromObjToMouse.x
                );
            newScale.x = s;
            
            var p = FindPosition(newScale.x, initialPos.x, initialScale.x, true);
            
            if (distancesFromObjToMouse.x > 0 && isDirectionPositive)
            {
                newPos.x = FindPosition(newScale.x, initialPos.x, initialScale.x, true);
            }
            else
            {
                newPos.x = FindPosition(newScale.x, initialPos.x, initialScale.x, false);
            }

        }

        else
        {
            newScale = new Vector2(initialScale.x, FindScale(Axis.y, initialScale.y, maxScaleY, distancesFromObjToMouse.y));

            if (distancesFromObjToMouse.y > 0 && isDirectionPositive)
            {
                newPos.y = FindPosition(newScale.y, initialPos.y, initialScale.y, true);
            }
            else
            {
                newPos.y = FindPosition(newScale.y, initialPos.y, initialScale.y, false);
            }
        }

        trans.localScale = newScale;
        trans.position = newPos;

        if (colliderObj.activeInHierarchy == true && newScale != initialScale)
        {
            colliderObj.SetActive(false);
            return;
        }
        if (colliderObj.activeInHierarchy == false && (newScale - initialScale).magnitude <= 1)
        {
            colliderObj.SetActive(true);
        }
    }
}
