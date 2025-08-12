using UnityEngine;

public class ImpaleOnCollision : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private Transform spearTrans;
    public bool? hitWall = null;
    public GameObject victim;
    private Rigidbody2D victimRB2D;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spearTrans = transform.parent.transform;
        rb2d = GetComponentInParent<Rigidbody2D>();
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
        rb2d.includeLayers |= (int)Layer.Default;
        spearTrans.position = victim.transform.position;
        hitWall = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (victimRB2D != null)
        {
            victimRB2D.linearVelocity = rb2d.linearVelocity;
        }
    }
}
