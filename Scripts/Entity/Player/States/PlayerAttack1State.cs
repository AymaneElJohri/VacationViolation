using Godot;
using System;

public class PlayerAttack1State : EntityState
{
    private Player Player => (Player)Entity;
    private bool _isAnimationFinished = false;
    
    public PlayerAttack1State(Entity entity) : base(entity) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        _isAnimationFinished = false;
        Player.SetSwordHitbox(true);  // Enable the sword hitbox when attack starts

        // Connect to the animation finished signal when entering the state
        Player.AnimatedSprite.AnimationFinished += OnAnimationFinished;
    }

    public override void Exit()
    {
        base.Exit();
        _isAnimationFinished = false;
        Player.SetSwordHitbox(false);  // Disable the sword hitbox when attack ends

        // Disconnect to the animation finished signal when exiting the state
        Player.AnimatedSprite.AnimationFinished -= OnAnimationFinished;
    }

    public override void PhysicsUpdate(double delta)
    {
        // Keep the player stationary during attack
        Player.Velocity = new Vector2(0, 0);
        
        // If animation is finished, allow state transition
        if (_isAnimationFinished)
        {
            Player.ChangeState<PlayerIdleState>();
        }
    }

    private void OnAnimationFinished()
    {
        _isAnimationFinished = true;
        Player._swordHitboxManager.ResetHitbox();
    }
}