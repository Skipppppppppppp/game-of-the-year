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
    public float maxDistanceForInterp = 30;
    public float minDistanceForInterp = 5;
    public float maxCoeffForInterp = 1000;
    public float minCoeffForIntetp = 1;

    private void ResetCooldown()
    {
        spearCD.Reset();
        cdOver = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (recallingSpear)
        {
            Vector2 distanceToPlayer = transform.position - spearTrans.position;

            if (distanceToPlayer.magnitude <= 1.5)
            {
                Destroy(spearTrans.gameObject);
                ResetCooldown();

                recallingSpear = false;
                spearRB2D = null;
                spearTrans = null;
                return;
            }
            
            // if (spearRB2D.linearVelocity.magnitude <= 10)
            // {
            float t = Mathf.Clamp((distanceToPlayer.magnitude - minDistanceForInterp) / maxDistanceForInterp, 0, 1);
            float spearRecallInterp = Mathf.Lerp(minCoeffForIntetp, maxCoeffForInterp, t);
            Vector2 direction = distanceToPlayer.normalized;
            spearRB2D.linearVelocity = direction * spearRecallInterp;
            Rotation.RotateObjectToTarget(transform.position, spearTrans);
            // }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spearCD.Update() && cdOver == false)
            cdOver = true;

        if (spearTrans != null && RaycastHelper.PathObstructed(transform.position, spearTrans.position, (int)LayerMask.Default))
        {
            recallingSpear = true;
            spearRB2D.excludeLayers = ~0;
            spearRB2D.constraints = RigidbodyConstraints2D.None;
            ResetCooldown();
            return;
        }

        if (!cdOver)
            return;

        if (!Input.GetKey(KeyCode.Q))
            return;

        ResetCooldown();

        if (spearTrans == null)
        {
            Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Quaternion targetRot = Rotation.GetRotationToTarget(targetPos, transform);
            GameObject summonedSpear = Instantiate(spear, position: transform.position, rotation: targetRot);
            spearRB2D = summonedSpear.GetComponent<Rigidbody2D>();
            spearTrans = summonedSpear.transform;

            Vector2 direction = MousePositionHelper.FindDistancesToMouse(transform.position).normalized;
            spearRB2D.AddForce(direction * spearForceCoeff);
            spearRB2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            return;
        }

        recallingSpear = true;

        spearRB2D.constraints = RigidbodyConstraints2D.None;
        Vector2 directionToPlayer = (transform.position - spearTrans.position).normalized;
        // spearRB2D.AddForce(directionToPlayer * pullForceCoeff);
    }
}
