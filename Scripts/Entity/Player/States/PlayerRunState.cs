using Godot;
using System;

public class PlayerRunState : EntityState
{
	private Player Player => (Player)Entity;
	
	public PlayerRunState(Entity entity) : base(entity) {}
	
	public override void Enter()
	{
		Player.SetSpeed(Player.RunSpeed);
	}
	
	public override void PhysicsUpdate(double delta)
	{
		Player.UpdateWalkVelocity();
	}
}
