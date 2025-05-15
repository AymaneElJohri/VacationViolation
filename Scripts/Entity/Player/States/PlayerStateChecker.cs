using Godot;
using System;

public class PlayerStateChecker
{
	private readonly Player _player;
	private bool _wasOnWall = false;
	
	public PlayerStateChecker(Player player)
	{
		_player = player;
	}
	
	public void CheckStateTransitions()
	{
		if(_player.CurrentState is PlayerAttack1State || _player.CurrentState is PlayerDeathState)
		{
			return;
		}

		// Due to some inconsistency in the IsOnwall function this is needed

		if (_player.IsOnWall() || _wasOnWall)
		{
			if(_player.IsOnFloorAndOnLadder() == true)
			{
				_player.ChangeState<PlayerWalkState>();
				return;
			}

			if (_player.IsOnLadder())
			{
				_player.ChangeState<PlayerWallClimbState>();
				_wasOnWall = true;
				return;
			}
		}

		// Reset wall flag when not on wall
		if (!_player.IsOnWall())
		{
			_wasOnWall = false;
		}

		if (!_player.IsOnFloor())
		{
			CheckAirborneTransitions();
			return;
		}	

		if (_player.IsOnFloor())
		{
			CheckGroundedTransitions();
		}
	}
	
	private void CheckGroundedTransitions()
	{
		if (Input.IsActionJustPressed($"player_{_player._playerNumber}_jump"))
		{
			_player.ChangeState<PlayerJumpState>();
			return;
		}
		
		if (Input.IsActionJustPressed($"player_{_player._playerNumber}_attack"))
		{
			_player.ChangeState<PlayerAttack1State>();
			return;
		}

		var direction = Input.GetAxis($"player_{_player._playerNumber}_left", $"player_{_player._playerNumber}_right");
		if (Input.IsActionPressed($"player_{_player._playerNumber}_crouch") && direction == 0)
		{
			if (!(_player.CurrentState is PlayerCrouchState))
			{
				_player.ChangeState<PlayerCrouchState>();
			}
			return;
		}
		
		if (direction != 0)
		{
			if (Input.IsActionPressed($"player_{_player._playerNumber}_run") && !(_player.CurrentState is PlayerRunState) && !Input.IsActionPressed($"player_{_player._playerNumber}_crouch"))
			{
				_player.ChangeState<PlayerRunState>();
			}
			else if (!(_player.CurrentState is PlayerWalkState) && !Input.IsActionPressed($"player_{_player._playerNumber}_run") && !Input.IsActionPressed($"player_{_player._playerNumber}_crouch"))
			{
				_player.ChangeState<PlayerWalkState>();
			}
			else if (!(_player.CurrentState is PlayerCrouchWalkState) && Input.IsActionPressed($"player_{_player._playerNumber}_crouch") && !Input.IsActionPressed($"player_{_player._playerNumber}_run"))
			{
				_player.ChangeState<PlayerCrouchWalkState>();
			}
			return;
		}

		_player.ChangeState<PlayerIdleState>();
	}
	
	private void CheckAirborneTransitions()
	{
		if (!(_player.CurrentState is PlayerJumpState))
			_player.ChangeState<PlayerFallState>();
		
		if (_player.CurrentState is PlayerJumpState && _player.Velocity.Y > 0)
		{
			_player.ChangeState<PlayerFallState>();
		}
	}
}
