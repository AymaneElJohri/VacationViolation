using Godot;
using System;

[GlobalClass]
public partial class ItemEffect : Resource
{
	[Export] string UseDescription;

	public virtual void Use()
	{

	}
}
