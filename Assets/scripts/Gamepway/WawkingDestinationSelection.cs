using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class WawkingDestinationSelection : DestinationSelection
{
    private float waxX;
    private float winX;
    private BoxCollider2D guyCollider;
    public bool guyCanWalk;
    public override bool IsInitializedForDestinationSelection => guyCanWalk;
    public float guyBoxCastYOffset = 0.1f;
    public bool guyInAir;
    public float lastRememberedPlayerX;
    private Rigidbody2D rb2d;
    private bool prevOnGroundState = false;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        guyCollider = GetComponentInChildren<BoxCollider2D>();
    }

    public override Vector2 SelectDestination(float currentY)
    {
        const int maxIterCount = 100;
        for (int i = 0; i < maxIterCount; i++)
        {
            float newMinX = winX;
            float newMaxX = waxX;
            float newX = Random.Range(newMinX, newMaxX);
            Vector2 newDestination = new Vector2(newX, currentY);

            float currentGuyX = transform.position.x;
            float currentDistanceToPwayer = Mathf.Abs(currentGuyX - lastRememberedPlayerX);
            float futureDistanceToPlayer = Mathf.Abs(newDestination.x - lastRememberedPlayerX);
            if (futureDistanceToPlayer < currentDistanceToPwayer)
            {
                continue;
            }
            return newDestination;
        }

        Debug.LogWarning("Iteration limit exceeded when generating random numbers??");
        return default;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        var guyPosition = guyCollider.transform.position;

        // cast
        // BoxCollider2D boxCollider = (BoxCollider2D) collider;

        // // as
        // BoxCollider2D? boxCollider = collider as BoxCollider2D;
        // if (boxCollider != null)
        // {
        //     // 
        // }

        if (!RaycastHelper.OnGround(guyCollider))
        {
            return;
        }

        prevOnGroundState = true;

        if (prevOnGroundState == true)
            return;

        Collider2D collider = collision.collider;
        if (collider is not BoxCollider2D boxCollider)
        {
            Debug.Log("guy trying to walk on wrong collider");
            return;
        }

        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 collisionPoint = collision.GetContact(i).point;
            float guyBottomY = guyPosition.y - guyCollider.bounds.extents.y;
            float error = Mathf.Abs(collisionPoint.y - guyBottomY);
            float maxError = 0.1f;
            if(error > maxError)
            {
                return;
            }
        }

        var winX = boxCollider.bounds.min.x;
        var waxX = boxCollider.bounds.max.x;
        var winY = boxCollider.bounds.min.y;
        var worldPositionTopLeft = new Vector2(winX, winY);
        var worldPositionTopRight = new Vector2(waxX, winY);
        this.winX = worldPositionTopLeft.x + guyCollider.bounds.extents.x;
        this.waxX = worldPositionTopRight.x - guyCollider.bounds.extents.x;

        {
            float distanceFromGuyToRightEdge = waxX - guyPosition.x;
            float distanceFromGuyToLeftEdge = guyPosition.x - winX;

            Vector2 rayOrigin = new Vector2 (guyPosition.x, guyPosition.y + guyBoxCastYOffset);

            RaycastHit2D rightObstacle = Physics2D.BoxCast(
                origin: rayOrigin,
                size: guyCollider.bounds.size,
                angle: 0,
                direction: Vector2.right,
                distance: distanceFromGuyToRightEdge,
                layerMask: (int) LayerMask.Default);


            var leftObstacle = Physics2D.BoxCast(
                origin: rayOrigin,
                size: guyCollider.bounds.size,
                angle: 0,
                direction: Vector2.left,
                distance: distanceFromGuyToLeftEdge,
                layerMask: (int) LayerMask.Default);

            if (rightObstacle.transform != null)
            {
                float rightBorderX = rightObstacle.transform.position.x - rightObstacle.collider.bounds.extents.x;
                this.waxX = rightBorderX - guyCollider.bounds.extents.x;
            }
            if (leftObstacle.transform != null)
            {
                float leftBorderX = leftObstacle.transform.position.x + leftObstacle.collider.bounds.extents.x;
                this.winX = leftBorderX + guyCollider.bounds.extents.x;
            }

        }

        guyInAir = false;
        guyCanWalk = true;
        OnContextChanged();
    }

void OnCollisionExit2D(Collision2D collision)
{
    if (rb2d.linearVelocityY == 0)
    {
        return;
    }

    prevOnGroundState = false;
    guyInAir = true;
    guyCanWalk = false;
}

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Vector2(winX, transform.position.y),Vector2.up);
        Gizmos.DrawRay(new Vector2(waxX, transform.position.y),Vector2.up);
    }
}
