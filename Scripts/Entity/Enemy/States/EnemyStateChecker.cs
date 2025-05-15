using Godot;
using System;

public class EnemyStateChecker
{
    private readonly Enemy _enemy;
    
    public EnemyStateChecker(Enemy enemy)
    {
        _enemy = enemy;
    }
    
    public void CheckStateTransitions()
    {
        // If in attack state, don't change states
        if (_enemy.CurrentState is EnemyAttackState || _enemy.CurrentState is EnemyDeathState)
        {
            return;
        }

        if (!_enemy.IsOnFloor())
		{
			CheckAirborneTransitions();
			return;
		}	

		if (_enemy.IsOnFloor())
		{
			CheckGroundedTransitions();
		}
    }
    
    private void CheckGroundedTransitions()
    {
        // Get the player's position
        Player player = _enemy.GetNode<Player>("../Player");
        Vector2 playerPosition = player.GlobalPosition;

        // Calculate the distance to the player
        float distanceToPlayer = _enemy.GlobalPosition.DistanceTo(playerPosition);
        float horizontalDistance = playerPosition.X - _enemy.GlobalPosition.X;

        // If close enough to attack
        if (distanceToPlayer <= _enemy.StopThreshold)
        {
            if (!(_enemy.CurrentState is EnemyAttackState))
            {
                _enemy.ChangeState<EnemyAttackState>();
                return;
            }
        }

        // Determine movement state based on player distance
        if (distanceToPlayer > _enemy.StopThreshold && distanceToPlayer <= _enemy.DetectionRange)
        {
            // If horizontal distance exceeds threshold, determine walk or run
            if (Mathf.Abs(horizontalDistance) > _enemy.StopThreshold)
            {
                if (!(_enemy.CurrentState is EnemyWalkState))
                {
					_enemy.MoveDirection = new Vector2(Mathf.Sign(horizontalDistance), 0);
                    _enemy.ChangeState<EnemyWalkState>();
                }
                return;
            }
        }

        // If no movement conditions met, go to idle
        _enemy.ChangeState<EnemyIdleState>();
    }
    
    private void CheckAirborneTransitions()
    {
        _enemy.ChangeState<EnemyFallState>();
    }
}