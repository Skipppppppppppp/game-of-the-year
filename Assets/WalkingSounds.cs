using UnityEngine;

public class WalkingSounds : MonoBehaviour
{
    private AudioSource audioSource;
    private Transform trans;
    private Rigidbody2D rb2d;
    private float xVelocity;
    private Vector2 pos;
    public float velocityToPlaySound = 0.5f;
    private bool onGround = false;
    private int layerMask;
    private float timer;
    public float timeBetweenSounds = 0.5f;
    public Surface[] surfaces;
    private int prevPlayedSoundIndex = 0;

[System.Serializable]
    public struct Surface
    {
        public string name;
        public AudioClip[] sounds;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!onGround)
        {
            onGround = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        onGround = false;
        timer = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb2d = GetComponent<Rigidbody2D>();
        trans = this.transform;
        layerMask |= 1 << LayerMask.NameToLayer("Default");
    }

    // Update is called once per frame
    void Update()
    {
        if (onGround == false)
        {
            return;
        }
        
        xVelocity = rb2d.linearVelocityX;

        if (Mathf.Abs(xVelocity) < velocityToPlaySound)
        {
            return;
        }

        pos = trans.position;
        RaycastHit2D rayHit = Physics2D.Raycast(pos, new Vector2 (0, -1), float.PositiveInfinity, layerMask);

        if (rayHit.transform == null)
        {
            return;
        }

        GameObject surface = rayHit.transform.gameObject;
        string surfaceType = surface.tag;

        if (timer < timeBetweenSounds)
        {
            timer += Time.deltaTime;
            return;
        }

        foreach (Surface s in surfaces)
        {
            if (s.name != surfaceType)
            {
                continue;
            }

            int soundIndex = RandomStuffHelper.PickUniqueNumber(0, s.sounds.Length, prevPlayedSoundIndex);

            AudioClip soundToPlay = s.sounds[soundIndex];
            
            audioSource.PlayOneShot(soundToPlay);
            prevPlayedSoundIndex = soundIndex;
            timer = 0;
        }
    }
}
