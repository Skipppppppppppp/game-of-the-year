using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class WawkingDestinationSelection : DestinationSelection
{
    private float waxX;
    private float winX;
    private BoxCollider2D guyCollider;
    private bool guyFoundCollider;
    public override bool IsInitializedForDestinationSelection => guyFoundCollider;

    void Start()
    {
        guyCollider = GetComponentInChildren<BoxCollider2D>();
    }

    public override Vector2 SelectDestination(float currentY)
    {

        float newMinX = winX;
        float newMaxX = waxX;
        float newX = Random.Range(newMinX, newMaxX);
        Vector2 newDestination = new Vector2(newX, currentY);
        return newDestination;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        var guyPosition = guyCollider.transform.position;

        // cast
        // BoxCollider2D boxCollider = (BoxCollider2D) collider;

        // // as
        // BoxCollider2D? boxCollider = collider as BoxCollider2D;
        // if (boxCollider != null)
        // {
        //     // 
        // }

        if (collider.gameObject.layer != 0)
        {
            return;
        }

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


            RaycastHit2D rightObstacle = Physics2D.BoxCast(
                origin: guyPosition,
                size: guyCollider.bounds.size,
                angle: 0,
                direction: Vector2.right,
                distance: distanceFromGuyToRightEdge,
                layerMask: LayerMask.GetMask("Default"));


            var leftObstacle = Physics2D.BoxCast(
                origin: guyPosition,
                size: guyCollider.bounds.size,
                angle: 0,
                direction: Vector2.left,
                distance: distanceFromGuyToLeftEdge,
                layerMask: LayerMask.GetMask("Default"));
                
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


        guyFoundCollider = true;
        OnContextChanged();
    }

void OnCollisionExit2D()
{
    guyFoundCollider = false;
}

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Vector2(winX, transform.position.y),Vector2.up);
        Gizmos.DrawRay(new Vector2(waxX, transform.position.y),Vector2.up);
    }
}
