using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class InteractWithBG : MonoBehaviour
{
    private Transform trans;
    public TextMeshProUGUI textToChange;
    public string interactionAnnouncement = "Press [F] to interact";
    private bool isPlayer;
    private float timeSinceInteraction = 0;
    public float interactionCooldown = 3;
    public DarkenScreen darkeningScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        isPlayer = GetComponent<scwipt>() != null;
    }

    // Update is called once per frame
    async void Update()
    {
        timeSinceInteraction += Time.deltaTime;
        if (timeSinceInteraction < interactionCooldown)
        {
            return;
        }

        Vector2 pos = trans.position;
        IObjectInteractionsHandler interactionInterface = null;

        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.5f);

        if (cols.Length == 0)
        {
            if (isPlayer)
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
            if (textToChange.text == interactionAnnouncement && isPlayer)
            {
                textToChange.text = "";
            }
            return;
        }

        interactionInterface.PlayerNearObject();

        if (isPlayer == false)
        {
            int chance = Random.Range(0, 2);
            if (chance == 0)
            {
                interactionInterface.Interact(trans);
                timeSinceInteraction = 0;
            }
            return;
        }

        textToChange.text = interactionAnnouncement;
        if (interactionInterface.ShouldInteract(trans))
        {
            timeSinceInteraction = 0;
            await darkeningScript.StartChangingOpacity(1f, 1f);
            interactionInterface.Interact(trans);
            await darkeningScript.StartChangingOpacity(0f, 1f);
        }

    }
}

public interface IObjectInteractionsHandler
{
    void PlayerNearObject();
    void Interact(Transform pwayerTrans);
    bool ShouldInteract(Transform pwayerTrans);
}