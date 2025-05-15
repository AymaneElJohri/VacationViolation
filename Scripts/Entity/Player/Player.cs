using Godot;
using System;

public partial class Player : Entity
{
	[Export] public int _playerNumber = 1;
	[Export] public float RunSpeed = 300.0f;
	[Export] public float WalkSpeed = 200.0f;
	[Export] public float CrouchSpeed = 100.0f;
	[Export] public float ClimbSpeed = 100.0f;
	[Export] public CollisionShape2D MainCollision;
	[Export] public CharacterBody2D SwordHitbox;

	[Export] public int Health = 4;

	private int Coins = 0;

	private PlayerStateChecker _stateChecker;
	private PlayerCollisionManager _collisionManager;
	public PlayerSwordHitboxManager _swordHitboxManager;

	[Export] private float HitFlashDuration = 0.1f; // Hitflash lasts
    private float _hitFlashTimer = 0f;
    private bool _hitFlash;

	[Export] private float InvincibilityDuration = 1f;
	private float _invincibleTimer = 0f;
	private bool IsInvincible;

	public InventoryData InventoryData = GD.Load<InventoryData>("res://Resources/Inventory/PlayerInventory.tres");

	public override void _Ready()
	{
		base._Ready();
		_stateChecker = new PlayerStateChecker(this);
		_collisionManager = new PlayerCollisionManager(this, MainCollision);
		_swordHitboxManager = new PlayerSwordHitboxManager(this, SwordHitbox);
		if (_swordHitboxManager == null) {
			GD.Print("SwordHitbox is null");
		}

		IsInvincible = false;

		for (int i = 1; i <= Health; i++)
		{
			var heartNode = GetNode<TextureRect>($"/root/Gameplay/HUD/CanvasLayer/IconsPlayer{_playerNumber}/VBoxContainer/Hearts/Heart{i}");
			heartNode.Show();
		}

	}

	public override void _PhysicsProcess(double delta)
	{
		_stateChecker.CheckStateTransitions();
		CurrentState.PhysicsUpdate(delta);

		if (IsInvincible)
        {
            _invincibleTimer += (float)delta;
			ShaderMaterial shaderMaterial = (ShaderMaterial)this.Material;

            // Check if hitflash duration is over
            if (_invincibleTimer >= InvincibilityDuration)
            {
                _invincibleTimer = 0f;
				shaderMaterial.SetShaderParameter("Active", false);
                IsInvincible = false;
            }

			_hitFlashTimer += (float)delta;

			// Check if hitflash duration is over
			if (_hitFlashTimer >= HitFlashDuration)
			{
				_hitFlashTimer = 0f;

				if ((bool)shaderMaterial.GetShaderParameter("Active"))
					shaderMaterial.SetShaderParameter("Active", false);
				else
					shaderMaterial.SetShaderParameter("Active", true);
			}
        }

		MoveAndSlide();
		UpdateAnimation();
	}

	public override void InitializeStates()
	{
		States[typeof(PlayerIdleState)] = new PlayerIdleState(this);
		States[typeof(PlayerWalkState)] = new PlayerWalkState(this);
		States[typeof(PlayerRunState)] = new PlayerRunState(this);
		States[typeof(PlayerJumpState)] = new PlayerJumpState(this);
		States[typeof(PlayerFallState)] = new PlayerFallState(this);
		States[typeof(PlayerCrouchState)] = new PlayerCrouchState(this);
		States[typeof(PlayerWallClimbState)] = new PlayerWallClimbState(this);

		States[typeof(PlayerAttack1State)] = new PlayerAttack1State(this);
		States[typeof(PlayerDeathState)] = new PlayerDeathState(this);
		States[typeof(PlayerCrouchWalkState)] = new PlayerCrouchWalkState(this);
		
		ChangeState<PlayerIdleState>();
	}

	protected override void UpdateAnimation()
	{
		base.UpdateAnimation();
		// _collisionManager.UpdateCollisionShape(CurrentState.ToString(), AnimatedSprite.Frame);
		_swordHitboxManager.UpdateCollisionShape(CurrentState.ToString(), AnimatedSprite.Frame);
	}

	public void UpdateWalkVelocity()
	{
		var direction = Input.GetAxis($"player_{_playerNumber}_left", $"player_{_playerNumber}_right");
		Velocity = new Vector2(direction * Speed, Velocity.Y);

		// Only change IsMovingRight if the player is moving
		if (direction == 1)
			IsMovingRight = true;
		else if (direction == -1)
			IsMovingRight = false;
	}

	public void TakeDamage()
	{
		if (IsInvincible)
			return;

		HideHearts();
		Health -= 1;
		IsInvincible = true;

		ApplyHitFlash();

		if (Health <= 0)
		{
			ChangeState<PlayerDeathState>();
		}
	}

	public void HealByAmount(int value)
	{
		Health += value;

		if (Health > 4)
			Health = 4;
		
		for (int i = 1; i <= Health; i++)
		{
			var heartNode = GetNode<TextureRect>($"/root/Gameplay/HUD/CanvasLayer/IconsPlayer{_playerNumber}/VBoxContainer/Hearts/Heart{i}");
			heartNode.Show();
		}
	}

	private void ApplyHitFlash()
    {
        _hitFlashTimer = 0f;
        _hitFlash = true;

        ShaderMaterial shaderMaterial = (ShaderMaterial)this.Material;

        shaderMaterial.SetShaderParameter("Active", true);
    }

	private void HideHearts()
	{
		if (Health <= 0)
			return;

		var heartNode = GetNode<TextureRect>($"/root/Gameplay/HUD/CanvasLayer/IconsPlayer{_playerNumber}/VBoxContainer/Hearts/Heart{Health}");
		heartNode.Hide();
	}

	public void SetSwordHitbox(bool value)
	{
		_swordHitboxManager.SetHitbox(value);
	}
}
