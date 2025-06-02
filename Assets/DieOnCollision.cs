using UnityEngine;

public class DieOnCollision : MonoBehaviour
{
    private BoxCollider2D hitBox;
    private Transform trans;
    private NonPlayerManageDamage damageScript;
    public float velocityToDie = 50;
    public Vector2 offset = new Vector2(1f, 1f);
    Vector2 size;
    int layerMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        damageScript = GetComponent<NonPlayerManageDamage>();
        hitBox = GetComponentInChildren<BoxCollider2D>();
        size = hitBox.bounds.size.ToVector2() + offset;
        foreach (var x in new[]
        {
            "Peopwe",
            "Moveable Stuff",
        })
        {
            var layer = LayerMask.NameToLayer(x);
            layerMask |= 1 << layer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = trans.position;
        var bodies = Physics2D.OverlapBoxAll(pos, size, 0, layerMask);
        foreach (Collider2D col in bodies)
        {
            Rigidbody2D rb2d = col.attachedRigidbody;
            if (rb2d.linearVelocity.magnitude >= velocityToDie)
            {
                damageScript.Die();
            }
        }
    }
}
