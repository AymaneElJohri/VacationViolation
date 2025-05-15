using Godot;
using System.Collections.Generic;

public class EnemySwordHitboxManager
{
    private readonly Enemy _enemy;
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

    public EnemySwordHitboxManager(Enemy enemy, CharacterBody2D swordHitbox)
    {
        _enemy = enemy;
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
            ["EnemyAttackState"] = new Dictionary<int, SwordHitboxData>
            {
                // Position: Where the hitbox is relative to player
                // Rotation: Angle in radians
                // Size: Width and height of the hitbox
                [0] = new SwordHitboxData(
                    position: new Vector2(0, 0),
                    rotation: Mathf.DegToRad(0),
                    size: new Vector2(0f, 0f)
                ),
                [1] = new SwordHitboxData(
                    position: new Vector2(0, 0),
                    rotation: Mathf.DegToRad(0),
                    size: new Vector2(0f, 0f)
                ),
                [2] = new SwordHitboxData(
                    position: new Vector2(0, 0),
                    rotation: Mathf.DegToRad(0),
                    size: new Vector2(0f, 0f)
                ),
                [3] = new SwordHitboxData(
                    position: new Vector2(0, 0),
                    rotation: Mathf.DegToRad(0),
                    size: new Vector2(0f, 0f)
                ),
                [4] = new SwordHitboxData(
                    position: new Vector2(18, -24),
                    rotation: Mathf.DegToRad(0),
                    size: new Vector2(26, 26)
                ),
                [5] = new SwordHitboxData(
                    position: new Vector2(0, 0),
                    rotation: Mathf.DegToRad(0),
                    size: new Vector2(0f, 0f)
                ),
                [6] = new SwordHitboxData(
                    position: new Vector2(0, 0),
                    rotation: Mathf.DegToRad(0),
                    size: new Vector2(0f, 0f)
                ),
                [7] = new SwordHitboxData(
                    position: new Vector2(0, 0),
                    rotation: Mathf.DegToRad(0),
                    size: new Vector2(0f, 0f)
                ),
                [8] = new SwordHitboxData(
                    position: new Vector2(9, -20),
                    rotation: Mathf.DegToRad(90),
                    size: new Vector2(24, 44)
                ),
                [9] = new SwordHitboxData(
                    position: new Vector2(0, 0),
                    rotation: Mathf.DegToRad(0),
                    size: new Vector2(0f, 0f)
                ),
                [10] = new SwordHitboxData(
                    position: new Vector2(0, 0),
                    rotation: Mathf.DegToRad(0),
                    size: new Vector2(0f, 0f)
                ),
                [11] = new SwordHitboxData(
                    position: new Vector2(0, 0),
                    rotation: Mathf.DegToRad(0),
                    size: new Vector2(0f, 0f)
                ),
                [12] = new SwordHitboxData(
                    position: new Vector2(0, 0),
                    rotation: Mathf.DegToRad(0),
                    size: new Vector2(0f, 0f)
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

            if (!_enemy.IsMovingRight)
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

        if (!_enemy.IsMovingRight)
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