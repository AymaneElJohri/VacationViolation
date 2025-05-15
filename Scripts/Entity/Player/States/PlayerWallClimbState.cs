using Godot;
using System;

public class PlayerWallClimbState : EntityState
{
    private Player Player => (Player)Entity;
    private float ClimbSpeed = 50;

    public PlayerWallClimbState(Entity entity) : base(entity) {}

    public override void Enter()
    {
        Player.SetSpeed(Player.ClimbSpeed);
    }

    public bool IsOnLadderLeft()
	{
		// Get the TileMap node
		var tileMap = Player.GetNode<TileMap>("../TileMap");
		
		// Get the player's current tile position
		Vector2I tilePosition = tileMap.LocalToMap(Player.GlobalPosition);
		
		// Get the tile data at the player's current position
		TileData tileDataLeft = tileMap.GetCellTileData(0, tilePosition);
		
		if (tileDataLeft != null)
		{
			// Check if the tile has a custom data layer indicating it's a ladder
			return (bool)tileDataLeft.GetCustomData("is_ladder_left") == true;
		}
		
		return false;
	}

    public override void PhysicsUpdate(double delta)
    {
        // Adjust input based on ladder orientation
        bool moveUpInput = IsOnLadderLeft() ? Input.IsActionPressed($"player_{Player._playerNumber}_left") : Input.IsActionPressed($"player_{Player._playerNumber}_right");
        bool moveDownInput = IsOnLadderLeft() ? Input.IsActionPressed($"player_{Player._playerNumber}_right") : Input.IsActionPressed($"player_{Player._playerNumber}_left");

        if (moveUpInput)
        {
            Player.AnimatedSprite.Play(Player.CurrentState.GetAnimationName());
            Player.Velocity = new Vector2(Player.Velocity.X, -ClimbSpeed);
        }
        else if (moveDownInput)
        {
            Player.AnimatedSprite.PlayBackwards(Player.CurrentState.GetAnimationName());
            Player.Velocity = new Vector2(Player.Velocity.X, ClimbSpeed);
        }
        else
        {
            Player.AnimatedSprite.Play(Player.CurrentState.GetAnimationName());
            Player.AnimatedSprite.Pause();
            Player.Velocity = new Vector2(Player.Velocity.X, 0);
        }
    }
}
