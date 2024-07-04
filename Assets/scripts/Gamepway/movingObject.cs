using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingObject : MonoBehaviour
{
    private bool isObjectMoving;
    private Rigidbody2D rb2d;
    public GameObject player;
    public GameObject moveableObject;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
        float distance = Vector2.Distance(player.transform.position, moveableObject.transform.position);
        float distanceToMouse = Vector2.Distance(player.transform.position, mousePosition2D);
        if (Input.GetMouseButtonUp(1) || distance >= 12)
        {
            isObjectMoving = false;
        }
        if (Input.GetMouseButtonDown(1) && distance <= 8)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);
            if (hit.collider != null && hit.collider.transform == transform)
            {
                isObjectMoving = true;
            }
        }
        if (isObjectMoving)
        {
            Vector2 direction = (mousePosition - transform.position).normalized;
            rb2d.velocity = (direction*1);
            if (distance <= 10)
            {
                rb2d.velocity = (direction*40);
            }
            if (distanceToMouse >=10 && distance <=11)
            {
                rb2d.velocity = (direction*10);
            }
        }
    }
}
