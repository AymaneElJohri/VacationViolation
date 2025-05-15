using Godot;
using System;

public class PlayerWalkState : EntityState
{
	private Player Player => (Player)Entity;
	
	public PlayerWalkState(Entity entity) : base(entity) {}
	
	public override void Enter()
	{
		Player.SetSpeed(Player.WalkSpeed);
	}
	
	public override void PhysicsUpdate(double delta)
	{
		Player.UpdateWalkVelocity();
	}
}
