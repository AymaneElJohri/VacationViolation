using Godot;
using System;

public partial class PauseMenu : Control
{
	public bool IsPaused = false;
	MenuManager menuManager;

    public override void _Ready()
    {
        menuManager = GetNode<MenuManager>("../..");
    }

    public void HandlePauseChange()
	{
		GetTree().Paused = IsPaused;
		
		if (IsPaused)
			Show();
		else
			Hide();
	}
	
	
	// public override void _UnhandledInput(InputEvent @event)
	// {
	// 	if (@event.IsActionPressed("pause"))
	// 	{
	// 		IsPaused = !IsPaused;
	// 		HandlePauseChange();
	// 		//GetTree().Paused = true;
	// 	}
	// }
	
	
	private void OnResumeButtonPressed()
	{
		menuManager.ClosePauseMenu();
	}
	
	private void OnSettingsButtonPressed()
	{
		menuManager.OpenSettingsMenu();
	}
	
	private void OnQuitMainButtonPressed()
	{
		menuManager.ClosePauseMenu();

		var sceneManager = GetNode<SceneManager>("/root/SceneManager");
		Node gameplayScene = GetNode("/root/Gameplay");
	
		sceneManager.SwapScenes(
			"res://Scenes/Menus/TitleScreen.tscn", 
			GetTree().Root, 
			gameplayScene, 
			"fade_to_black"
		);
	}
	
	private void OnQuitDesktopButtonPressed()
	{
		GetTree().Quit();
	}
}
