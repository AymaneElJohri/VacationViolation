using Godot;
using System;

public class EnemyWalkState : EntityState
{
	private Enemy Enemy => (Enemy)Entity;
	
	public EnemyWalkState(Entity entity) : base(entity) {}
	
	public override void Enter()
	{
		Enemy.SetSpeed(Enemy.WalkSpeed);
	}
	
	public override void PhysicsUpdate(double delta)
	{
		Enemy.UpdateWalkVelocity();
	}
}
