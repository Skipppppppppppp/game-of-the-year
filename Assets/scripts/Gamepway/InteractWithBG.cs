using TMPro;
using UnityEngine;

public class InteractWithBG : MonoBehaviour
{
    private Transform trans;
    public TextMeshProUGUI textToChange;
    public string interactionAnnouncement = "Press [F] to interact";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = trans.position;
        IObjectInteractionsHandler interactionInterface = null;

        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.5f);

        if (cols.Length == 0)
        {
            textToChange.text = "";
            return;
        }

        foreach (Collider2D col in cols)
        {
            interactionInterface = col.GetComponentInParent<IObjectInteractionsHandler>();
            if (interactionInterface is not null)
            {
                break;
            }
        }

        if (interactionInterface is null)
        {
            if (textToChange.text == interactionAnnouncement)
            {
                textToChange.text = "";
            }
            return;
        }

        interactionInterface.PlayerNearObject();

        if (Input.GetKeyDown(KeyCode.F))
        {
            interactionInterface.Interact(trans);
        }

        textToChange.text = interactionAnnouncement;
    }
}

public interface IObjectInteractionsHandler
{
    void PlayerNearObject();
    void Interact(Transform pwayerTrans);
}