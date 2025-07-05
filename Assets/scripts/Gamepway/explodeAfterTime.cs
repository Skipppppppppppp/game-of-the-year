using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class explodeAfterTime : MonoBehaviour
{
    public float timer = 1;
    private Vector2 pos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            return;
        }

        pos = transform.position;
        Destroy(this.gameObject);
    }
}
