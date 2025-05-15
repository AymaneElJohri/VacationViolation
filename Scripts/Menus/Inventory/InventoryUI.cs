using Godot;
using System;

public partial class InventoryUI : Control
{
    public PackedScene InventorySlot = GD.Load<PackedScene>("res://Scenes/Menus/Inventory/InventorySlot.tscn");
    private int FocusIndex = 0;

    [Export] public InventoryData Data;

    public override void _Ready()
    {
        // Connect signals
        Inventory.shown += InventoryOpened;
        Inventory.hidden += InventoryClosed;
        
        ClearInventory();
        Data.Changed += OnInventoryChanged;
    }

	private void InventoryOpened()
	{
        ClearInventory();
		UpdateInventory();
	}

	private void InventoryClosed()
	{

	}

    private void ClearInventory()
    {
        //Remove all child nodes
        foreach (Node child in GetChildren())
        {
            child.QueueFree();
        }
    }

    private void UpdateInventory()
    {
        foreach (var slotData in Data.slots)
        {
            InventorySlotUI newSlot = InventorySlot.Instantiate<InventorySlotUI>();
            this.AddChild(newSlot);
			newSlot.SlotData = slotData;
            newSlot.FocusEntered += ItemFocused;
        }
    }

    private void ItemFocused()
    {
        for (int i = 0; i < GetChildCount(); i++)
        {
            if (GetChild<Button>(i).HasFocus())
            {
                FocusIndex = i;
                return;
            }
        }
    }

    private void OnInventoryChanged()
    {
        ClearInventory();
        UpdateInventory();
    }
}