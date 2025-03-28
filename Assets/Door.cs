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
    public float maxScaleX;
    public bool opensToTheLeft;

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
        trans = GetComponent<Transform>();
        initialPos = trans.position;
        rb2d = GetComponent<Rigidbody2D>();
        layerMask |= 1 << UnityEngine.LayerMask.NameToLayer("Moveable Stuff");
        collider = GetComponent<BoxCollider2D>();
        initialBoundsX = collider.bounds.extents.x;
        initialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(1))
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
        var distanceToMouseX = mousePosition.x - initialPos.x;
        var neededScaleX = distanceToMouseX/initialBoundsX;
        Vector2 newPos = trans.position;
        
        bool shouldChangeScale = false;

        if (opensToTheLeft == false && neededScaleX > 0)
        {
            neededScaleX = Mathf.Clamp(neededScaleX, initialScale.x, maxScaleX);
            newPos = new Vector2(neededScaleX/2 + initialPos.x - initialScale.x/2, initialPos.y);
            shouldChangeScale = true;
        }

        if (opensToTheLeft == true && neededScaleX < 0)
        {
            neededScaleX = Mathf.Clamp(neededScaleX, -maxScaleX, -initialScale.x);
            newPos = new Vector2(neededScaleX/2 + initialPos.x + initialScale.x/2, initialPos.y);
            shouldChangeScale = true;
        }

        if (shouldChangeScale == false)
        {
            return;
        }

        var newScale = new Vector2(neededScaleX, initialScale.y);
        trans.localScale = newScale;

        trans.position = newPos;
        if (collider.enabled == true && newScale.x != initialScale.x)
        {
            collider.enabled = false;
            return;
        }
        if (collider.excludeLayers == 1)
        {
            collider.enabled = true;
        }
        Debug.Log(collider.excludeLayers.value);
    }
}
