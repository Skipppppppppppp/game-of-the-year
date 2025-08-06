using System;
using System.Threading;
using UnityEngine;

public class grenader : MonoBehaviour
{
    public TimerUtility timer;
    public GameObject grenade;
    [Range(0, 1f)] public float timeScale;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
        if (!timer.Update())
        {
            return;
        }

        Vector2 pos = transform.position;
        GameObject thrownGrenade = Instantiate(grenade, position: pos, rotation: Quaternion.identity);
        var e = thrownGrenade.GetComponent<explodeAfterTime>();
        e.timer.Timeout = 0;
        e.damage = 0;
    }
}
