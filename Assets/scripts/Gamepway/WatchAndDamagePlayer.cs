using UnityEngine;

public class WatchAndDamagePlayer : watchplayer
{
    public float timeNeeded;
    private float timer = 0;
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
            timer = 0;
            return;
        }
        if (awareOfPlayer == false)
        {
            if (previousAwarenessState == true)
            {
                timer = 0;
            }
            return;
        }

        if (!playerInSight)
        {
            return;
        }

        if (timer < timeNeeded)
        {
            timer += Time.deltaTime;
            return;
        }
    
        timer = 0;
        audioSource.PlayOneShot(gunshot, 1f);
        healthScript.ApplyDamage(damage);
    }
}
