using Godot;
using System;

public partial class EnemyWeapon : Area2D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered; // Connect the signal
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Player")) // Check if the collided object is a player
        {
			var player = (body as Player);

            player.TakeDamage();
        }
    }
}
