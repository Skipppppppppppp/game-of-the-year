using System;
using System.Data;
using UnityEngine;

public class RememberInitialProperties : MonoBehaviour
{
    public RememberedProperties Props;
    public float LinearDamping => Props.LinearDamping;
    public float GravityScale => Props.GravityScale;
}

[Serializable]
public struct RememberedProperties
{
    public float LinearDamping;
    public float GravityScale;
}

public static class RememberedPropertiesHelper
{
    public static RememberedProperties GetInitialProps(this Rigidbody2D rb)
    {
        var r = rb.gameObject.GetComponent<RememberInitialProperties>();
        if (r != null)
        {
            return r.Props;
        }
        return rb.GetPropertiesForRemember();
    }

    public static RememberedProperties GetPropertiesForRemember(this Rigidbody2D rb)
    {
        return new()
        {
            LinearDamping = rb.linearDamping,
            GravityScale = rb.gravityScale,
        };
    }
}
