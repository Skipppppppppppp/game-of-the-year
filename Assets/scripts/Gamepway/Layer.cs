using System;

public enum Layer
{
    Default = 0,
    Pwayer = 6,
    Peopwe = 7,
    MoveableStuff = 3,
    Portals = 8,
    Doors = 9,
}

[Flags]
public enum LayerMask
{
    None = 0,
    Default = 1 << Layer.Default,
    Pwayer = 1 << Layer.Pwayer,
    Peopwe = 1 << Layer.Peopwe,
    MoveableStuff = 1 << Layer.MoveableStuff,
    Portals = 1 << Layer.Portals,
    Doors = 1 << Layer.Doors,
}

public static class LayerHelper1
{
    public static bool Contains(this LayerMask mask, Layer layer)
    {
        int m = (int)mask;
        int l = (int)layer;
        return (m & (1 << l)) != 0;
    } 
}