using Godot;
using System;

public partial class DeathMenu : Control
{
	public bool IsPaused = false;
	MenuManager menuManager;

    public override void _Ready()
    {
        menuManager = GetNode<MenuManager>("../..");
    }

    public void HandlePauseChange(bool value)
	{
		IsPaused = value;
		GetTree().Paused = IsPaused;
		
		if (IsPaused)
			Show();
		else
			Hide();
	}

	private void OnRestartButtonPressed()
	{
		menuManager.CloseDeathMenu();

		var sceneManager = GetNode<SceneManager>("/root/SceneManager");

		Node gameplayScene = GetNode("/root/Gameplay/LevelHolder").GetChild(0);
   		string currentLevelPath = gameplayScene.SceneFilePath;

		sceneManager.SwapScenes(
			currentLevelPath,
			GetNode("/root/Gameplay/LevelHolder"),
			gameplayScene,
			"fade_to_black"
		);
	}
	
	private void OnQuitMainButtonPressed()
	{
		menuManager.CloseDeathMenu();

		HandlePauseChange(false);
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
