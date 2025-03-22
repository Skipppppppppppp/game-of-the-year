using UnityEngine;

public static class CenterOfMassFinder
{
    public static Vector2 FindObjectPosition(Rigidbody2D rb2d)
    {
        Transform trans = rb2d.transform;
        var localPositionWithScale = rb2d.centerOfMass;
        var localPositionWithoutScale = localPositionWithScale;
        var scale = trans.localScale;
        localPositionWithoutScale.x /= scale.x;
        localPositionWithoutScale.y /= scale.y;
        var ret = trans.localToWorldMatrix.MultiplyPoint3x4(localPositionWithoutScale);
        return ret;
    }
}
