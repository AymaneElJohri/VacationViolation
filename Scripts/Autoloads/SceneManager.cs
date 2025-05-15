using Godot;
using System;

public partial class SceneManager : Node
{
	// Constants
	public const int LEVEL_H = 144;
	public const int LEVEL_W = 240;

	// Signals
	[Signal] public delegate void LoadStartEventHandler(Node loadingScreen);
	[Signal] public delegate void SceneAddedEventHandler(Node loadedScene, Node loadingScreen);
	[Signal] public delegate void LoadCompleteEventHandler(Node loadedScene);

	// Internal signals
	[Signal] public delegate void ContentFinishedLoadingEventHandler(Node content);
	[Signal] public delegate void ContentInvalidEventHandler(string contentPath);
	[Signal] public delegate void ContentFailedToLoadEventHandler(string contentPath);

	// Private fields
	private PackedScene _loadingScreenScene;
	private Node _loadingScreen;
	private string _transition;
	private Vector2 _zeldaTransitionDirection;
	private string _contentPath;
	private Timer _loadProgressTimer;
	private Node _loadSceneInto;
	private Node _sceneToUnload;
	private bool _loadingInProgress = false;
	
	public override void _Ready()
	{
		// Connect internal signals
		Connect(nameof(ContentInvalid), new Callable(this, nameof(_OnContentInvalid)));
		Connect(nameof(ContentFailedToLoad), new Callable(this, nameof(_OnContentFailedToLoad)));
		Connect(nameof(ContentFinishedLoading), new Callable(this, nameof(_OnContentFinishedLoading)));
		
		// Preload loading screen
		_loadingScreenScene = GD.Load<PackedScene>("res://Scenes/Menus/LoadingScreen.tscn");
	}

    // Add Loading Screen
    private void _AddLoadingScreen(string transitionType = "fade_to_black")
	{
		// Determine transition type
		_transition = transitionType == "no_transition" ? "no_to_transition" : transitionType;
		
		// Instantiate and add loading screen
		_loadingScreen = _loadingScreenScene.Instantiate();
		GetTree().Root.AddChild(_loadingScreen);
		
		// Start transition
		(_loadingScreen as Node).Call("start_transition", _transition);
	}
	
	// Swap Scenes (Main Method)
	public void SwapScenes(
		string sceneToLoad, 
		Node loadInto = null, 
		Node sceneToUnload = null, 
		string transitionType = "fade_to_black")
	{
		if (_loadingInProgress)
		{
			GD.PushWarning("SceneManager is already loading something");
			return;
		}
		
		_loadingInProgress = true;
		_loadSceneInto = loadInto ?? GetTree().Root;
		_sceneToUnload = sceneToUnload;
		
		_AddLoadingScreen(transitionType);
		_LoadContent(sceneToLoad);
	}
	
	// Zelda-style Scene Swap
	public void SwapScenesZelda(
		string sceneToLoad, 
		Node loadInto, 
		Node sceneToUnload, 
		Vector2 moveDir)
	{
		if (_loadingInProgress)
		{
			GD.PushWarning("SceneManager is already loading something");
			return;
		}
		
		_transition = "zelda";
		_loadSceneInto = loadInto;
		_sceneToUnload = sceneToUnload;
		_zeldaTransitionDirection = moveDir;
		_LoadContent(sceneToLoad);
	}
	
	// Load Content
	private void _LoadContent(string contentPath)
	{
		EmitSignal(SignalName.LoadStart, _loadingScreen);
		
		// Zelda transition doesn't use loading screen
		if (_transition != "zelda")
		{
			_loadingScreen.Call("report_midpoint");
		}
		
		_contentPath = contentPath;
		var loader = ResourceLoader.LoadThreadedRequest(contentPath);
		
		if (!ResourceLoader.Exists(contentPath) || loader == Error.Failed)
		{
			EmitSignal(SignalName.ContentInvalid, contentPath);
			return;
		}
		
		_loadProgressTimer = new Timer();
		_loadProgressTimer.WaitTime = 0.6;
		_loadProgressTimer.Timeout += _MonitorLoadStatus;
		
		GetTree().Root.AddChild(_loadProgressTimer);
		_loadProgressTimer.Start();
	}
	
