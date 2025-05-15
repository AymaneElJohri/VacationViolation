using Godot;
using System;

public partial class Inventory : Control
{
	public static event Action shown;
    public static event Action hidden;

	public Label descriptionLabel;
	public Label useLabel;


	public bool IsInInventory = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		descriptionLabel = GetNode<Label>("MarginContainer2/Description");
		descriptionLabel.Text = "";

		useLabel = GetNode<Label>("UseLabel");
		useLabel.Hide();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void HandlePauseChange()
	{
		GetTree().Paused = IsInInventory;
		
		if (IsInInventory)
		{
			Show();
			shown?.Invoke();
		}
		else
		{
			Hide();
			hidden?.Invoke();
		}
	}

	public void UpdateItemDescription(string newText)
	{
		descriptionLabel.Text = newText;
	}
}
