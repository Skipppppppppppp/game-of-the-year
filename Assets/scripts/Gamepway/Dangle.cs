using UnityEngine;

public class Dangle : MonoBehaviour
{
    public Transform anchor;
    public float hangDistance;
    public float distanceChangeIncrement = 0.2f;
    private DistanceJoint2D joint;
    private Rigidbody2D rb;

    public void StartDangling()
    {
        rb = GetComponent<Rigidbody2D>();

        if (anchor != null)
        {
            joint = gameObject.AddComponent<DistanceJoint2D>();
            joint.enabled = true;
            joint.connectedBody = anchor.GetComponent<Rigidbody2D>();
            joint.autoConfigureDistance = false;
            joint.distance = hangDistance;
            joint.enableCollision = false;

            if (joint.connectedBody == null)
            {
                joint.connectedAnchor = anchor.position;
            }
        }
    }

    void FixedUpdate()
    {
        if (anchor == null)
            return;
            
        if (Input.GetKey(KeyCode.S))
        {
            joint.distance += distanceChangeIncrement;
        }

        if (Input.GetKey(KeyCode.W))
        {
            joint.distance -= distanceChangeIncrement;
        }
    }
}