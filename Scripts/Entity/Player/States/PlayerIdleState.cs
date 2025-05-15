using Godot;
using System;

public class PlayerIdleState : EntityState
{
	private Player Player => (Player)Entity;
	
	public PlayerIdleState(Entity entity) : base(entity) {}

	public override void Enter()
	{
		Player.Velocity = new Vector2(0, 0);
	}
	
	public override void PhysicsUpdate(double delta)
	{
		
	}
}
