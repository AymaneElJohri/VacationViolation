using Godot;
using System;

public partial class FpsCounter : Label
{
	public override void _Process(double delta)
	{
		Text = ($"FPS: {Engine.GetFramesPerSecond()}");
	}
}
