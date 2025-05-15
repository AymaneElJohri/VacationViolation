using Godot;
using System;

public class PlayerDeathState : EntityState
{
	private Player _player => (Player)Entity;
	private bool _isAnimationFinished = false;
	
	public PlayerDeathState(Entity entity) : base(entity) 
	{
		// Connect to the animation finished signal when entering the state
	}

	public override void Enter()
	{
		base.Enter();
		_isAnimationFinished = false;

		// Connect to the animation finished signal when entering the state
		_player.AnimatedSprite.AnimationFinished += OnAnimationFinished;
	}

	public override void Exit()
	{
		base.Exit();
		_isAnimationFinished = false;

		// Disconnect to the animation finished signal when exiting the state
		_player.AnimatedSprite.AnimationFinished -= OnAnimationFinished;
	}

	public override void PhysicsUpdate(double delta)
	{
		// Keep the player stationary during attack
		_player.Velocity = new Vector2(0, 0);
		
	}

	private void OnAnimationFinished()
	{
		_isAnimationFinished = true;
		// If animation is finished, remove enemy and its sword
		FadeOut();
	}

	private void FadeOut()
	{
		if (_player.CurrentState is PlayerDeathState);
		{
			MenuManager menuManager = _player.GetNode<MenuManager>("/root/Gameplay/HUD");
			menuManager.OpenDeathMenu();
		}
	}
}
