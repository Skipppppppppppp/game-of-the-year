using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InfectGuy : MonoBehaviour
{
    private GameObject guy;
    private Vector2 guyPos;
    private Transform trans;
    private Transform guyTrans;
    private Vector2 pos;
    private Collider2D guyCollider;
    private ManageDamage healthScript;
    private float guyYExtents;
    private int layerMask;
    private float timer;
    public float animationDuration = 2;
    public float healthToGive = 40;
    private ParticleSystem pawticleSystem;

    public float infectionRadius = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var scwipt = GetComponentInParent<scwipt>();
        trans = scwipt.gameObject.GetComponent<Transform>();
        layerMask |= 1 << LayerMask.NameToLayer("Peopwe");
        healthScript = GetComponentInParent<ManageDamage>();
        pawticleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer == 0)
        {
            pos = trans.position;
            guyCollider = Physics2D.OverlapCircle(pos, infectionRadius, layerMask);
            if (guyCollider == null)
            {
                timer = 0;
                return;
            }
        }

        if (guy == null)
        {
            guy = guyCollider.gameObject;
            guyTrans = guy.GetComponent<Transform>();
            guyYExtents = guyCollider.bounds.extents.y;
            StartCoroutine(EmitParticlesForDuration(animationDuration));
        }

        var guyPos = guyTrans.position;

        if (timer < animationDuration)
        {
            float newY = guyPos.y + guyYExtents;
            trans.position = new Vector2(guyPos.x, newY);
            timer += Time.deltaTime;
            return;
        }

        Destroy(guy);
        healthScript.AddHealth(healthToGive);

        timer = 0;
    }

    private IEnumerator EmitParticlesForDuration(float duration)
    {
        pawticleSystem.Play();
        yield return new WaitForSeconds(duration);
        pawticleSystem.Stop();
    }
}