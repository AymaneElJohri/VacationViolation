using Godot;
using System.Collections.Generic;

public class PlayerSwordHitboxManager
{
    private readonly Player _player;
    private readonly CollisionShape2D _swordHitbox;
    private Dictionary<string, Dictionary<int, SwordHitboxData>> _attackFrameData;

    // New struct to hold all hitbox data
    public struct SwordHitboxData
    {
        public Vector2 Position;
        public float Rotation;
        public Vector2 Size;

        public SwordHitboxData(Vector2 position, float rotation, Vector2 size)
        {
            Position = position;
            Rotation = rotation;
            Size = size;
        }
    }

    public PlayerSwordHitboxManager(Player player, CharacterBody2D swordHitbox)
    {
        _player = player;
        _swordHitbox = swordHitbox.GetNode<CollisionShape2D>("Area2D/SwordCollision");
        
        InitializeAttackFrameData();
        
        if (_swordHitbox != null)
        {
            _swordHitbox.Disabled = true;
        }
    }

    private void InitializeAttackFrameData()
    {
        _attackFrameData = new Dictionary<string, Dictionary<int, SwordHitboxData>>
        {
            ["PlayerAttack1State"] = new Dictionary<int, SwordHitboxData>
            {
                // Position: Where the hitbox is relative to player
                // Rotation: Angle in radians
                // Size: Width and height of the hitbox
                [0] = new SwordHitboxData(
                    position: new Vector2(-1, -21),
                    rotation: Mathf.DegToRad(-133.3f),
                    size: new Vector2(4.46f, 15.59f)
                ),
                [1] = new SwordHitboxData(
                    position: new Vector2(-2, -21),
                    rotation: Mathf.DegToRad(-133.3f),
                    size: new Vector2(4.46f, 15.59f)
                ),
                [2] = new SwordHitboxData(
                    position: new Vector2(-3, -20),
                    rotation: Mathf.DegToRad(-133.3f),
                    size: new Vector2(4.46f, 16.96f)
                ),
                [3] = new SwordHitboxData(
                    position: new Vector2(7, -15),
                    rotation: Mathf.DegToRad(-133.3f),
                    size: new Vector2(31.1f, 31.1f)
                ),
                [4] = new SwordHitboxData(
                    position: new Vector2(-15, -9),
                    rotation: Mathf.DegToRad(-113.3f),
                    size: new Vector2(4.46f, 16.96f)
                ),
                [5] = new SwordHitboxData(
                    position: new Vector2(-15, -9),
                    rotation: Mathf.DegToRad(-113.3f),
                    size: new Vector2(4.46f, 16.96f)
                )
            }
        };
    }

    public void SetHitbox(bool value)
    {
        // Using CallDeferred because otherwise it can happen when engine is calculting phisics which is not allowed
        if (_swordHitbox != null)
        {
            _swordHitbox.CallDeferred("set_disabled", !value);
        }
    }

    public void UpdateCollisionShape(string animationName, int frame)
    {
        if (_swordHitbox == null || !_attackFrameData.ContainsKey(animationName))
            return;

        var frameData = _attackFrameData[animationName];
        
        if (frameData.TryGetValue(frame, out var hitboxData))
        {
            Vector2 position = hitboxData.Position;
            float rotation = hitboxData.Rotation;

            if (!_player.IsMovingRight)
            {
                position = new Vector2(-position.X, position.Y);
                rotation = -rotation;
            }

            _swordHitbox.Position = position;
            _swordHitbox.Rotation = rotation;

            // Update the hitbox size
            if (_swordHitbox.Shape is CapsuleShape2D capsule)
            {
                if (capsule.ResourceLocalToScene == false)
                {
                    capsule = capsule.Duplicate() as CapsuleShape2D;
                    _swordHitbox.Shape = capsule;
                }

                capsule.Height = hitboxData.Size.Y;
                capsule.Radius = hitboxData.Size.X / 2;
            }
        }
    }

    public void SetCustomHitbox(Vector2 position, float rotationDegrees, Vector2 size)
    {
        if (_swordHitbox == null)
            return;

        Vector2 finalPosition = position;
        float finalRotation = Mathf.DegToRad(rotationDegrees);

        if (!_player.IsMovingRight)
        {
            finalPosition = new Vector2(-finalPosition.X, finalPosition.Y);
            finalRotation = -finalRotation;
        }

        _swordHitbox.Position = finalPosition;
        _swordHitbox.Rotation = finalRotation;

        if (_swordHitbox.Shape is CapsuleShape2D capsule)
        {
            capsule.Height = size.Y;
            capsule.Radius = size.X / 2;
        }
    }

    public void ResetHitbox()
    {
        _swordHitbox.Position = new Vector2(0, -15);
        _swordHitbox.Rotation = Mathf.DegToRad(90);

        if (_swordHitbox.Shape is CapsuleShape2D capsule)
        {
            capsule.Height = 17;
            capsule.Radius = 2;
        }
    }
}