using Godot;
using System.Collections.Generic;

public partial class SettingsMenu : Control
{
	private Label fpsDisplay;
	private Settings settings;
	private Label masterVolValue;
	private Label musicVolValue;
	private Label sfxVolValue;

	private int _masterBusIndex;
    private int _musicBusIndex;
    private int _effectsBusIndex;
	
	private Dictionary<int, int> _fpsDict = new Dictionary<int, int>
	{
		{ 0, 30 },
		{ 1, 60 },
		{ 2, 120 },
		{ 3, 144 },
		{ 4, 160 },
		{ 5, 165 },
		{ 6, 180 },
		{ 7, 200 },
		{ 8, 240 },
		{ 9, 0}
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		settings = GetNode<Settings>("/root/GlobalSettings");
		fpsDisplay = GetNode<Label>("../FpsCounter");
		masterVolValue = GetNode<Label>("MarginContainer/SettingTabs/Audio/MarginContainer2/AudioSettings/MasterVolValue");
		musicVolValue = GetNode<Label>("MarginContainer/SettingTabs/Audio/MarginContainer2/AudioSettings/MusicVolValue");
		sfxVolValue = GetNode<Label>("MarginContainer/SettingTabs/Audio/MarginContainer2/AudioSettings/SfxVolValue");

		_masterBusIndex = AudioServer.GetBusIndex("Master");
        _musicBusIndex = AudioServer.GetBusIndex("Music");
        _effectsBusIndex = AudioServer.GetBusIndex("Sound Effects");

		// Load settings into UI
		int windowMode = settings.GetSetting<int>("DisplayMode");
		OptionButton windowModeBtn = GetNode<OptionButton>("MarginContainer/SettingTabs/Video/MarginContainer/VideoSettings/DisplayOptions");
		windowModeBtn.Select(windowMode);

		bool vsync = settings.GetSetting<bool>("Vsync");
		CheckButton vsyncBtn = GetNode<CheckButton>("MarginContainer/SettingTabs/Video/MarginContainer/VideoSettings/VsyncBtn");
		vsyncBtn.ButtonPressed = vsync;

		bool fps = settings.GetSetting<bool>("DisplayFps");
		CheckButton fpsBtn = GetNode<CheckButton>("MarginContainer/SettingTabs/Video/MarginContainer/VideoSettings/DisplayFpsBtn");
		fpsBtn.ButtonPressed = fps;

		int maxfps = settings.GetSetting<int>("MaxFps");
		OptionButton maxfpsBtn = GetNode<OptionButton>("MarginContainer/SettingTabs/Video/MarginContainer/VideoSettings/MaxFpsBtn");
		maxfpsBtn.Select(maxfps);

		int masterVolume = settings.GetSetting<int>("MasterVolume");
		Slider masterSlider = GetNode<Slider>("MarginContainer/SettingTabs/Audio/MarginContainer2/AudioSettings/MasterSlider");
		masterSlider.Value = masterVolume;
		masterVolValue.Text = $"{masterVolume.ToString("+#;-#;0")}dB";

        int musicVolume = settings.GetSetting<int>("MusicVolume");
		Slider musicSlider = GetNode<Slider>("MarginContainer/SettingTabs/Audio/MarginContainer2/AudioSettings/MusicSlider");
		musicSlider.Value = musicVolume;
		musicVolValue.Text = $"{musicVolume.ToString("+#;-#;0")}dB";
        
		int sfxVolume = settings.GetSetting<int>("SfxVolume");
		Slider sfxSlider = GetNode<Slider>("MarginContainer/SettingTabs/Audio/MarginContainer2/AudioSettings/SfxSlider");
		sfxSlider.Value = sfxVolume;
		sfxVolValue.Text = $"{sfxVolume.ToString("+#;-#;0")}dB";
	}

	private void OnDisplayOptionsItemSelected(int value)
	{
		switch (value)
		{
			case 0:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				break;
			case 1:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				break;
		}
		settings.SetSetting("DisplayMode", value);
	}

	private void OnVsyncBtnChanged(bool value)
	{
		if (value)
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Enabled);
		else
			DisplayServer.WindowSetVsyncMode(DisplayServer.VSyncMode.Disabled);

		settings.SetSetting("Vsync", value);
	}

	private void OnDisplayFpsBtnChanged(bool value)
	{
		if (value)
			fpsDisplay.Show();
		else
			fpsDisplay.Hide();
		
		settings.SetSetting("DisplayFps", value);
	}

	private void OnMaxFpsItemChanged(int value)
	{
		Engine.MaxFps = _fpsDict[value];
		settings.SetSetting("MaxFps", value);
	}

	private void OnMasterSliderValueChanged(int value)
	{
		AudioServer.SetBusVolumeDb(_masterBusIndex, value);
		masterVolValue.Text = $"{value.ToString("+#;-#;0")}dB";
		settings.SetSetting("MasterVolume", value);
	}

	private void OnMusicSliderValueChanged(int value)
	{
		AudioServer.SetBusVolumeDb(_musicBusIndex, value);
		musicVolValue.Text = $"{value.ToString("+#;-#;0")}dB";
		settings.SetSetting("MusicVolume", value);
	}

	private void OnSfxSliderValueChanged(int value)
	{
		AudioServer.SetBusVolumeDb(_effectsBusIndex, value);
		sfxVolValue.Text = $"{value.ToString("+#;-#;0")}dB";
		settings.SetSetting("SfxVolume", value);
	}

	private void OnCloseBtnPressed()
	{
		var customNode = GetNode("../..");

		if (customNode != null && customNode.GetType() != typeof(Window))
		{
			MenuManager menuManager = customNode as MenuManager;
			menuManager.CloseSettingsMenu();
		}
		else
		{
			Hide();
		}
	}
}
