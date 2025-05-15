using Godot;
using System;

public class EnemyIdleState : EntityState
{
	private Enemy Enemy => (Enemy)Entity;
	
	public EnemyIdleState(Entity entity) : base(entity) {}
	
	public override void PhysicsUpdate(double delta)
	{
		Enemy.Velocity = new Vector2(0, 0);
	}
}
