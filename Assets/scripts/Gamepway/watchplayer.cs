using System;
using UnityEngine;

public class watchplayer : MonoBehaviour
{

    public bool awareOfPlayer = false;
    public bool playerInSight = false;
    protected bool previousAwarenessState = false;
    public WawkingDestinationSelection destinationSelectionScript;

    public static event Action DealDamage;

    public void InvokeDamageEvent()
    {
        DealDamage?.Invoke();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destinationSelectionScript = this.gameObject.GetComponent<WawkingDestinationSelection>();
    }

    // Update is called once per frame
    void Update()
    {
        if (previousAwarenessState == true)
        {
            if (awareOfPlayer == false)
            {
                destinationSelectionScript.guyCanWalk = true;
            }
            previousAwarenessState = awareOfPlayer;
            return;
        }

        previousAwarenessState = awareOfPlayer;
    }
}
