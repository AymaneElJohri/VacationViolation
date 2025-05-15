using Godot;
using System.Collections.Generic;

public class PlayerCollisionManager
{
    private readonly Player _player;
    private readonly CollisionShape2D _mainCollision;
    private readonly CollisionShape2D _hitCollisoin;
    private Dictionary<string, Dictionary<int, (Vector2 Size, Vector2 Offset)>> _collisionFrameData;

    public PlayerCollisionManager(Player player, CollisionShape2D mainCollision)
    {
        _player = player;
        _mainCollision = mainCollision;
        InitializeCollisionFrameData();
    }

    private void InitializeCollisionFrameData()
    {
        _collisionFrameData = new Dictionary<string, Dictionary<int, (Vector2, Vector2)>>
        {
            // Idle animation frames
            ["PlayerIdleState"] = new Dictionary<int, (Vector2, Vector2)>
            {
                [0] = (new Vector2(18, 30), new Vector2(0, -15))
                // Because the other frames are not defined it will always use frame 0
            },
            
            // Walk animation frames
            ["PlayerWalkState"] = new Dictionary<int, (Vector2, Vector2)>
            {
                [0] = (new Vector2(18, 30), new Vector2(0, -15))
                // Because the other frames are not defined it will always use frame 0, this is done because otherwise walking into walls would not work
            },

            // Run animation frames
            ["PlayerRunState"] = new Dictionary<int, (Vector2, Vector2)>
            {
                [0] = (new Vector2(18, 30), new Vector2(0, -15))
                // Because the other frames are not defined it will always use frame 0, this is done because otherwise walking into walls would not work
            },

            // Crouch animation frames
            ["PlayerCrouchState"] = new Dictionary<int, (Vector2, Vector2)>
            {
                [0] = (new Vector2(18, 22), new Vector2(0, -11))
                // Because the other frames are not defined it will always use frame 0, this is done because otherwise walking into walls would not work
            },

            // Crouch walk animation frames
            ["PlayerCrouchWalkState"] = new Dictionary<int, (Vector2, Vector2)>
            {
                [0] = (new Vector2(18, 22), new Vector2(0, -11))
                // Because the other frames are not defined it will always use frame 0, this is done because otherwise walking into walls would not work
            },

            // Fall animation frames
            ["PlayerFallState"] = new Dictionary<int, (Vector2, Vector2)>
            {
                [0] = (new Vector2(18, 30), new Vector2(0, -15))
                // Because the other frames are not defined it will always use frame 0, this is done because otherwise walking into walls would not work
            },
            
            // Jump animation frames
            ["PlayerJumpState"] = new Dictionary<int, (Vector2, Vector2)>
            {
                [0] = (new Vector2(18, 26), new Vector2(0, -13)),
                [1] = (new Vector2(18, 24), new Vector2(0, -12)),
                [2] = (new Vector2(18, 28), new Vector2(0, -16)),
                [3] = (new Vector2(18, 22), new Vector2(0, -19))
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
            if (!_player.IsMovingRight)
            {
                offset.X = -offset.X;
            }
            _mainCollision.Position = offset;
        }
    }
}