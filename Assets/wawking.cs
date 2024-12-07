using System.Collections;
using System.Net.Http.Headers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class wawking : MonoBehaviour
{
    public BoxCollider2D platformCollider;
    private Vector2 destination;
    private Rigidbody2D rb2d;
    private float waxX;
    private float winX;
    [Range(0.01f,10)] public float speed;

    void SelectDestination(float minX, float maxX, float currentY)
    {
        float newX = Random.Range(minX, maxX);
        Vector2 newDestination = new Vector2(newX, currentY);
        destination = newDestination;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        destination = transform.position;
        var winX = platformCollider.bounds.min.x;
        var waxX = platformCollider.bounds.max.x;
        var winY = platformCollider.bounds.min.y;
        var worldPositionTopLeft = new Vector2(winX, winY);
        var worldPositionTopRight = new Vector2(waxX, winY);
        this.winX = worldPositionTopLeft.x;
        this.waxX = worldPositionTopRight.x;
        StartCoroutine(UpdateCoroutine());
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (Mathf.Abs(destination.x - transform.position.x) <= 0.1f)
            {
                SelectDestination(winX, waxX, transform.position.y);
                float waitingTime = Random.Range(1f,3f);
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
