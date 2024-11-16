using UnityEngine;

#nullable enable

public sealed class MovingObjects : MonoBehaviour
{
    private int Layer;
    public GameObject player;
    public float minForInterp;
    public float maxForInterp;
    public float minForDistance;
    public float maxForDistance;
    private Rigidbody2D? movingObject;
    private float initialGravityScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Layer = LayerMask.NameToLayer("Moveable Stuff");
    }

    private Rigidbody2D? RaycastForObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray,float.PositiveInfinity,1<<Layer);
        var trans = rayHit.transform;
        if (trans == null)
        {
            return null;
        }
        return rayHit.rigidbody;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetMouseButton(1))
        {
            if (movingObject == null)
            {
                return;
            }
            movingObject.gravityScale = initialGravityScale;
            movingObject = null;
            return;
        }

        if (movingObject == null)
        {
            var raycasted = RaycastForObject();
            if (raycasted == null)
            {
                return;
            }
            movingObject = raycasted;
            initialGravityScale = movingObject.gravityScale;
            movingObject.gravityScale = 0;
        }
        var trans = movingObject.transform;
        var rb2d = movingObject;
        Vector2 objectPosition = trans.position;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
        float distanceFromObjToMouse = (mousePosition2D - objectPosition).magnitude;
        float distanceToObject = Vector2.Distance(player.transform.position, trans.position);
        float distanceToMouse = Vector2.Distance(player.transform.position, mousePosition2D);
        bool isObjectMoving = true;
        if (distanceToObject >= 12 && distanceToMouse >= distanceToObject)
        {
            isObjectMoving = false;
        }
        if (isObjectMoving)
        {
            Vector2 direction = (mousePosition2D - objectPosition).normalized;
            float p = (distanceFromObjToMouse - minForDistance) / (maxForDistance - minForDistance);
            rb2d.linearVelocity = (direction*Mathf.Lerp(minForInterp, maxForInterp, p));
        }
        else
        {
            movingObject.linearVelocity = new Vector2 (0f,0f);
            movingObject.angularVelocity = 0;
        }
    }
}
