using Godot;
using System;

public class PlayerFallState : EntityState
{
	private Player Player => (Player)Entity;
	
	public PlayerFallState(Entity entity) : base(entity) {}
	
	public override void PhysicsUpdate(double delta)
	{
		Player.UpdateWalkVelocity();

		float fallSpeed = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
		
		Player.Velocity += Vector2.Down * (float)(fallSpeed * delta);
	}
}
