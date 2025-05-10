using UnityEngine;

public class GuyManageDamage : MonoBehaviour
{
    public float initialHP = 3;
    public float hp = 3;
    private GameObject guy;

    public void TakeDamage(float amount)
    {
        hp = hp - amount;
        UpdateHP(hp, guy);
    }

    // should add changing images, speed and MAYBE IN THE FUTURE behaviour when hp changes (basically phases)
    // and also make it so bro screams when gets hit / dies (loser)
    // will do when I have images and sounds and also when I'm not lazy
     
    static void UpdateHP(float currentHP, GameObject thisGuy)
    {
        if (currentHP > 0)
        {
            return;
        }
        
        Destroy(thisGuy);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform trans = GetComponent<Transform>();
        guy = trans.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
