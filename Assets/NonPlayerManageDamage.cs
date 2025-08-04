using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;

public class NonPlayerManageDamage : MonoBehaviour, IDamageHandler
{
    public float initialHP = 3;
    public float hp = 3;
    private GameObject thisGameObject;
    public AudioClip[] sounds;
    public AudioClip[] deathSounds;
    private AudioSource audioSource;
    int prevDamageSoundIndex = 0;
    public bool isGuy = false;
    private Transform trans;

    public void TakeDamage(float amount)
    {
        hp = hp - amount;
        UpdateHP(hp);
    }

    public void Die()
    {
        if (isGuy == false)
        {
            Break breakScript = GetComponent<Break>();
            if (breakScript != null)
            {
                breakScript.VeryFunMeshThings();
            }
        }

        RandomStuffHelper.playRandomSound(deathSounds, audioSource);

        if (isGuy)
        {
            var col = GetComponentInChildren<BoxCollider2D>();
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            Vector2 extents = col.bounds.extents;
            CreateHalfGuy(extents, 1);
            CreateHalfGuy(extents, -1);
 
            Destroy(thisGameObject);
        }
    }

    private void CreateHalfGuy(Vector2 guyExtents, int topOrBottom)
    {
        GameObject halfGuy = new GameObject("Half a Guy");

        if (topOrBottom == 1)
        {
            halfGuy.layer = 7;
        }
        else
        {
            halfGuy.layer = 3;
        }

        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        SpriteRenderer halfSpriteRenderer =
        halfGuy.AddComponent<SpriteRenderer>();

        halfSpriteRenderer.sprite = spriteRenderer.sprite;
        halfSpriteRenderer.color = spriteRenderer.color; // TEMPORARY

        Vector2 newPos = new Vector2(trans.position.x, trans.position.y + guyExtents.y / 2 * topOrBottom);
        halfGuy.transform.position = newPos;

        Vector2 newScale = new Vector2(guyExtents.x * 2, guyExtents.y);
        halfGuy.transform.localScale = newScale;

        Rigidbody2D halfRB2D = halfGuy.AddComponent<Rigidbody2D>();
        halfRB2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        Rigidbody2D guyRB2D = GetComponent<Rigidbody2D>();

        halfRB2D.linearVelocity = guyRB2D.linearVelocity;
        halfRB2D.mass = guyRB2D.mass / 2;
        halfRB2D.angularDamping = guyRB2D.angularDamping;

        if (topOrBottom == -1)
        {
            halfRB2D.excludeLayers = (int) LayerMask.Pwayer;
        }

        RememberInitialProperties rememberedPropertiesScript = GetComponent<RememberInitialProperties>();

        if (rememberedPropertiesScript == null)
        {
            halfRB2D.linearDamping = guyRB2D.linearDamping;
        }
        else
        {
            RememberedProperties initialProps = rememberedPropertiesScript.Props;
            halfRB2D.linearDamping = initialProps.LinearDamping;
        }

        halfGuy.AddComponent<BoxCollider2D>();
    }

    // should add changing images, speed and MAYBE IN THE FUTURE behaviour when hp changes (basically phases)
    // and also make it so bro screams when gets hit / dies (loser)
    // will do when I have images and sounds and also when I'm not lazy

    // 24.05.25 im not lazy now but i dont have images and/or sounds

    void UpdateHP(float currentHP)
    {
        if (currentHP > 0)
        {
            return;
        }

        Die();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        thisGameObject = trans.gameObject;
        audioSource = GetComponent<AudioSource>();
    }
}
