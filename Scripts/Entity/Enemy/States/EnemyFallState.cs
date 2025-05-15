using Godot;
using System;

public class EnemyFallState : EntityState
{
	private Enemy Enemy => (Enemy)Entity;
	
	public EnemyFallState(Entity entity) : base(entity) {}
	
	public override void PhysicsUpdate(double delta)
	{
		Enemy.UpdateWalkVelocity();

		float fallSpeed = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
		
		Enemy.Velocity += Vector2.Down * (float)(fallSpeed * delta);
	}
}
