using Unity.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum Axis{x, y}

public class DoorLogic : MonoBehaviour
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



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        initialPos = trans.position;
        rb2d = GetComponent<Rigidbody2D>();
        layerMask |= 1 << UnityEngine.LayerMask.NameToLayer("Moveable Stuff");
        collider = GetComponent<BoxCollider2D>();
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

    (Vector2 NewPos, Vector2 NewScale) Rescale(Vector2 distancesFromObjToMouse)
    {
        Vector2 newScale = initialScale;
        Vector2 newPos = initialPos;

        var s = FindScale(
            initialScaleOnAxis: ComponentRef(ref initialScale),
            maxScale: maxScaleX,
            distanceToMouse: ComponentRef(ref distancesFromObjToMouse));
        ComponentRef(ref newScale) = s;
        
        bool positive = distancesFromObjToMouse.x > 0 && isDirectionPositive;
        var p = FindPosition(
            ComponentRef(ref newScale),
            ComponentRef(ref initialPos), 
            ComponentRef(ref initialScale), 
            positive: positive);
        
        ComponentRef(ref newPos) = p;

        return (newPos, newScale);
    }

    void FixedUpdate()
    {
        if (objectSelected == false)
        {
            return;
        }


        var distancesFromObjToMouse = MousePositionHelper.FindDistancesToMouse(initialPos);
        if (!isDirectionPositive)
        {
            distancesFromObjToMouse = -distancesFromObjToMouse;
        }

        var n = Rescale(distancesFromObjToMouse);
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
}
