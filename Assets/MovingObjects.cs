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
    static Vector2 ObjectPosition(Rigidbody2D rb2d)
    {
        Transform trans = rb2d.transform;
        var localPositionWithScale = rb2d.centerOfMass;
        var localPositionWithoutScale = localPositionWithScale;
        var scale = trans.localScale;
        localPositionWithoutScale.x /= scale.x;
        localPositionWithoutScale.y /= scale.y;
        var ret = trans.localToWorldMatrix.MultiplyPoint3x4(localPositionWithoutScale);
        return ret;
    }

    void OnDrawGizmos()
    {
        if (movingObject == null)
        {
            return;
        }
        var rb2d = movingObject;
        Vector2 objectPosition = ObjectPosition(rb2d);
        Gizmos.DrawCube(objectPosition,new Vector3 (1,1,1));
    }

    void FixedUpdate()
    {
        if (!Input.GetMouseButton(1) || movingObject == null)
        {
            return;
        }
        var rb2d = movingObject;
        Vector2 objectPosition = ObjectPosition(rb2d);
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
        float distanceFromObjToMouseX = Mathf.Abs(mousePosition2D.x - objectPosition.x);
        float distanceFromObjToMouseY = Mathf.Abs(mousePosition2D.y - objectPosition.y);
        float distanceToObjectX = Mathf.Abs(player.transform.position.x - objectPosition.x);
        float distanceToMouseX =  Mathf.Abs(player.transform.position.x - mousePosition2D.x);
        float distanceToObjectY = Mathf.Abs(player.transform.position.y - objectPosition.y);
        float distanceToMouseY =  Mathf.Abs(player.transform.position.y - mousePosition2D.y);
        bool isObjectMovingX = true;
        bool isObjectMovingY = true;
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
