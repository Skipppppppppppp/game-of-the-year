using UnityEngine;

public class ImpaleOnCollision : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private Transform trans;
    public bool? hitWall = null;
    public GameObject victim;
    private Rigidbody2D victimRB2D;
    private Transform transVictim;
    public MovingObjects movingObjectsScript;
    private Vector2 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        rb2d = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hitWall != null)
            return;

        victim = collision.gameObject;
        victimRB2D = collision.rigidbody;

        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        if (victim.layer == (int) Layer.Default)
        {
            hitWall = true;
            return;
        }

        rb2d.excludeLayers = ~0;
        rb2d.includeLayers |= (int) Layer.Default;
        transVictim = victim.transform;

        offset = trans.position - transVictim.position;

        hitWall = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (victimRB2D == null)
            return;

        if (victim.layer == (int)Layer.Default)
            return;

        if (movingObjectsScript.movingObject == victimRB2D)
            return;

        Vector2 victimPos = transVictim.position;
        victimRB2D.linearVelocity = rb2d.linearVelocity;
        transform.position = victimPos + offset;
    }
}
