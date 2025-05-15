using Godot;
using System;
using Godot.Collections;

public partial class Enemy : Entity
{
    [Export] public float WalkSpeed = 50.0f;
    [Export] public CollisionShape2D MainCollision;
    [Export] public CharacterBody2D SwordHitbox;

	[Export] public float StopThreshold = 20.0f;
	[Export] public Vector2 MoveDirection = Vector2.Zero;
	[Export] public float DetectionRange = 100.0f;

	[Export] public float Health = 100;

    private EnemyStateChecker _stateChecker;
    private EnemyCollisionManager _collisionManager;
    public EnemySwordHitboxManager _swordHitboxManager;

	private Vector2 _knockbackVelocity = Vector2.Zero;
    [Export] private float KnockbackDuration = 0.2f; // Time enemy is stunned
    private float _knockbackTimer = 0f;
    private bool _isKnockedBack = false;

    [Export] private float HitFlashDuration = 0.2f; // Hitflash lasts
    private float _hitFlashTimer = 0f;
    private bool _hitFlash;

    [Export] public Array<DropData> drops;
    public PackedScene ItemPickup = GD.Load<PackedScene>("res://Scenes/Items/ItemPickup.tscn");

    public override void _Ready()
    {
        base._Ready();
        _stateChecker = new EnemyStateChecker(this);
        _collisionManager = new EnemyCollisionManager(this, MainCollision);
        _swordHitboxManager = new EnemySwordHitboxManager(this, SwordHitbox);
        if (_swordHitboxManager == null) {
            GD.Print("SwordHitbox is null");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_hitFlash)
        {
            _hitFlashTimer += (float)delta;

            // Check if hitflash duration is over
            if (_hitFlashTimer >= HitFlashDuration)
            {
                _hitFlashTimer = 0f;
                
                ShaderMaterial shaderMaterial = (ShaderMaterial)this.Material;

                shaderMaterial.SetShaderParameter("Active", false);
            }
        }

		if (_isKnockedBack)
        {
            _knockbackTimer += (float)delta;

            // Apply knockback velocity
            Velocity = _knockbackVelocity;

            // Check if knockback duration is over
            if (_knockbackTimer >= KnockbackDuration)
            {
                _isKnockedBack = false;
                _knockbackTimer = 0f;
                _knockbackVelocity = Vector2.Zero;
            }

            MoveAndSlide();
            return; // Skip normal movement logic while knocked back
        }

        _stateChecker.CheckStateTransitions();
        CurrentState.PhysicsUpdate(delta);
        
        MoveAndSlide();
        UpdateAnimation();
    }

    public override void InitializeStates()
    {
        States[typeof(EnemyIdleState)] = new EnemyIdleState(this);
        States[typeof(EnemyWalkState)] = new EnemyWalkState(this);
        States[typeof(EnemyAttackState)] = new EnemyAttackState(this);
        States[typeof(EnemyDeathState)] = new EnemyDeathState(this);
        States[typeof(EnemyFallState)] = new EnemyFallState(this);
        
        ChangeState<EnemyIdleState>();
    }

    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
        _collisionManager.UpdateCollisionShape(CurrentState.ToString(), AnimatedSprite.Frame);
        _swordHitboxManager.UpdateCollisionShape(CurrentState.ToString(), AnimatedSprite.Frame);
    }

    public void UpdateWalkVelocity()
    {
        var direction = Math.Sign(MoveDirection.X);
        Velocity = new Vector2(direction * Speed, Velocity.Y);

        // Change IsMovingRight based on movement direction
        if (direction == 1)
            IsMovingRight = true;
        else if (direction == -1)
            IsMovingRight = false;
    }

	public void TakeDamage(int damage, Vector2 knockbackDirection, float knockbackForce)
    {
        Health -= damage;

        // Apply knockback
        ApplyKnockback(knockbackDirection, knockbackForce);
        ApplyHitFlash();

        if (Health == 0)
        {
            ChangeState<EnemyDeathState>();
        }
    }

    private void ApplyHitFlash()
    {
        _hitFlashTimer = 0f;
        _hitFlash = true;

        ShaderMaterial shaderMaterial = (ShaderMaterial)this.Material;

        shaderMaterial.SetShaderParameter("Active", true);
    }

    private void ApplyKnockback(Vector2 direction, float force)
    {
        if (Health <= 0)
        {
            return;
        }

        _knockbackVelocity = new Vector2(direction.Normalized().X * force, 0);
        _isKnockedBack = true;
        _knockbackTimer = 0f;

        if (CurrentState is not EnemyIdleState)
        {
            ChangeState<EnemyIdleState>();
        }
    }

    public void SetSwordHitbox(bool value)
    {
        _swordHitboxManager.SetHitbox(value);
    }

    public void DropItems()
    {
        if (drops == null || drops.Count == 0)
            return;

        for (int i = 0; i < drops.Count; i++)
        {
            if (drops[i] == null)
                continue;
            
            int dropCount = drops[i].GetDropCount();

            for (int j = 0; j < dropCount; j++)
            {
                ItemPickup drop = ItemPickup.Instantiate() as ItemPickup;
                drop.itemData = drops[i].item;
                GetParent().AddChild(drop);
                drop.itemData = drops[i].item;
                drop.GlobalPosition = GlobalPosition;

                var randomDirection = new Vector2(1, 0).Rotated((float)GD.RandRange(-0.5, 0.5));
                drop.Velocity = randomDirection * (float)GD.RandRange(50, 100);
            }
        }
    }
}