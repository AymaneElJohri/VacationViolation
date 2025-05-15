using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class InventoryData : Resource
{
    [Export] public Array<SlotData> slots;

    public void _Init()
    {
        ConnectSlots();
    }

    //Called when item is picked up, also needs to be called to add items via code so the remove after coun < 1 connects correctly
    public bool AddItem(ItemData item, int count = 1)
    {
        foreach (var s in slots)
        {
            if (s != null && s.itemData == item)
            {
                s.quantity += count;
                return true;
            }
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null)
            {
                var newSlotdata = new SlotData();
                newSlotdata.itemData = item;
                newSlotdata.quantity = count;
                slots[i] = newSlotdata;
                newSlotdata.Changed += slotChanged;
                return true;
            }
        }

        GD.Print("Inventory full");
        return false;
    }

    public void ConnectSlots()
    {
        foreach (var s in slots)
        {
            if (s != null)
                s.Changed += slotChanged;
        }
    }

    private void slotChanged()
    {
        foreach (var s in slots)
        {
            if (s != null && s.quantity < 1)
            {
                s.Changed -= slotChanged;
                var index = slots.IndexOf(s);
                slots[index] = null;
                EmitChanged();
            }
        }
    }

    //Get saved inventory
    public List<System.Collections.Generic.Dictionary<string, object>> GetSaveData()
    {
        var itemSave = new List<System.Collections.Generic.Dictionary<string, object>>();
        for (int i = 0; i < slots.Count; i++)
        {
            itemSave.Add(ItemToSave(slots[i]));
        }
        return itemSave;
    }



    //Save Inventory for later
    public System.Collections.Generic.Dictionary<string, object> ItemToSave(SlotData slot)
    {
        var result = new System.Collections.Generic.Dictionary<string, object>
        { 
            { "item", ""},
            { "quantity", 0}
        };

        if (slot != null)
        {
            result["quantity"] = slot.quantity;
            if (slot.itemData != null)
            {
                result["item"] = slot.itemData.ResourcePath;
            }
        }

        return result;
    }

    public void ParseSaveData(List<System.Collections.Generic.Dictionary<string, object>> saveData)
    {
        var newSlots = new Array<SlotData>();

        for (int i = 0; i < saveData.Count; i++)
        {
            slots.Add(ItemFromSave(saveData[i]));
        }

        slots = newSlots;

        ConnectSlots();
    }

    // Reconstruct a slot from save data
    private SlotData ItemFromSave(System.Collections.Generic.Dictionary<string, object> saveObject)
    {
        if (saveObject["item"].ToString() == "")
        {
            return null;
        }

        SlotData newSlot = new SlotData();
        newSlot.itemData = GD.Load<ItemData>(saveObject["item"].ToString());
        newSlot.quantity = Convert.ToInt32(saveObject["quantity"]);

        return newSlot;
    }



    // To save the data just call player.InventoryData.GetSaveData();
    // To load the data just call player.InventoryData.ParseSaveData(saveData)
}
