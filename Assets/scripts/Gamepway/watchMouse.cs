using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class watchMouse : Rotation
{
    private Transform transform;

    // Start is called before the first frame update
    void Start()
    {   
        transform = this.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Rotate(mousePosition, transform);
    }
}
