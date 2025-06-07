using UnityEngine;
using UnityEngine.Analytics;

public class StretchToMouse : MonoBehaviour
{
    private Transform trans;
    public Transform handTrans;
    public Transform pwayerTrans;
    public MovingObjects movingObjectsScript;
    private SpriteRenderer spriteRenderer;
    private Vector2 initialSize;

    void ScaleTo(Transform trans, Vector2 originalPosition, Vector2 targetPosition)
    {
        float distanceToObject = (originalPosition - targetPosition).magnitude;
        trans.position = new Vector2((originalPosition.x + targetPosition.x) / 2, (originalPosition.y + targetPosition.y) / 2);
        trans.localScale = new Vector2(trans.localScale.x, 1);
        spriteRenderer.size = new Vector2(initialSize.x, distanceToObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialSize = spriteRenderer.size;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pwayerPosition = pwayerTrans.position;
        Vector2 otherObjectPos = handTrans.position;

        ScaleTo(trans, pwayerPosition, otherObjectPos);
    }
}
