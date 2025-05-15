using Godot;
using System;

[GlobalClass]
public partial class SlotData : Resource
{
    [Export] public ItemData itemData;
    [Export] public int quantity = 0;

    public void SetQuantity(int value)
    {
        quantity = value;
        if (quantity < 1)
            EmitChanged();
    }
}
