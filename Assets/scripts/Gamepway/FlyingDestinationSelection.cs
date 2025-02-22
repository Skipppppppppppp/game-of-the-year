using UnityEngine;

public class FlyingDestinationSelection : DestinationSelection
{
    public override bool IsInitializedForDestinationSelection => false;
    public override Vector2 SelectDestination(float currentY)
    {
        return default;
    }
}
