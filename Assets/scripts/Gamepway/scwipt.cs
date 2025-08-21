using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Content;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting.IonicZip;

public class scwipt : MonoBehaviour
{
    public GameObject directionIndicator;
    public AudioClip sound1;
    public AudioClip sound2;
    public AudioClip critSound;
    public static Vector2 right = Vector2.right;
    public static Vector2 left = Vector2.left;
    public static Vector2 up = Vector2.up;
    public LayerMask destroyableLayer;
    private AudioSource audioSource;
    private ParticleSystem particleSystem;
    private int hitNumber = 1;
    const int maxCharges = 20;
    private int chargeUpLeft = maxCharges;
    private Rigidbody2D rb2d;
    private bool playerOnGround;
    private bool _tryingToJump;
    public float initialHp = 100;
    public float hp;
    public float speedCoeffGround;
    public float speedCoeffAir;
    public float jumpForceCoeff;
    public float dampingCoeffGround;
    public float dampingCoeffAir;
    public float dashCoeffGround;
    public float dashCoeffAir;
    private Transform trans;
    public LayerMask portalLayerMask;
    private float pwayerZ;
    public BoxCollider2D playerCollider;

    void Start()
    {
        directionIndicator.SetActive(false);
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        particleSystem = GetComponent<ParticleSystem>();
        trans = GetComponent<Transform>();
        portalLayerMask |= LayerMask.Portals;
        pwayerZ = trans.position.z;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!RaycastHelper.OnGround(playerCollider))
        {
            return;
        }

        if (rb2d.linearVelocity.magnitude <= 30)
            rb2d.linearDamping = dampingCoeffGround;
        else
            rb2d.linearDamping = dampingCoeffAir;

        playerOnGround = true;

        if (_tryingToJump)
        {
            rb2d.linearDamping = dampingCoeffAir;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        // if (RaycastHelper.OnGround(playerCollider))
        // {
        //     return;
        // }

        playerOnGround = false;
        rb2d.linearDamping = dampingCoeffAir;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            directionIndicator.SetActive(true);
            var scaleProgress = ((float)chargeUpLeft) / maxCharges;
            var f = 1 - Mathf.Clamp01(scaleProgress);
            directionIndicator.transform.localScale = new Vector3(f, f, 1);
        }
        if (chargeUpLeft > 0 && Input.GetKeyUp(KeyCode.E))
        {
            directionIndicator.SetActive(false);
            chargeUpLeft = maxCharges;
        }
        if (chargeUpLeft <= 0 && Input.GetKeyUp(KeyCode.E))
        {
            chargeUpLeft = maxCharges;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - trans.position).normalized;
            if (playerOnGround == true)
            {
                rb2d.AddForce(direction * dashCoeffGround);
                StartCoroutine(EmitParticlesForDuration(0.1f));
            }
            else
            {
                rb2d.AddForce(direction * dashCoeffAir);
            }
            directionIndicator.SetActive(false); 
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _tryingToJump = true;
        }
    }
    void FixedUpdate()
    {
        if (trans.rotation.z > 0.38f)
        {
            trans.rotation = new Quaternion(0, 0, 0.38f, trans.rotation.w);
        }
        if (trans.rotation.z < -0.38f)
        {
            trans.rotation = new Quaternion(0, 0, -0.38f, trans.rotation.w);
        }
        if (Input.GetKey(KeyCode.E))
        {
            chargeUpLeft -= 1;
        }
        if (playerOnGround) // player physics for when player is on the ground cause I want different physics from whn on ground and airborne
        {
            if (_tryingToJump)
            {
                rb2d.AddForce(up * jumpForceCoeff);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb2d.AddForce(right * speedCoeffGround);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb2d.AddForce(right * -speedCoeffGround);
            }
        }
        if (playerOnGround == false) // player physics when airborne
        {
            if (Input.GetKey(KeyCode.D))
            {
                rb2d.AddForce(right * speedCoeffAir);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb2d.AddForce(right * -speedCoeffAir);
            }
        }
        _tryingToJump = false;
    }

    private IEnumerator EmitParticlesForDuration(float duration)
    {
        particleSystem.Play();
        yield return new WaitForSeconds(duration);
        particleSystem.Stop();
    }

    private void PlayRandomSound()
    {
        AudioClip[] sounds = { sound1, sound2 };
        AudioClip randomSound = sounds[Random.Range(0, sounds.Length)];
        audioSource.PlayOneShot(randomSound, 1f);
    }
}
