using System;
using UnityEngine;

public class hp_bar : MonoBehaviour
{
    [Range(0,100)] public float hp = 100;
    private RectTransform bartransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bartransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        bartransform.localScale = new Vector2(1, hp / 100);
    }
}
