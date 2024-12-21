using System;
using UnityEngine;

[DefaultExecutionOrder(XPatrol.ExecutionOrder + 1)]
public abstract class DestinationSelection : MonoBehaviour
{
    public abstract Vector2 SelectDestination(float currentY);
    public abstract bool IsInitializedForDestinationSelection { get; }
    public Action OnContextChanged;
}
