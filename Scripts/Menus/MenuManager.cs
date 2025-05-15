using Godot;
using System;

public partial class MenuManager : Node
{
    private PauseMenu pauseMenu;
    private DeathMenu deathMenu;
    private SettingsMenu settingsMenu;
    private Inventory inventoryMenu;

    private enum MenuState
    {
        None,
        Paused,
        Death,
        Settings,
        Inventory
    }

    private MenuState currentMenuState = MenuState.None;
    private MenuState previousMenuState = MenuState.None;

    public override void _Ready()
    {
        // Find the necessary menu nodes
        pauseMenu = GetNode<PauseMenu>("CanvasLayer/PauseMenu");
        deathMenu = GetNode<DeathMenu>("CanvasLayer/DeathMenu");
        settingsMenu = GetNode<SettingsMenu>("CanvasLayer/SettingsMenu");
        inventoryMenu = GetNode<Inventory>("CanvasLayer/Inventory");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Handle input based on current menu state
        if (@event.IsActionPressed("pause"))
        {
            HandlePauseInput();
        }
        else if (@event.IsActionPressed("inventory"))
        {
            HandleInventoryInput();
        }
    }

    private void HandlePauseInput()
    {
        // Prevent pausing if death menu is active
        if (currentMenuState == MenuState.Death)
            return;

        // Toggle pause menu if no other menu is open
        if (currentMenuState == MenuState.None)
        {
            OpenPauseMenu();
        }
        else if (currentMenuState == MenuState.Paused)
        {
            ClosePauseMenu();
        }
        else if (currentMenuState == MenuState.Settings)
        {
            CloseSettingsMenu();
        }
    }

    private void HandleInventoryInput()
    {
        // Prevent inventory if pause or death menu is active
        if (currentMenuState == MenuState.Paused || 
            currentMenuState == MenuState.Death || 
            currentMenuState == MenuState.Settings)
            return;

        // Toggle inventory
        if (currentMenuState == MenuState.None)
        {
            OpenInventory();
        }
        else if (currentMenuState == MenuState.Inventory)
        {
            CloseInventory();
        }
    }

    public void OpenPauseMenu()
    {
        // Close any open menus
        CloseAllMenus();

        // Open pause menu
        currentMenuState = MenuState.Paused;
        pauseMenu.IsPaused = true;
        pauseMenu.HandlePauseChange();
        GetTree().Paused = true;
    }

    public void ClosePauseMenu()
    {
        // Close pause menu
        currentMenuState = MenuState.None;
        pauseMenu.IsPaused = false;
        pauseMenu.HandlePauseChange();
        GetTree().Paused = false;
    }

    public void OpenDeathMenu()
    {
        // Close any open menus
        CloseAllMenus();

        // Open death menu
        currentMenuState = MenuState.Death;
        deathMenu.HandlePauseChange(true);
        GetTree().Paused = true;
    }

    public void CloseDeathMenu()
    {
        // Close death menu
        currentMenuState = MenuState.None;
        deathMenu.HandlePauseChange(false);
        GetTree().Paused = false;
    }

    public void OpenSettingsMenu()
    {
        // Only allow opening settings from pause or death menu
        if (currentMenuState == MenuState.Paused || currentMenuState == MenuState.Death)
        {
            previousMenuState = currentMenuState;
            currentMenuState = MenuState.Settings;
            settingsMenu.Show();
            GetTree().Paused = true;
        }
    }

    public void CloseSettingsMenu()
    {
        // Close settings and return to previous menu
        if (currentMenuState == MenuState.Settings)
        {
            currentMenuState = previousMenuState;
            settingsMenu.Hide();

            // Restore pause state based on previous menu
            if (currentMenuState == MenuState.Paused)
            {
                pauseMenu.Show();
            }
            else if (currentMenuState == MenuState.Death)
            {
                deathMenu.Show();
            }
        }
    }

    public void OpenInventory()
    {
        // Open inventory
        currentMenuState = MenuState.Inventory;
        inventoryMenu.IsInInventory = true;
        inventoryMenu.HandlePauseChange();
        GetTree().Paused = true;
    }

    public void CloseInventory()
    {
        // Close inventory
        currentMenuState = MenuState.None;
        inventoryMenu.IsInInventory = false;
        inventoryMenu.HandlePauseChange();
        GetTree().Paused = false;
    }

    private void CloseAllMenus()
    {
        // Close all possible menus
        switch (currentMenuState)
        {
            case MenuState.Paused:
                ClosePauseMenu();
                break;
            case MenuState.Death:
                CloseDeathMenu();
                break;
            case MenuState.Settings:
                CloseSettingsMenu();
                break;
            case MenuState.Inventory:
                CloseInventory();
                break;
        }
    }
}