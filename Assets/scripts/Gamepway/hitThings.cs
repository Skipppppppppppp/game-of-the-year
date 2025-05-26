using System.Threading;
using UnityEngine;

public class hitThings : MonoBehaviour
{
    public AudioClip[] sounds;
    public AudioClip bigAttackSound;
    private Transform trans;
    private AudioSource audioSource;
    private int layerMask;
    private int obstacleLayerMask;
    public float maxDistance;
    private float hitTimer = 0;
    public float timeToResetHits = 1;
    private int hitNumber = 1;
    public int bigHitNumber = 3;
    public float usualDamage = 1;
    public float bigDamage = 3;
    private int prevSoundIndex = 10;
    private bool shouldAdvanceTimer = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        audioSource = GetComponent<AudioSource>();

        obstacleLayerMask = 1 << LayerMask.NameToLayer("Default");
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

        Rigidbody2D damageTakerRB2D = RaycastHelper.TryRaycastToMouse(
            playerPos: pos,
            maxDistance: maxDistance,
            layerMask: layerMask,
            obstacleLayerMask: obstacleLayerMask);

        if (damageTakerRB2D == null)
        {
            return;
        }

        IDamageHandler damageTaker = damageTakerRB2D.GetComponentInParent<IDamageHandler>();

        if (damageTaker == null)
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