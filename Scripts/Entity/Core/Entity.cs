using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;

// TODO: Entity should have a member to hold an EntityStateMachine interface so the Entity can check the state transitions in _PhysicsProcess
public abstract partial class Entity : CharacterBody2D
{
	protected Dictionary<Type, EntityState> States = new Dictionary<Type, EntityState>();
	public EntityState CurrentState { get; protected set; }
	public AnimatedSprite2D AnimatedSprite;
	
	[Export] public float Speed { get; protected set; } = 200.0f;
	[Export] public float JumpVelocity { get; protected set; } = -400.0f;
	
	public bool IsMovingRight { get; set; } = true;
	
	public void SetSpeed(float newSpeed)
	{
		Speed = newSpeed;
	}

	public void SetJumpVelocity(float newJumpVelocity)
	{
		JumpVelocity = newJumpVelocity;
	}
	
	public override void _Ready()
	{
		AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		InitializeStates();
	}
	
	public abstract void InitializeStates();
	
	public override void _PhysicsProcess(double delta)
	{
		CurrentState?.PhysicsUpdate(delta);
		MoveAndSlide();
		UpdateAnimation();
	}
	
	public override void _Process(double delta)
	{
		CurrentState?.Update(delta);
	}
	
	public void ChangeState<T>() where T : EntityState
	{
		var newStateType = typeof(T);
		if (!States.ContainsKey(newStateType)) return;
		
		CurrentState?.Exit();
		CurrentState = States[newStateType];
		CurrentState.Enter();
	}
	
	protected virtual void UpdateAnimation()
	{
		if (AnimatedSprite == null) return;
		
		string animationName = CurrentState?.GetAnimationName() ?? "PlayerIdleState";
		
		try
		{
			// Only override animation if ShouldOverrideAnimationState returns true
			if (ShouldOverrideAnimationState(CurrentState))
			{
				if(!AnimatedSprite.IsPlaying() || AnimatedSprite.Animation != animationName)
				{
					AnimatedSprite.Play(animationName);
				}
			}
			
			AnimatedSprite.FlipH = !IsMovingRight;
		}
		catch (Exception e)
		{
			GD.PrintErr($"Failed to play animation {animationName}: {e.Message}");
		}
	}

	protected bool ShouldOverrideAnimationState(EntityState currentState)
	{
		// Don't override animation for wall climb state
		return !(CurrentState is PlayerWallClimbState);
	}

	public bool IsOnLadder()
	{
		// Get the TileMap node
		var tileMap = GetNode<TileMap>("../TileMap");
		
		// Get the player's current tile position
		Vector2I tilePosition = tileMap.LocalToMap(GlobalPosition);
		
		// Get the tile data at the player's current position
		TileData tileData = tileMap.GetCellTileData(0, tilePosition);
		
		if (tileData != null)
		{
			// Check if the tile has a custom data layer indicating it's a ladder
			return ((bool)tileData.GetCustomData("is_ladder_left") == true) || ((bool)tileData.GetCustomData("is_ladder_right") == true);
		}
		
		return false;
	}

	public bool IsOnFloorAndOnLadder()
	{
		// Get the TileMap node
		var tileMap = GetNode<TileMap>("../TileMap");
		
		// Get the player's current tile position
		Vector2I tilePosition = tileMap.LocalToMap(GlobalPosition);
		
		// Get the tile data at the player's current position
		TileData tileData = tileMap.GetCellTileData(0, tilePosition);
		
		if (IsOnFloor() && tileData != null)
		{
			if ((bool)tileData.GetCustomData("is_ladder_left") == true)
			{
				return true;
			}
			else if ((bool)tileData.GetCustomData("is_ladder_right") == true)
			{
				return true;
			}
		}

		return false;
	}
}
