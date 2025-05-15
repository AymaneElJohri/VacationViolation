using Godot;
using System.Collections.Generic;

public class EnemyCollisionManager
{
    private readonly Enemy _enemy;
    private readonly CollisionShape2D _mainCollision;
    private readonly CollisionShape2D _hitCollisoin;
    private Dictionary<string, Dictionary<int, (Vector2 Size, Vector2 Offset)>> _collisionFrameData;

    public EnemyCollisionManager(Enemy enemy, CollisionShape2D mainCollision)
    {
        _enemy = enemy;
        _mainCollision = mainCollision;
        InitializeCollisionFrameData();
    }

    private void InitializeCollisionFrameData()
    {
        _collisionFrameData = new Dictionary<string, Dictionary<int, (Vector2, Vector2)>>
        {
            // Idle animation frames
            ["EnemyIdleState"] = new Dictionary<int, (Vector2, Vector2)>
            {
                [0] = (new Vector2(18, 30), new Vector2(0, -15))
                // Because the other frames are not defined it will always use frame 0
            },
            
            // Walk animation frames
            ["EnemyWalkState"] = new Dictionary<int, (Vector2, Vector2)>
            {
                [0] = (new Vector2(18, 30), new Vector2(0, -15))
                // Because the other frames are not defined it will always use frame 0, this is done because otherwise walking into walls would not work
            },

            // Attack animation frames
            ["EnemyAttackState"] = new Dictionary<int, (Vector2, Vector2)>
            {
                [0] = (new Vector2(18, 30), new Vector2(0, -15))
                // Because the other frames are not defined it will always use frame 0, this is done because otherwise walking into walls would not work
            }
        };
    }

    public void UpdateCollisionShape(string animationName, int frame)
    {
        if (!_collisionFrameData.TryGetValue(animationName, out var frameData))
            return;

        // Try to get frame-specific data, or use frame 0 as default
        if (!frameData.TryGetValue(frame, out var collisionData))
        {
            frameData.TryGetValue(0, out collisionData);
        }

        if (_mainCollision.Shape is CapsuleShape2D capsule)
        {
            capsule.Height = collisionData.Size.Y;
            capsule.Radius = collisionData.Size.X / 2;
            
            Vector2 offset = collisionData.Offset;
            if (!_enemy.IsMovingRight)
            {
                offset.X = -offset.X;
            }
            _mainCollision.Position = offset;
        }
    }
}