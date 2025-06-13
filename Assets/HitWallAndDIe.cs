using UnityEngine;

public class HitWallAndDIe : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private IDamageHandler damageScript;
    private float prevVelocity = 0;
    public float velocityChangeNeededToDie = 20;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        damageScript = GetComponent<IDamageHandler>();

        if (damageScript == null)
        {
            Debug.Log("add the damage script dumbass");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float velocity = rb2d.linearVelocity.magnitude;

        float differenceBetweenVelocities = Mathf.Abs(velocity - prevVelocity);
        prevVelocity = velocity;
        
        if (differenceBetweenVelocities < velocityChangeNeededToDie)
        {
            return;
        }

        float damageToGive = differenceBetweenVelocities / velocityChangeNeededToDie;
        damageScript.TakeDamage(damageToGive);
    }
}
