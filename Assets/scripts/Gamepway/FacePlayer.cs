using UnityEngine;
using UnityEngine.Analytics;

public class FacePlayer : Rotation
{
    public Transform pwayerTrans;
    private Transform trans;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (pwayerTrans != null)
        {
            Vector3 pwayerPosition = pwayerTrans.position;
            Rotate(pwayerPosition, trans);
        }
    }
}
