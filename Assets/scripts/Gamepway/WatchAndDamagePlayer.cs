using UnityEditor.Rendering;
using UnityEngine;

public class WatchAndDamagePlayer : watchplayer
{
    public float reloadTime;
    public int amountOfShots;
    public float timeBetweenShots;
    private int currentShot = 1;
    private float reloadTimer = 0;
    private float timeSinceLastShot = 0;
    private AudioSource audioSource;
    public AudioClip gunshot;
    private Rigidbody2D rb2d;
    private bool guyCanShoot;
    public ManageDamage healthScript;
    public float damage = 5;

    void OnCollisionEnter2D()
    {
        guyCanShoot = true;
    }

    void OnCollisionExit2D()
    {
        guyCanShoot = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (guyCanShoot == false)
        {
            reloadTimer = 0;
            return;
        }
        if (awareOfPlayer == false)
        {
            reloadTimer = 0;
            return;
        }

        if (!playerInSight)
        {
            return;
        }

        if (reloadTimer < reloadTime)
        {
            reloadTimer += Time.deltaTime;
            return;
        }

        if (timeSinceLastShot < timeBetweenShots)
        {
            timeSinceLastShot += Time.deltaTime;
            return;
        }

        timeSinceLastShot = 0;

        if (currentShot > amountOfShots)
        {
            reloadTimer = 0;
            currentShot = 1;
            return;
        }

        currentShot += 1;
        audioSource.PlayOneShot(gunshot, 1f);
        healthScript.ApplyDamage(damage);
    }
}
