using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ItemData : Resource
{
    [Export] public string Name = "";
    [Export(PropertyHint.MultilineText)] public string Description = "";
    [Export] public int Worth;
    [Export] public bool NeedsToBeSold = false;
    [Export] public Texture2D Texture;
    [ExportCategory("Item Use Effects")]
    [Export] public Array<ItemEffect> effects;

    public bool Use()
    {
        if (effects.Count == 0)
            return false;
        
        foreach (var e in effects)
        {
            e.Use();
        }
        
        return true;
    }
}
