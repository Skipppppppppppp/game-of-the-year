using UnityEngine;

public class watchplayer : MonoBehaviour
{

    public bool awareOfPlayer = false;
    private bool previousAwarenessState = false;
    private WawkingDestinationSelection destinationSelectionScript;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destinationSelectionScript = this.gameObject.GetComponent<WawkingDestinationSelection>();
    }

    // Update is called once per frame
    void Update()
    {
        if (awareOfPlayer == false && previousAwarenessState == true)
        {
            destinationSelectionScript.guyCanWalk = true;
        }

        if (awareOfPlayer == true && destinationSelectionScript.guyCanWalk == true)
        {
            destinationSelectionScript.guyCanWalk = false;
            previousAwarenessState = awareOfPlayer; {}
        }

        

    }
}
