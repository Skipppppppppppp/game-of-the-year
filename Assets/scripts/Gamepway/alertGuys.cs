using Codice.Client.BaseCommands;
using TMPro;
using UnityEngine;

public class alertGuys : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private Transform trans;
    [Range(1,20)] public float detectionRadius;
    [Range(1,10)] public float differenceBetweenRadii;
    private float totalRadius;
    private int guyLayerMask;
    private int wallLayerMask;
    private FacePlayer facePlayerScript;
    private WatchAndDamagePlayer guyDamageScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
        trans = rb2d.transform;
        guyLayerMask |= 1 << LayerMask.NameToLayer("Peopwe");
        wallLayerMask |= 1 << LayerMask.NameToLayer("Default");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        totalRadius = detectionRadius + differenceBetweenRadii;
        var hits = Physics2D.OverlapCircleAll(trans.position, totalRadius, guyLayerMask);
        foreach (Collider2D i in hits)
        {
            var guyTrans = i.transform.parent;

            if (guyTrans == null)
            {
                return;
            }

            float distanceToGuy = (trans.position - guyTrans.position).magnitude;

            var guy = guyTrans.gameObject;
            var guyWatchPlayerScript = guy.GetComponent<watchplayer>();

            if (guyWatchPlayerScript == null)
            {
                return;
            }

            if (distanceToGuy > detectionRadius)
            {
                guyWatchPlayerScript.awareOfPlayer = false;
                continue;
            }

            Vector2 direction = (guyTrans.position - trans.position).normalized;
            RaycastHit2D wall = Physics2D.Raycast(trans.position, direction, distanceToGuy, wallLayerMask);

            if (wall.collider != null)
            {
                guyWatchPlayerScript.playerInSight = false;
                continue;
            }

            guyWatchPlayerScript.awareOfPlayer = true;
            guyWatchPlayerScript.playerInSight = true;
            facePlayerScript = guy.GetComponentInChildren<FacePlayer>();

            if (facePlayerScript == null)
            {
                return;
            }

            facePlayerScript.pwayerTrans = trans;

            guyDamageScript = guy.GetComponent<WatchAndDamagePlayer>();

            if (guyDamageScript == null)
            {
                return;
            }

            guyDamageScript.healthScript = GetComponent<ManageDamage>();            
        }
    }

        void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, totalRadius);
    }

}