	// Monitor Load Status
	private void _MonitorLoadStatus()
	{
		var loadProgress = new Godot.Collections.Array();
		var loadStatus = ResourceLoader.LoadThreadedGetStatus(_contentPath, loadProgress);
		
		switch (loadStatus)
		{
			case ResourceLoader.ThreadLoadStatus.InvalidResource:
				EmitSignal(SignalName.ContentInvalid, _contentPath);
				_loadProgressTimer.Stop();
				return;
				
			case ResourceLoader.ThreadLoadStatus.InProgress:
				if (_loadingScreen != null)
				{
					_loadingScreen.Call("update_bar", (float)loadProgress[0] * 100);
				}
				break;
				
			case ResourceLoader.ThreadLoadStatus.Failed:
				EmitSignal(SignalName.ContentFailedToLoad, _contentPath);
				_loadProgressTimer.Stop();
				return;
				
			case ResourceLoader.ThreadLoadStatus.Loaded:
				_loadProgressTimer.Stop();
				_loadProgressTimer.QueueFree();
				
				var loadedResource = ResourceLoader.LoadThreadedGet(_contentPath);
				
				if (loadedResource is PackedScene packedScene)
				{
					EmitSignal(
						SignalName.ContentFinishedLoading, 
						packedScene.Instantiate()
					);
				}
				else
				{
					GD.PrintErr($"Loaded resource is not a PackedScene: {_contentPath}");
					EmitSignal(SignalName.ContentInvalid, _contentPath);
				}
				return;
		}
	}
	
	// Content Loading Failure Handlers
	private void _OnContentInvalid(string path)
	{
		GD.PrintErr($"error: Cannot load resource: '{path}'");
	}
	
	private void _OnContentFailedToLoad(string path)
	{
		GD.PrintErr($"error: Failed to load resource: '{path}'");
	}
	
	// Content Finished Loading
	private void _OnContentFinishedLoading(Node incomingScene)
	{
		var outgoingScene = _sceneToUnload;
		
		// Data transfer between scenes
		if (outgoingScene != null)
		{
			if (outgoingScene.HasMethod("get_data") && incomingScene.HasMethod("receive_data"))
			{
				incomingScene.Call("receive_data", outgoingScene.Call("get_data"));
			}
		}
		
		// Add incoming scene
		_loadSceneInto.AddChild(incomingScene);
		EmitSignal(SignalName.SceneAdded, incomingScene, _loadingScreen);
		
		// Zelda transition specific logic
		if (_transition == "zelda")
		{
			// Slide new level in
			incomingScene.Set("position", new Vector2(
				_zeldaTransitionDirection.X * LEVEL_W,
				_zeldaTransitionDirection.Y * LEVEL_H
			));
			
			var tweenIn = GetTree().CreateTween();
			tweenIn.TweenProperty(incomingScene, "position", Vector2.Zero, 1)
				.SetTrans(Tween.TransitionType.Sine);
				
			// Slide old level out
			var tweenOut = GetTree().CreateTween();
			var vectorOffScreen = new Vector2(
				-_zeldaTransitionDirection.X * LEVEL_W,
				-_zeldaTransitionDirection.Y * LEVEL_H
			);
			tweenOut.TweenProperty(outgoingScene, "position", vectorOffScreen, 1)
				.SetTrans(Tween.TransitionType.Sine);
		}
		
		if (_sceneToUnload != null && _sceneToUnload != GetTree().Root)
		{
			_sceneToUnload.QueueFree();
		}
		
		// Scene initialization
		if (incomingScene.HasMethod("init_scene"))
		{
			incomingScene.Call("init_scene");
		}
		
		// Finish loading screen
		if (_loadingScreen != null)
		{
			_loadingScreen.Call("finish_transition");
		}
		
		// Start scene
		if (incomingScene.HasMethod("start_scene"))
		{
			incomingScene.Call("start_scene");
		}
		
		// Complete loading
		_loadingInProgress = false;
		EmitSignal(SignalName.LoadComplete, incomingScene);
	}
}
