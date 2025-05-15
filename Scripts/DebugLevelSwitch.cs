using Godot;
using System;

public partial class DebugLevelSwitch : MarginContainer
{
	Button button0;
	Button button1;
	Button button2;

	SceneManager sceneManager;
	Node2D levelHolder;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		button0 = GetNode<Button>("HBoxContainer/Button1");
		button0.Pressed += button0Click;
		button1 = GetNode<Button>("HBoxContainer/Button2");
		button1.Pressed += button1Click;
		button2 = GetNode<Button>("HBoxContainer/Button3");
		button2.Pressed += button2Click;

		sceneManager = GetNode<SceneManager>("/root/SceneManager");
		levelHolder = GetNode<Node2D>("/root/Gameplay/LevelHolder/");
	}

	public void button0Click()
	{
		sceneManager.SwapScenes("res://Scenes/Worlds/World0/Level0.tscn",levelHolder,DataManager.Instance.currentLevel,"fade_to_black");
	}

	public void button1Click()
	{
		sceneManager.SwapScenes("res://Scenes/Worlds/World0/Level1.tscn",levelHolder,DataManager.Instance.currentLevel,"fade_to_black");
	}

	public void button2Click()
	{
		sceneManager.SwapScenes("res://Scenes/Worlds/World0/Level Lars.tscn",levelHolder,DataManager.Instance.currentLevel,"fade_to_black");
	}
}
