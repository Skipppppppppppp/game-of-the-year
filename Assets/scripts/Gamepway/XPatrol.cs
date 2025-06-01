using System;
using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(ExecutionOrder)]
public class XPatrol : MonoBehaviour
{
    public const int ExecutionOrder = 200;
    private DestinationSelection selectDestination;
    private Vector2 destination;
    private Rigidbody2D rb2d;
    [Range(0.01f, 100)] public float speed;

    void SelectDestination()
    {
        destination = selectDestination.SelectDestination(transform.position.y);
        var imageTransform = rb2d.transform.GetChild(0);
        if (destination.x <= transform.position.x && imageTransform.localScale.x > 0)
        {
            imageTransform.localScale= new Vector2 (-imageTransform.localScale.x, imageTransform.localScale.y);
        }
        else
        if (destination.x > transform.position.x && imageTransform.localScale.x < 0)
        {
            imageTransform.localScale= new Vector2 (-imageTransform.localScale.x, imageTransform.localScale.y);
        }
    }
    
    void Start()
    {
        selectDestination = GetComponent<DestinationSelection>();
        selectDestination.OnContextChanged = SelectDestination;
        rb2d = GetComponent<Rigidbody2D>();
        StartCoroutine(UpdateCoroutine());
    }

    IEnumerator UpdateCoroutine()
    {
        while (true)
        {
            if (!selectDestination.IsInitializedForDestinationSelection)
            {
                yield return null;
                continue;
            }
            if (Mathf.Abs(destination.x - transform.position.x) <= 0.1f)
            {
                SelectDestination();
                float waitingTime = UnityEngine.Random.Range(3f,5f);
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
}
