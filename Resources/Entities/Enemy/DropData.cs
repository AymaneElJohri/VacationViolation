using Godot;
using System;

[GlobalClass]
public partial class DropData : Resource
{
    [Export] public ItemData item;

    [Export(PropertyHint.Range, "0,100,1,suffix:%")] public float Probability { get; set; } = 100f;

    [Export(PropertyHint.Range, "1,10,1,suffix:items")] public int MinAmount { get; set; } = 1;

    [Export(PropertyHint.Range, "1,10,1,suffix:items")] public int MaxAmount { get; set; } = 1;

    public int GetDropCount()
    {
        if (GD.RandRange(0f, 100f) > Probability)
        {
            return 0;
        }

        return GD.RandRange(MinAmount, MaxAmount);
    }
}