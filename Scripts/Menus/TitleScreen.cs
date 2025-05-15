using Godot;
using System;
using System.Linq;
using System.IO;

public partial class TitleScreen : TextureRect
{
	private TextureRect _textureRect;
	private Button _continueButton;
	private AnimatedTexture _currentTexture;
	private string _currentAnimation = "idle";
	private const int SAVE_SLOTS = 4;
	private const string SAVE_PATH = "user://save_";
	private Timer _animationTimer;

	public override void _Ready()
	{
		_textureRect = GetNode<TextureRect>("MarginContainer/HorizontalContainer/MarginContainer/TextureRect");
		_continueButton = GetNode<Button>("MarginContainer/HorizontalContainer/VerticalContainer/MenuOptions/Continue");
		
		if (_textureRect == null)
		{
			GD.PrintErr("TextureRect not found!");
			return;
		}
		
		if (_continueButton == null)
		{
			GD.PrintErr("Continue Button not found!");
			return;
		}
		
		_animationTimer = new Timer();
		_animationTimer.OneShot = false;
		_animationTimer.WaitTime = 0.12f;
		AddChild(_animationTimer);
		_animationTimer.Timeout += OnAnimationComplete;
		
		UpdateContinueButtonText();
		SafeSetAnimation("idle");
	}
	
	private void OnAnimationComplete()
	{
		if (_currentTexture != null)
		{
			_currentTexture.Pause = true;
			_currentTexture.CurrentFrame = _currentTexture.Frames - 1;
		}
	}

	private void SafeSetAnimation(string newAnimation)
	{
		if (!IsInstanceValid(this) || !IsInsideTree())
			return;
		
		try 
		{
			if (_textureRect == null)
				return;
			
			var resourcePath = $"res://Animations/menu_{newAnimation}.tres";
			
			if (!ResourceLoader.Exists(resourcePath))
			{
				GD.PrintErr($"Animation resource not found: {resourcePath}");
				return;
			}
			
			var texture = ResourceLoader.Load<AnimatedTexture>(resourcePath);
			if (texture == null)
			{
				GD.PrintErr($"Failed to load texture: {resourcePath}");
				return;
			}
			
			_currentTexture = texture;
			_textureRect.Texture = texture;
			
			if (_currentTexture != null)
			{
				_currentTexture.Pause = false;
				_currentTexture.CurrentFrame = 0;
			}
			
			_currentAnimation = newAnimation;
			
			if (_animationTimer != null)
			{
				_animationTimer.Stop();
				_animationTimer.WaitTime = GetAnimationDuration(newAnimation);
				_animationTimer.Start();
			}
		}
		catch (Exception e)
		{
			GD.PrintErr($"Error in SafeSetAnimation: {e.Message}");
		}
	}
	
	private void PlayAnimationTexture(string animationName)
	{
		if (_textureRect == null)
		return;
		
		var resourcePath = $"res://Animations/menu_{animationName}.tres";

		// Check if resource exists before loading
		if (!ResourceLoader.Exists(resourcePath))
		{
			GD.PrintErr($"Animation resource not found: {resourcePath}");
			return;
		}

		var texture = ResourceLoader.Load<AnimatedTexture>(resourcePath);

		// Null check on loaded texture
		if (texture == null)
		{
			GD.PrintErr($"Failed to load texture: {resourcePath}");
			return;
		}

		_currentTexture = texture;

		_currentTexture.Pause = false;
		_currentTexture.CurrentFrame = 0;
		_textureRect.Texture = texture;

		_animationTimer.WaitTime = GetAnimationDuration(animationName);
		_animationTimer.Start();
	}
	
	private float GetAnimationDuration(string animationName)
	{
		return animationName switch
		{
			"menu_unhover" => 0.48f,
			"menu_hover" => 0.48f,
			"settings_hover" => 0.12f,
			"quit_hover" => 0.84f,
			_ => 0.12f
		};
	}
	
	private void UpdateContinueButtonText()
	{
		if (HasAnySaves())
		{
			_continueButton.Text = "Continue";
		}
		else
		{
			_continueButton.Text = "New Game";
		}
	}
	
	private bool HasAnySaves()
	{
		for (int i = 1; i <= SAVE_SLOTS; i++)
		{
			if (Godot.FileAccess.FileExists($"{SAVE_PATH}{i}.save"))
			{
				return true;
			}
		}
		return false;
	}
	
	private string GetMostRecentSaveFile()
	{
		string mostRecentSave = null;
		DateTime mostRecentTime = DateTime.MinValue;

		for (int i = 1; i <= SAVE_SLOTS; i++)
		{
			string savePath = $"{SAVE_PATH}{i}.save";
			if (Godot.FileAccess.FileExists(savePath))
			{
				var fileInfo = new FileInfo(ProjectSettings.GlobalizePath(savePath));
				if (fileInfo.LastWriteTime > mostRecentTime)
				{
					mostRecentTime = fileInfo.LastWriteTime;
					mostRecentSave = savePath;
				}
			}
		}
		
		return mostRecentSave;
	}
	
	private void _on_continue_mouse_entered()
	{
		SafeSetAnimation("continue_hover");
	}
	
	private void _on_continue_mouse_exited()
	{
		SafeSetAnimation("idle");
	}
	
	private void _on_continue_pressed()
	{
		if (HasAnySaves())
		{
			string mostRecentSave = GetMostRecentSaveFile();
			if (mostRecentSave != null)
			{
				LoadGame(mostRecentSave);
			}
			else
			{
				GD.PrintErr("Failed to determine most recent save file!");
			}
		}
		else
		{
			var sceneManager = GetNode<SceneManager>("/root/SceneManager");
			sceneManager.SwapScenes("res://Scenes/Gameplay.tscn",GetTree().Root,this,"fade_to_black");
			StartNewGame();
		}
	}
	
	private void LoadGame(string savePath)
	{
		GD.Print($"Loading game from: {savePath}");
	}
	
	private void StartNewGame()
	{
		if (_animationTimer != null)
		{
			_animationTimer.Stop();
			_animationTimer.QueueFree();
			_animationTimer = null;
		}
		
		GD.Print("Starting new game");
	}
	
	private void _on_load_mouse_entered()
	{
		SafeSetAnimation("continue_hover");
	}
	
	private void _on_load_mouse_exited()
	{
		SafeSetAnimation("idle");
	}
	
	private void _on_load_pressed()
	{
		OpenLoadMenu();
	}
	
	private void OpenLoadMenu()
	{
		for (int i = 1; i <= SAVE_SLOTS; i++)
		{
			string savePath = $"{SAVE_PATH}{i}.save";
			if (Godot.FileAccess.FileExists(savePath))
			{
				var fileInfo = new FileInfo(ProjectSettings.GlobalizePath(savePath));
				GD.Print($"Save Slot {i}: Last modified {fileInfo.LastWriteTime}");
			}
			else
			{
				GD.Print($"Save Slot {i}: Empty");
			}
		}
	}
	
	private void _on_settings_mouse_entered()
	{
		SafeSetAnimation("idle");
	}
	
	private void _on_settings_pressed()
	{
		var settingsMenu = GetNode<Control>("SettingsMenu");
		settingsMenu.Show();
	}
	
	private void _on_quit_mouse_entered()
	{
		SafeSetAnimation("quit_hover");
	}
	
	private void _on_quit_mouse_exited()
	{
		SafeSetAnimation("idle");
	}
	
	private void _on_quit_pressed()
	{
		GetTree().Quit();
	}
}
