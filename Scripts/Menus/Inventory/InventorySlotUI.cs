using Godot;
using System;

public partial class InventorySlotUI : Button
{
	private SlotData _slotData;
	public SlotData SlotData 
	{ 
		get => _slotData; 
		set => SetSlotData(value); 
	}

	TextureRect textureRect;
	Label label;
	Inventory inventory;

	private SlotData slotToUse;

    public override void _Ready()
    {
        textureRect = GetNode<TextureRect>("TextureRect");
		label = GetNode<Label>("Label");
		inventory = GetNode<Inventory>("../../..");

		textureRect.Texture = null;
		label.Text = "";

		FocusEntered += ItemFocused;
		FocusExited += ItemUnfocused;
    }

	public void SetSlotData(SlotData value)
	{
		_slotData = value;
		if (_slotData == null)
			return;
		textureRect.Texture = _slotData.itemData.Texture;
		label.Text = _slotData.quantity.ToString();
	}

	public void ItemFocused()
	{
		if (SlotData != null && SlotData.itemData != null)
		{
			inventory.UpdateItemDescription(SlotData.itemData.Description);
			if (SlotData.itemData.effects != null && SlotData.itemData.effects.Count > 0)
			{
				slotToUse = SlotData;
				inventory.useLabel.Show();
			}
			else
			{
				slotToUse = null;
				inventory.useLabel.Hide();
			}
		}
		else
		{
			slotToUse = null;
			inventory.useLabel.Hide();
			inventory.UpdateItemDescription("");
		}
	}

	public void ItemUnfocused()
	{
		inventory.UpdateItemDescription("");
		inventory.useLabel.Hide();
	}

	public void ItemPressed()
	{
		if (_slotData != null && _slotData.itemData != null)
		{
			bool wasUsed = SlotData.itemData.Use();
			if (wasUsed == false)
				return;
			SlotData.SetQuantity(SlotData.quantity - 1);
			label.Text = SlotData.quantity.ToString();
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("Use"))
		{
			// Only use the item if it's the currently focused slot
			// and it has an item with effects
			if (HasFocus() && slotToUse != null && slotToUse.itemData != null && slotToUse.itemData.effects != null)
				ItemPressed();
		}
	}
}