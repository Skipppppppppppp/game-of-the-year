using System.Collections;
using System.Net.Http.Headers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class wawking : MonoBehaviour
{
    private Vector2 destination;
    private Rigidbody2D rb2d;
    private BoxCollider2D guyCollider;
    private bool guyFoundCollider;
    private float waxX;
    private float winX;
    [Range(0.01f,10)] public float speed;

    void SelectDestination(float minX, float maxX, float currentY)
    {
        float newMinX = minX;
        float newMaxX = maxX;
        float newX = Random.Range(newMinX, newMaxX);
        Vector2 newDestination = new Vector2(newX, currentY);
        destination = newDestination;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Vector2 (winX,rb2d.transform.position.y),Vector2.up);
        Gizmos.DrawRay(new Vector2 (waxX,rb2d.transform.position.y),Vector2.up);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;

        // cast
        // BoxCollider2D boxCollider = (BoxCollider2D) collider;

        // // as
        // BoxCollider2D? boxCollider = collider as BoxCollider2D;
        // if (boxCollider != null)
        // {
        //     // 
        // }

        if (collider is not BoxCollider2D boxCollider)
        {
            Debug.Log("guy trying to walk on wrong collider");
            return;
        }

        var winX = boxCollider.bounds.min.x;
        var waxX = boxCollider.bounds.max.x;
        var winY = boxCollider.bounds.min.y;
        var worldPositionTopLeft = new Vector2(winX, winY);
        var worldPositionTopRight = new Vector2(waxX, winY);
        this.winX = worldPositionTopLeft.x + guyCollider.bounds.extents.x;
        this.waxX = worldPositionTopRight.x - guyCollider.bounds.extents.x;
        guyFoundCollider = true;
        destination = transform.position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destination = transform.position;
        guyFoundCollider = false;
        rb2d = GetComponent<Rigidbody2D>();
        guyCollider = GetComponentInChildren<BoxCollider2D>();
        StartCoroutine(UpdateCoroutine());
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (guyFoundCollider == false)
            {
                yield return null;
                continue;
            }
            if (Mathf.Abs(destination.x - transform.position.x) <= 0.1f)
            {
                SelectDestination(winX, waxX, transform.position.y);
                float waitingTime = Random.Range(3f,5f);
                yield return new WaitForSeconds(waitingTime);
                continue;
            }
            if (destination.x < transform.position.x)
            {
                rb2d.AddForce(Vector2.left * speed);
            }
            else
            {
                rb2d.AddForce(Vector2.right * speed);
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
