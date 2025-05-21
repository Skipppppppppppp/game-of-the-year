using UnityEngine;

public class Portal : MonoBehaviour, IObjectInteractionsHandler
{
    public Transform otherEndTrans;
    private Vector2 otherEndPos;

    void Start()
    {
        otherEndPos = otherEndTrans.position;
    }

    public void PlayerNearObject()
    {

    }

    public void Interact(Transform pwayerTrans)
    {
        float currentZ = pwayerTrans.position.z;

        Vector3 pos = new Vector3(otherEndPos.x, otherEndPos.y, currentZ);
        pwayerTrans.position = pos;
    }
}