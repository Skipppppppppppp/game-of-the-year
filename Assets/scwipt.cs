using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private int chargeUpLeft = 20;
    private Rigidbody2D rb2d;
    private bool playerOnGround;
    private bool _tryingToJump;

    void Start()
    {
        directionIndicator.SetActive(false);
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        particleSystem = GetComponent<ParticleSystem>();
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        rb2d.drag = 4;
        playerOnGround = true;
        if (Input.GetKey(KeyCode.Space))
        {
            rb2d.drag = 1;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        playerOnGround = false;
        rb2d.drag = 1;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            directionIndicator.SetActive(true);
        }
        if (chargeUpLeft > 0 && Input.GetKeyUp(KeyCode.E))
        {
            directionIndicator.SetActive(false);
            chargeUpLeft = 20;
        }
        if (chargeUpLeft <= 0 && Input.GetKeyUp(KeyCode.E))
        {
            chargeUpLeft = 20;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - transform.position).normalized;
            rb2d.velocity = direction * 60;
            if (playerOnGround == true)
            {
                StartCoroutine(EmitParticlesForDuration(0.1f));
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
        if (Input.GetKey(KeyCode.E))
        {
            chargeUpLeft -= 1;
        }
        if (playerOnGround) // player physics for when player is on the ground cause I want different physics from whn on ground and airborne
        {
            if (_tryingToJump)
            {
                rb2d.AddForce(up * 1000);
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb2d.AddForce(right * 50);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb2d.AddForce(left * 50);
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 3, destroyableLayer);

            if (hit.collider != null)
            {
                Destroy(hit.collider.gameObject);
            }
            if (hitNumber == 3)
            {
                audioSource.PlayOneShot(critSound, 1f);
                hitNumber = 1;
            }
            else
            {
                PlayRandomSound();
                hitNumber += 1;
            }
        }
        if (playerOnGround == false) // player physics when airborne
        {
            if (Input.GetKey(KeyCode.D))
            {
                rb2d.AddForce(right * 4);
            }
            if (Input.GetKey(KeyCode.A))
            {
                rb2d.AddForce(left * 4);
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
