using Unity.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
    public string axis;
    public bool isDirectionPositive;
    // doors don't break properly if they can be opened to the left?? like they just disappear????? hi???

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

    float FindScale(string axis, bool positiveOrNegative, float initialScaleOnAxis, float maxScale)
    {
        Vector2 currentScale = trans.localScale;
        Vector2 newPos;
        float neededScale = 0;

        if (axis.ToLower() == "x")
        {
            var distanceToMouseX = mousePosition.x - initialPos.x;
            neededScale = distanceToMouseX/initialScale.x;
        }

        if (axis.ToLower() == "y")
        {
            var distanceToMouseY = mousePosition.y - initialPos.y;
            neededScale = distanceToMouseY/initialScale.y;
        }

        if (positiveOrNegative)
        {
            neededScale = Mathf.Clamp(neededScale, initialScaleOnAxis, maxScale);
        }

        if (!positiveOrNegative)
        {
            neededScale = Mathf.Clamp(neededScale, -maxScale, -initialScale.x);
        }

        if (Mathf.Abs(neededScale - initialScaleOnAxis) <= 0.1)
        {
            neededScale = initialScaleOnAxis;
        }

        return neededScale;
    }

    float FindPosition(float scaleOnAxis, float initialCoord, float initialScaleOnAxis)
    {
        float newCoord = 0;

        if (scaleOnAxis > 0)
        {
            newCoord = scaleOnAxis/2 + initialCoord - initialScaleOnAxis/2;
            return newCoord;
        }
        
        newCoord = scaleOnAxis/2 + initialCoord + initialScaleOnAxis/2;
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

        Vector2 newScale = new Vector2();
        Vector2 newPos = new Vector2();

        if (axis == "x")
        {
            newScale = new Vector2(FindScale("x", isDirectionPositive, initialScale.x, maxScaleX), initialScale.y);
            newPos = new Vector2(FindPosition(newScale.x, initialPos.x, initialScale.x), initialPos.y);
        }

        if (axis == "y")
        {
            newScale = new Vector2(initialScale.x, FindScale("y", isDirectionPositive, initialScale.y, maxScaleY));
            newPos = new Vector2(initialPos.x, FindPosition(newScale.y, initialPos.y, initialScale.y));
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
