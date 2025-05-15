using Godot;
using System;

public class PlayerJumpState : EntityState
{
	private Player Player => (Player)Entity;
	private bool AnimationFinishedPlaying;
	private bool AnimationFinishedConected = false;
	
	public PlayerJumpState(Entity entity) : base(entity) {}
	
	public override void Enter()
	{
		if (!AnimationFinishedConected)
		{
			Player.AnimatedSprite.AnimationFinished += AnimationFinished;
			AnimationFinishedConected = true;
		}
		
		Player.Velocity = new Vector2(Player.Velocity.X, Player.JumpVelocity);
		AnimationFinishedPlaying = false;
	}
	
	public override void PhysicsUpdate(double delta) 
	{
		Player.UpdateWalkVelocity();
		
		Player.Velocity += Vector2.Down * (float)(ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle() * delta);
	
		if (AnimationFinishedPlaying)
		{
			Player.AnimatedSprite.Pause();
			Player.AnimatedSprite.Frame = 3;
		}
	}
	
	private void AnimationFinished()
	{
		AnimationFinishedPlaying = true;

		if (AnimationFinishedConected)
		{
			Player.AnimatedSprite.AnimationFinished -= AnimationFinished;
			AnimationFinishedConected = false;
		}
	}
}
