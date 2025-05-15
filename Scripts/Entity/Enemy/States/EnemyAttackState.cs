using Godot;
using System;

public class EnemyAttackState : EntityState
{
    private Enemy Enemy => (Enemy)Entity;
    private bool _isAnimationFinished = false;
    private bool isConnected = false;
    
    public EnemyAttackState(Entity entity) : base(entity) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        _isAnimationFinished = false;
        Enemy.SetSwordHitbox(true);  // Enable the sword hitbox when attack starts

        // Connect to the animation finished signal when entering the state
        Enemy.AnimatedSprite.AnimationFinished += OnAttackAnimationFinished;
        isConnected = true;
    }

    public override void Exit()
    {
        base.Exit();
        _isAnimationFinished = false;
        Enemy.SetSwordHitbox(false);  // Disable the sword hitbox when attack ends

        if (isConnected)
            Enemy.AnimatedSprite.AnimationFinished -= OnAttackAnimationFinished;
    }

    public override void PhysicsUpdate(double delta)
    {
        // Keep the player stationary during attack
        Enemy.Velocity = new Vector2(0, 0);
        
        // If animation is finished, allow state transition
        if (_isAnimationFinished)
        {
            Enemy.ChangeState<EnemyIdleState>();
        }
    }

    private void OnAttackAnimationFinished()
    {
        _isAnimationFinished = true;
        Enemy._swordHitboxManager.ResetHitbox();

        // Disconnect to the animation finished signal when entering the state
        Enemy.AnimatedSprite.AnimationFinished -= OnAttackAnimationFinished;
        isConnected = false;
    }
}