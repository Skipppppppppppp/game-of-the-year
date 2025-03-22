using UnityEngine;
using UnityEngine.Analytics;

public class StretchToMouse : MonoBehaviour
{
        private Transform transform;
        private Transform pwayerTransform;
        private Transform itemTransform;
        public MovingObjects movingObjectsScript;
        private SpriteRenderer spriteRenderer;

    void ScaleTo(Transform trans, Vector2 originalPosition, Vector2 targetPosition)
    {
        float distanceToObject = (originalPosition-targetPosition).magnitude;
        trans.position = new Vector2 ((originalPosition.x + targetPosition.x)/2, (originalPosition.y + targetPosition.y)/2);
        trans.localScale = new Vector2 (trans.localScale.x, distanceToObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform = this.gameObject.transform;
        Rigidbody2D pwayerrb2d = GetComponentInParent<Rigidbody2D>();
        pwayerTransform = pwayerrb2d.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 pwayerPosition = pwayerTransform.position;
        if (movingObjectsScript.movingObject == null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ScaleTo(transform, pwayerPosition, mousePosition);
            return;
        }

        itemTransform = movingObjectsScript.movingObject.transform;
        Vector2 itemPosition = itemTransform.position;
        ScaleTo(transform, pwayerPosition, itemPosition);
    }
}
