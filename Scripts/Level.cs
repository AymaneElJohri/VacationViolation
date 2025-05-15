using Godot;
using System;

public partial class Level : Node
{
	public Player player;
	public Player player2;

	public override void _Ready()
	{
		//Setting the player to be used by resources and others that need it
		player = GetNode<Player>("Player");
		DataManager.Instance.SetPlayer(player);

		if (DataManager.Instance.multiplayer)
		{
			player2 = GetNode<Player>("Player2");
			DataManager.Instance.SetPlayer2(player2);
		}

		// Resetting the coin count
		DataManager.Instance.SetCoins(0);

		// Setting current level
		DataManager.Instance.SetCurrentLevel(this);
	}

	public override void _Process(double delta)
	{
	}
}
