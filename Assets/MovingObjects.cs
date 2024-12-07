using Unity.VisualScripting;
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
    private const float maxForInterpCoeff = 1;
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
        }
    }
    void FixedUpdate()
    {
        if (!Input.GetMouseButton(1) || movingObject == null)
        {
            return;
        }
        bool isObjectMovingX = false;
        bool isObjectMovingY = false;
        var trans = movingObject.transform;
        var rb2d = movingObject;
        Vector2 objectPosition = trans.position;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
        float distanceFromObjToMouseX = Mathf.Abs(mousePosition2D.x - objectPosition.x);
        float distanceFromObjToMouseY = Mathf.Abs(mousePosition2D.y - objectPosition.y);
        float distanceToObjectX = Mathf.Abs(player.transform.position.x - trans.position.x);
        float distanceToMouseX =  Mathf.Abs(player.transform.position.x - mousePosition2D.x);
        float distanceToObjectY = Mathf.Abs(player.transform.position.y - trans.position.y);
        float distanceToMouseY =  Mathf.Abs(player.transform.position.y - mousePosition2D.y);
        isObjectMovingX = true;
        isObjectMovingY = true;
        if (distanceToObjectX >= 12 && distanceToMouseX >= distanceToObjectX)
        {
            isObjectMovingX = false;
        }
        if (distanceToObjectY >= 12 && distanceToMouseY >= distanceToObjectY)
        {
            isObjectMovingY = false;
        }
        if (isObjectMovingX)
        {
            float directionX = mousePosition2D.x - objectPosition.x;
            float p = (distanceFromObjToMouseX - minForDistance) / (maxForDistance - minForDistance);
            float maxForInterp = maxForInterpCoeff/initialGravityScale;
            rb2d.linearVelocityX = (directionX*Mathf.Lerp(minForInterp, maxForInterp, p));
        }
        else
        {
            movingObject.linearVelocityX = 0f;
        }
        if (isObjectMovingY)
        {
            float directionY = mousePosition2D.y - objectPosition.y;
            float p = (distanceFromObjToMouseY - minForDistance) / (maxForDistance - minForDistance);
            float maxForInterp = maxForInterpCoeff/initialGravityScale;
            rb2d.linearVelocityY = (directionY*Mathf.Lerp(minForInterp, maxForInterp, p));
        }
        else
        {
            movingObject.linearVelocityY = 0f;
        }
    }
}
