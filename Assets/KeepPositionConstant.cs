using UnityEngine;

public class KeepPositionConstant : MonoBehaviour
{
    private Transform trans;
    private Vector2 initialPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        initialPos = trans.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        trans.position = initialPos;
    }
}
