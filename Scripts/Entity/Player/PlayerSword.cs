using Godot;
using System;

public partial class PlayerSword : Area2D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered; // Connect the signal
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Enemies")) // Check if the collided object is an enemy
        {
			var enemy = (body as Enemy);

			Player player = GetNode<Player>("../../../Player");
			Vector2 knockbackDirection = body.GlobalPosition - player.GlobalPosition;

			float knockbackForce = 300;
            // Call a method on the enemy to reduce health
            enemy.TakeDamage(10, knockbackDirection, knockbackForce);
        }
    }
}
