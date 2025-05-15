using Godot;
using System;

public partial class DataManager : Node
{
	// For outside use of global nodes
	public static DataManager Instance { get; private set; }
	public AudioStreamPlayer SFXAudioStreamPlayer;
	public AudioStreamPlayer musicAudioStreamPlayer;
	public Player player;
	public Player player2;
	public Node currentLevel;

	private int coins = 0;
	public bool multiplayer = false;

	public override void _Ready()
	{
		Instance = this;
		SFXAudioStreamPlayer = GetNode<AudioStreamPlayer>("SFX Audio Player");
		musicAudioStreamPlayer = GetNode<AudioStreamPlayer>("Music Audio Player");
	}


	//Because this is an autoloaded script we can use it to play sounds and ad referecens to nodes that otherwise could not be referenced
	public void PlaySFXAudio(AudioStream audio)
	{
		SFXAudioStreamPlayer.Stream = audio;
		SFXAudioStreamPlayer.Play();
	}
	
	public void PlayMusicAudio(AudioStream audio)
	{
		musicAudioStreamPlayer.Stream = audio;
		musicAudioStreamPlayer.Play();
	}

	public void SetPlayer(Player newPlayer)
	{
		player = newPlayer;
	}

	public void SetPlayer2(Player newPlayer2)
	{
		player2 = newPlayer2;
	}

	public void SetCoins(int coinsToSet)
	{
		coins = coinsToSet;
		var coinCount = GetNode<Label>($"/root/Gameplay/HUD/CanvasLayer/IconsPlayer{player._playerNumber}/VBoxContainer/Coins/CoinCount");
		coinCount.Text = coins.ToString();
	}

	public void AddCoins(int coinsToAdd)
	{
		coins += coinsToAdd;
		var coinCount = GetNode<Label>($"/root/Gameplay/HUD/CanvasLayer/IconsPlayer{player._playerNumber}/VBoxContainer/Coins/CoinCount");
		coinCount.Text = coins.ToString();
	}

	public void SetCurrentLevel(Node value)
	{
		currentLevel = value;
	}
}
