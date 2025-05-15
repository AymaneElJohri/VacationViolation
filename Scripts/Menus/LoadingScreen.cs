using Godot;
using System;

public partial class LoadingScreen : CanvasLayer
{
	[Signal] public delegate void TransitionInCompleteEventHandler();

	private ProgressBar _progressBar;
	private AnimationPlayer _animPlayer;
	private Timer _timer;

	private string _startingAnimationName;

	public override void _Ready()
	{
		Layer = 100;

		_progressBar = GetNode<ProgressBar>("%ProgressBar");
		_animPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
		_timer = GetNode<Timer>("Timer");

		_progressBar.Visible = false;
	}

	public void start_transition(string animationName)
	{
		if (!_animPlayer.HasAnimation(animationName))
		{
			GD.PushWarning($"'{animationName}' animation does not exist");
			animationName = "fade_to_black";
		}

		_startingAnimationName = animationName;
		_animPlayer.Play(animationName);

		_timer.Start();
	}

	public async void finish_transition()
	{
		if (_timer != null)
		{
			_timer.Stop();
		}

		string endingAnimationName = _startingAnimationName.Replace("to", "from");

		if (!_animPlayer.HasAnimation(endingAnimationName))
		{
			GD.PushWarning($"'{endingAnimationName}' animation does not exist");
			endingAnimationName = "fade_from_black";
		}

		_animPlayer.Play(endingAnimationName);

		// Wait for animation to finish before freeing
		await ToSignal(_animPlayer, "animation_finished");
		QueueFree();
	}

	public void report_midpoint()
	{
		EmitSignal(SignalName.TransitionInComplete);
	}

	private void _on_timer_timeout()
	{
		_progressBar.Visible = true;
	}

	public void update_bar(float val)
	{
		_progressBar.Value = val;
	}
}
