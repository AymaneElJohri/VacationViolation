using Godot;
using System;

public class EnemyDeathState : EntityState
{
    private Enemy Enemy => (Enemy)Entity;
    private Color _enemyModualte;
    private bool _isDead;
    
    public EnemyDeathState(Entity entity) : base(entity) 
    {

    }

    public override void Enter()
    {
        base.Enter();
        // Connect to the animation finished signal when entering the state
        Enemy.AnimatedSprite.AnimationFinished += OnDeathAnimationFinished;

        _enemyModualte = Enemy.AnimatedSprite.Modulate;

        Enemy.CallDeferred(nameof(DropItems));
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void PhysicsUpdate(double delta)
    {
        if (_isDead)
        {
            Enemy.AnimatedSprite.Frame = Enemy.AnimatedSprite.SpriteFrames.GetFrameCount(Enemy.AnimatedSprite.Animation) - 1;
            _enemyModualte = new Color( _enemyModualte.R,  _enemyModualte.G,  _enemyModualte.B,  _enemyModualte.A - (float)delta);
            Enemy.AnimatedSprite.Modulate =  _enemyModualte;
        }

        if (Enemy.AnimatedSprite.Modulate.A <= 0)
        {
            Enemy.QueueFree();
            Enemy.SwordHitbox.QueueFree();
        }

        // Keep the player stationary during attack
        Enemy.Velocity = new Vector2(0, 0);
    }

    private void OnDeathAnimationFinished()
    {
        // Disconnect to the animation finished signal when OnDeathAnimationFinished has been called
        Enemy.AnimatedSprite.AnimationFinished -= OnDeathAnimationFinished;

        _isDead = true;

        Enemy.AnimatedSprite.Pause();
    }

    public void DropItems()
    {
        //This is here so that you dont get a compiler error stating that DropItems does not exist in current context
    }
}