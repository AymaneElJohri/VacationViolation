using Godot;
using System;

[Tool]
public partial class ItemPickup : CharacterBody2D
{
	private ItemData _itemData;
	public ItemData itemData 
	{ 
		get => _itemData; 
		set => SetItemData(value);
	}

	public Area2D area2D;
	public Sprite2D sprite2D;
	public AudioStreamPlayer2D audioStreamPlayer2D;
	public Player player;

	public float Gravity = 300f;

	[Export] public string path = "res://Resources/Items/Coins/SingleCoins/Brons.tres";

	public override void _Ready()
	{
		area2D = GetNode<Area2D>("Area2D");
		sprite2D = GetNode<Sprite2D>("Sprite2D");
		audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		
		player = GetNode<Player>("../Player");

		UpdateTexture();

		if (Engine.IsEditorHint())
		{
			return;
		}

		area2D.BodyEntered += OnBodyEntered;

		itemData = GD.Load<ItemData>(path);
	}

    public override void _PhysicsProcess(double delta)
    {
		Velocity += new Vector2(0, Gravity) * (float)delta;

        var CollisionInfo = MoveAndCollide( Velocity * (float)delta);
		if (CollisionInfo != null)
		{
			Velocity = Velocity.Bounce(CollisionInfo.GetNormal());
			Velocity = Velocity.Clamp(new Vector2(-200, -200), new Vector2(200, 200));
		}

		Velocity -= Velocity * (float)delta * 4;

		MoveAndSlide();
    }

    public void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Player"))
		{
			if (itemData != null)
			{
				if (!itemData.NeedsToBeSold)
				{
					DataManager.Instance.AddCoins(itemData.Worth);
				}

				if (player.InventoryData.AddItem(itemData))
				{
					ItemPickedUp();
				}
			}
		}
	}

	public void ItemPickedUp()
	{
		area2D.BodyEntered -= OnBodyEntered;
		audioStreamPlayer2D.Play();
		Visible = false;
		audioStreamPlayer2D.Finished += RemoveItem;
	}

	private void RemoveItem()
	{
		QueueFree();
	}

	public void SetItemData(ItemData value)
	{
		_itemData = value;
		UpdateTexture();
	}

	public void UpdateTexture()
	{
		if (itemData != null && sprite2D != null)
		{
			sprite2D.Texture = itemData.Texture;
			sprite2D.Scale = new Vector2(1f/3f, 1f/3f);
		}
	}
}
