using Godot;
using System;

public class PlayerCrouchWalkState : EntityState
{
	private Player Player => (Player)Entity;
	
	public PlayerCrouchWalkState(Entity entity) : base(entity) {}
	
	public override void Enter()
	{
		Player.SetSpeed(Player.CrouchSpeed);
	}
	
	public override void PhysicsUpdate(double delta)
	{
		Player.UpdateWalkVelocity();
	}
}