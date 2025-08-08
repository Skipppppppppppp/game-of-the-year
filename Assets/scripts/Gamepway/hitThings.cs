using System.Threading;
using UnityEngine;
using System.Collections.Generic;

public class hitThings : MonoBehaviour
{
    public AudioClip[] sounds;
    public AudioClip bigAttackSound;
    private Transform trans;
    private AudioSource audioSource;
    private LayerMask layerMask;
    private LayerMask obstacleLayerMask;
    public float maxDistance;
    private float hitTimer = 0;
    public float timeToResetHits = 1;
    private int hitNumber = 1;
    public int bigHitNumber = 3;
    public float usualDamage = 1;
    public float bigDamage = 3;
    public float baseballZoneHeight;
    public float baseballCoeff = 2000;
    private ExcludedValues prevSoundIndex = ExcludedValues.None;
    private bool shouldAdvanceTimer = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        audioSource = GetComponent<AudioSource>();

        obstacleLayerMask = LayerMask.Default;
        layerMask = LayerMask.Peopwe | LayerMask.MoveableStuff;
    }

    // Update is called once per frame
    void Update()
    {
        // if you hit the air, the amount of hits doesn't increase so you can't hit air and 
        // then do a big attack
        // if you hit an opponent the number increases
        // if you hit an opponent and then hit air the number doesn't increase or decrease
        // because idk i just wanted it this way, might change

        if (hitTimer >= timeToResetHits)
        {
            hitNumber = 1;
            hitTimer = 0;
            shouldAdvanceTimer = false;
        }

        if (shouldAdvanceTimer)
        {
            hitTimer += Time.deltaTime;
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        hitTimer = 0;
        shouldAdvanceTimer = true;

        RandomStuffHelper.playRandomSound(sounds, audioSource, ref prevSoundIndex);

        Vector2 pos = trans.position;

        Rigidbody2D[] knockedBackRB2Ds = RaycastHelper.RotatedOverlapBox(
            playerPos: pos,
            boxWidth: maxDistance,
            boxHeight: baseballZoneHeight,
            layerMask: (int) layerMask,
            obstacleLayerMask: (int) obstacleLayerMask);

        foreach (var g in knockedBackRB2Ds)
        {
            Vector2 distanceToMouse = MousePositionHelper.FindDistancesToMouse(pos);
            Vector2 forceToAdd = distanceToMouse.normalized * baseballCoeff;

            if (g.GetComponent<explodeAfterTime>() == null)
                forceToAdd = forceToAdd / 8;
            g.AddForce(forceToAdd);
            print(g.gameObject);
        }

        Rigidbody2D[] damageTakerRB2Ds = RaycastHelper.TryRaycastAllToMouse(
            playerPos: pos,
            maxDistance: maxDistance,
            layerMask: (int) layerMask,
            obstacleLayerMask: (int) obstacleLayerMask);

        if (damageTakerRB2Ds == null)
        {
            return;
        }

        IDamageHandler damageTaker = null;
        float prevDistanceToTaker = float.PositiveInfinity;

        foreach (Rigidbody2D rb2d in damageTakerRB2Ds)
        {
            IDamageHandler damageTakingScript = rb2d.GetComponentInParent<IDamageHandler>();

            if (damageTakingScript == null)
            {
                continue;
            }

            Transform transTaker = rb2d.transform;
            Vector2 takerPos = transTaker.position;
            float distanceToPlayer = (pos - takerPos).magnitude;

            if (damageTaker is null || distanceToPlayer < prevDistanceToTaker)
            {
                damageTaker = damageTakingScript;
                prevDistanceToTaker = distanceToPlayer;
            }
        }

        if (damageTaker is null)
        {
            return;
        }

        if (hitNumber == bigHitNumber)
        {
            damageTaker.TakeDamage(bigDamage);
            audioSource.PlayOneShot(bigAttackSound);
            hitNumber = 1;
            return;
        }

        damageTaker.TakeDamage(usualDamage);
        hitNumber += 1;
    }
}

public interface IDamageHandler
{
    void TakeDamage(float damageAmount);
}