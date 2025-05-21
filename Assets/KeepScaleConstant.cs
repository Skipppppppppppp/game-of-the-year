using UnityEngine;

public class MaintainWorldScale : MonoBehaviour
{
    private Vector2 initialWorldScale;

    void Start()
    {
        initialWorldScale = transform.lossyScale;
    }

    void Update()
    {
        if (transform.parent == null) return;

        Vector2 parentScale = transform.parent.lossyScale;
        Vector2 newLocalScale = new Vector2(
            initialWorldScale.x / parentScale.x,
            initialWorldScale.y / parentScale.y
        );
        transform.localScale = newLocalScale;
    }
}