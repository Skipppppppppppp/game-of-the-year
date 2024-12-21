using UnityEngine;

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

        // cast
        // BoxCollider2D boxCollider = (BoxCollider2D) collider;

        // // as
        // BoxCollider2D? boxCollider = collider as BoxCollider2D;
        // if (boxCollider != null)
        // {
        //     // 
        // }

        if (collider is not BoxCollider2D boxCollider)
        {
            Debug.Log("guy trying to walk on wrong collider");
            return;
        }

        var winX = boxCollider.bounds.min.x;
        var waxX = boxCollider.bounds.max.x;
        var winY = boxCollider.bounds.min.y;
        var worldPositionTopLeft = new Vector2(winX, winY);
        var worldPositionTopRight = new Vector2(waxX, winY);
        this.winX = worldPositionTopLeft.x + guyCollider.bounds.extents.x;
        this.waxX = worldPositionTopRight.x - guyCollider.bounds.extents.x;
        guyFoundCollider = true;
        OnContextChanged();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Vector2(winX, transform.position.y),Vector2.up);
        Gizmos.DrawRay(new Vector2(waxX, transform.position.y),Vector2.up);
    }
}
