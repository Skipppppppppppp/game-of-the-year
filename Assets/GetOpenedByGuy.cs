using UnityEngine;
using UnityEngine.UIElements;

public class GetOpenedByGuy : MonoBehaviour
{
    private Transform trans => transform;
    private Vector2 pos;
    private bool canBeClosed;
    public GameObject doorObj;
    private OpenAndClose doorScript;
    private int layerMask;
    private OpeningState openingState;

    private bool IsGuy(Collider2D collider)
    {
        return collider.GetComponent<XPatrol>() != null; 
    }

    bool AreGuysAround()
    {
        Vector2 scale = trans.localScale;
        Collider2D[] guyCols = Physics2D.OverlapBoxAll(pos, scale, 0, layerMask);

        if (guyCols.Length == 0)
        {
            return false;
        }

        foreach (Collider2D guyCol in guyCols)
        {
            if (IsGuy(guyCol))
            {
                return true;
            }
        }
        return false;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider is not BoxCollider2D other)
        {
            return;
        }

        if (IsGuy(collider) == false)
        {
            return;
        }

        openingState.SetOpening();
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (AreGuysAround() == true)
        {
            return;
        }
        openingState.SetClosing();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pos = trans.position;
        doorScript = doorObj.GetComponent<OpenAndClose>();
        layerMask = 1 >> LayerMask.NameToLayer("Peopwe");
    }

    // Update is called once per frame
    void Update()
    {
        if (openingState.IsBeingOpened == true)
        {
            if (doorScript.ContinueOpening() == DoorOpeningStatus.Done)
            {
                openingState.Reset();
            }
            doorScript.ContinueOpening();
        }

        if (openingState.IsBeingClosed == true)
        {
            if (doorScript.ContinueClosing() == DoorOpeningStatus.Done)
            {
                openingState.Reset();
            }
                doorScript.ContinueClosing();
    }
}

    // void Update()
    // {
    //     const int maxActions = 2;
    //     for (int action = 0; action < maxActions; action++)
    //     {
    //         if (!GetOpeningStatePart(action))
    //         {
    //             continue;
    //         }
    //         if (CloseOrOpen(action) == DoorOpeningStatus.Done)
    //         {
    //             openingState.Reset();
    //         }
    //         break;
    //     }
    // }

    // private bool GetOpeningStatePart(int part)
    // {
    //     switch (part)
    //     {
    //         case 0:
    //             return openingState.IsBeingOpened;
    //         case 1:
    //             return openingState.IsBeingClosed;
    //         default:
    //             throw null!;
    //     }
    // }

    // private DoorOpeningStatus CloseOrOpen(int action)
    // {
    //     switch (action)
    //     {
    //         case 0:
    //             return doorScript.ContinueOpening();
    //         case 1:
    //             return doorScript.ContinueClosing();
    //         default:
    //             throw null!;
    //     }
    // }

struct OpeningState
{
    public bool IsBeingOpened { get; private set; }
    public bool IsBeingClosed { get; private set; }

    public void SetOpening()
    {
        IsBeingOpened = true;
        IsBeingClosed = false;
    }

    public void SetClosing()
    {
        IsBeingOpened = false;
        IsBeingClosed = true;
    }

    public void Reset()
    {
        this = default;
    }
}