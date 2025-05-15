using Godot;
using System;

[GlobalClass]
public partial class Heal : ItemEffect
{
	[Export] int HealAmount = 1;
	[Export] AudioStream Audio;

	public override void Use()
	{
		DataManager.Instance.player.HealByAmount(HealAmount);
		
		if (Audio != null)
		{
			DataManager.Instance.PlaySFXAudio(Audio);
		}
	}
}
