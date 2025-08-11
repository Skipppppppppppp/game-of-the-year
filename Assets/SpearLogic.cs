using System.Threading;
using UnityEngine;

public class SpearLogic : MonoBehaviour
{
    public GameObject spear;
    public float spearForceCoeff = 2000;
    public float spearRecallCoeff = 50;
    // public float maxDistanceForInterp = 30;
    public TimerUtility spearCD;
    private Rigidbody2D spearRB2D;
    private Transform spearTrans;
    private bool recallingSpear = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (recallingSpear)
        {
            Vector2 distanceToPlayer = transform.position - spearTrans.position;
            if (distanceToPlayer.magnitude > 1)
            {
                // float t = Mathf.Clamp(distanceToPlayer.magnitude / maxDistanceForInterp, 0, 1);
                // float spearRecallInterp = 1 / Mathf.Lerp(0, 1, t);
                Vector2 direction = distanceToPlayer.normalized;
                spearRB2D.linearVelocity = direction * spearRecallCoeff;
                Rotation.RotateObjectToTarget(transform.position, spearTrans);
                return;
            }

            Destroy(spearTrans.gameObject);

            recallingSpear = false;
            spearRB2D = null;
            spearTrans = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!spearCD.Update())
            return;

        if (!Input.GetKey(KeyCode.Q))
            return;

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
        spearRB2D.excludeLayers = ~0;
        spearRB2D.constraints = RigidbodyConstraints2D.None;
    }
}
