using UnityEngine;

public class NonPlayerManageDamage : MonoBehaviour, IDamageHandler
{
    public float initialHP = 3;
    public float hp = 3;
    private GameObject guy;
    public AudioClip[] sounds;
    public AudioClip[] deathSounds;

    public void TakeDamage(float amount)
    {
        hp = hp - amount;
        UpdateHP(hp);
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

        Destroy(guy);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform trans = GetComponent<Transform>();
        guy = trans.gameObject;
    }
}
