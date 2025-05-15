using Godot;
using System;

public class PlayerCrouchState : EntityState
{
	private Player Player => (Player)Entity;
	
	public PlayerCrouchState(Entity entity) : base(entity) {}
	
	public override void Enter()
	{
		Player.SetSpeed(Player.CrouchSpeed);
	}
	
	public override void PhysicsUpdate(double delta)
	{
		Player.UpdateWalkVelocity();
	}
}
