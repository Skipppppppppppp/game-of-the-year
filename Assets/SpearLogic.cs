using System.Threading;
using UnityEngine;

public class SpearLogic : MonoBehaviour
{
    public float pullForceCoeff;
    public GameObject spear;
    public float spearForceCoeff = 2000;
    public float spearRecallCoeff = 50;
    public TimerUtility spearCD;
    private Rigidbody2D spearRB2D;
    private Transform spearTrans;
    private bool recallingSpear = false;
    private bool cdOver = false;
    private ImpaleOnCollision spearScript;
    public MovingObjects movingObjectsScript;

    private void ResetCooldown()
    {
        spearCD.Reset();
        cdOver = false;
    }

    private void StartPulling()
    {
        recallingSpear = true;

        HurtVictim();

        spearRB2D.constraints = RigidbodyConstraints2D.None;
        spearRB2D.includeLayers = (int) LayerMask.Default;
        if (spearRB2D.excludeLayers == ~0)
            spearRB2D.excludeLayers = 0;

        Vector2 directionToPlayer = (transform.position - spearTrans.position).normalized;
        spearRB2D.AddForce(directionToPlayer * pullForceCoeff);
    }

    private void HurtVictim()
    {
        GameObject victim = spearScript.victim;
        if (victim == null)
            return;
        IDamageHandler damageScript = victim.GetComponent<IDamageHandler>();
        if (victim.layer != (int)Layer.Peopwe && damageScript != null)
            damageScript.TakeDamage(10000);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (recallingSpear == false)
            return;

        Vector2 distanceToPlayer = transform.position - spearTrans.position;

        if (distanceToPlayer.magnitude <= 1.5)
        {
            Destroy(spearTrans.gameObject);
            ResetCooldown();

            recallingSpear = false;
            spearRB2D = null;
            spearTrans = null;
            spearScript = null;
            return;
        }

        Rotation.RotateObjectToTarget(transform.position, spearTrans);
        if (spearRB2D.linearVelocity.magnitude <= 10)
        {
            // float t = Mathf.Clamp((distanceToPlayer.magnitude - minDistanceForInterp) / maxDistanceForInterp, 0, 1);
            // float spearRecallInterp = Mathf.Lerp(minCoeffForIntetp, maxCoeffForInterp, t);
            Vector2 direction = distanceToPlayer.normalized;
            spearRB2D.AddForce(direction * spearRecallCoeff);
            // spearRB2D.linearVelocity = direction * spearRecallInterp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spearCD.Update() && cdOver == false)
            cdOver = true;


        if (spearTrans != null && recallingSpear == false && spearScript.hitWall != null)
        {
            if (spearScript.victim == null || RaycastHelper.PathObstructed(transform.position, spearTrans.position, (int)LayerMask.Default | (int)LayerMask.Doors))
            {
                StartPulling();
                spearRB2D.excludeLayers = ~0;
                spearRB2D.gravityScale = 0;
                return;
            }
        }

        if (!cdOver)
            return;

        if (!Input.GetKey(KeyCode.Q))
            return;

        ResetCooldown();

        if (recallingSpear == true)
        {
            Vector2 directionToPlayer = (transform.position - spearTrans.position).normalized;
            spearRB2D.AddForce(directionToPlayer * pullForceCoeff);
            return;
        }

        if (spearTrans == null)
        {
            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Quaternion targetRot = Rotation.GetRotationToTarget(targetPos, transform);
            GameObject summonedSpear = Instantiate(spear, position: transform.position, rotation: targetRot);
            spearScript = summonedSpear.GetComponent<ImpaleOnCollision>();
            spearRB2D = summonedSpear.GetComponent<Rigidbody2D>();
            spearTrans = summonedSpear.transform;
            spearScript.movingObjectsScript = movingObjectsScript;

            Vector2 direction = MousePositionHelper.FindDistancesToMouse(transform.position).normalized;
            spearRB2D.AddForce(direction * spearForceCoeff);
            spearRB2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            return;
        }

        StartPulling();
    }
}
