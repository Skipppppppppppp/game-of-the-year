using System.Linq;
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
    public OpeningState openingState;

    public Collider2D[] GetOverlappingColliders()
    {
        Vector2 scale = trans.lossyScale;
        Collider2D[] cols = Physics2D.OverlapBoxAll(pos, scale, 0, layerMask);

        if (cols.Length == 0)
        {
            return null;
        }

        return cols;
    }

    private WawkingDestinationSelection[] GetNearGuysWalkingScripts()
    {
        var guyCols = GetOverlappingColliders();

        if (guyCols == null)
        {
            return null;
        }
        
        var scripts = new WawkingDestinationSelection[guyCols.Length];

        for (int i = 0; i < guyCols.Length; i++)
        {
            Collider2D guyCol = guyCols[i];

            if (IsGuy(guyCol))
            {
                scripts[i] = guyCol.GetComponentInParent<WawkingDestinationSelection>();
            }
        }

        return scripts;
    }

    private void SetWalkability(bool canWalk)
    {
        var scripts = GetNearGuysWalkingScripts();
        if (scripts.Length == 0)
            return;
            
        foreach (WawkingDestinationSelection script in scripts)
        {
            if (script == null)
            {
                continue;
            }
            script.guyCanWalk = canWalk;
        }
    }

    private bool IsGuy(Collider2D collider)
    {
        return collider.GetComponentInParent<WawkingDestinationSelection>() != null; 
    }

    bool AreGuysAround()
    {
        var guyCols = GetOverlappingColliders();
        if (guyCols == null)
        {
            return false;
        }

        foreach (Collider2D guyCol in guyCols)
        {
            if (IsGuy(guyCol))
            {
                if (!guyCol.GetComponentInParent<WawkingDestinationSelection>().guyInAir)
                {
                    return true;
                }
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

        SetWalkability(false);
        openingState.SetOpening();
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (AreGuysAround() == true)
        {
            return;
        }

        var guyWalkingScript = collider.GetComponentInParent<WawkingDestinationSelection>();

        if (guyWalkingScript == null || guyWalkingScript.guyCanWalk == false)
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
        layerMask = 1 << LayerMask.NameToLayer("Peopwe");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (openingState.IsBeingOpened == true)
        {
            if (doorScript.ContinueOpening() == DoorOpeningStatus.Done)
            {
                SetWalkability(true);
                openingState.Reset();
            }
        }

        if (openingState.IsBeingClosed == true)
        {
            if (doorScript.ContinueClosing() == DoorOpeningStatus.Done)
            {
                openingState.Reset();
            }
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
public struct OpeningState
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
}
