using Godot;
using System.Collections.Generic;

public partial class Main : Node2D
{
    [Export]
    private NodePath menuManagerPath;
    private MenuManager menuManager;
    [Export]
    private NodePath gameManagerPath;
    private GameManager gameManager;

    private bool bIsGameLaunched = false;
    private float currentPlayTime = 0f;
    private float autoSaveInterval = 60f;
    
    public GameCore gameCore;

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!bIsGameLaunched)
            return;

        AutoSave(delta);
    }

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("Main | Initialization failed.");
            return;
        }

        LoadGameData();
        SetupGameMenu();
        SetupBindings();
    }

    private bool Initialize()
    {
        gameCore = GameCore.Instance;
        menuManager = GetNodeOrNull<MenuManager>(menuManagerPath);
        if (menuManager == null)
            return false;
        gameManager = GetNodeOrNull<GameManager>(gameManagerPath);
        if (gameManager == null)
            return false;

        return true;
    }

    private void SetupBindings()
    {
        foreach (ResourceTab tab in menuManager.gameMenu.resourceTabs)
        {
            gameManager.ResourcesUpdated += tab.UpdateResourceAmount;
        }

        gameManager.ResourcesUpdated += UpdateResourceInGameData;
        menuManager.GameStarted += LaunchGame;
    }

    private void LoadGameData()
    {
        gameCore.gameData = DataUtil.Instance.LoadGame();

        if (gameCore.gameData == null)
        {
            GD.PrintErr("Main | Failed to load game data.");
            return;
        }
    }

    private void UpdateResourceInGameData(string resourceName, float current, float max)
    {
        if (gameCore.gameData.OwnedResources.ContainsKey(resourceName))
        {
            gameCore.gameData.OwnedResources[resourceName] = current;
        }
        else
        {
            gameCore.gameData.OwnedResources[resourceName] = current;
        }
    }    

    private void LaunchGame()
    {
        bIsGameLaunched = true;
        gameManager.StartGame();
    }
    private void AutoSave(double delta)
    {
        currentPlayTime += (float)delta;
        if (currentPlayTime >= autoSaveInterval)
        {
            gameCore.gameData.PlayTime += currentPlayTime;
            SaveGame();
            currentPlayTime = 0f;
        }
    }
    private void SaveGame()
    {
        DataUtil.Instance.SaveGame(gameCore.gameData);
    }
    public void UpdateAsteroidPoints(int[] points)
    {
        gameCore.gameData.AsteroidPoints = points;
    }


    private void SetupGameMenu()
    {
        GameMenu gameMenu = menuManager.GetNodeOrNull<GameMenu>("GameMenu");
        if (gameMenu == null)
        {
            GD.PrintErr("Main | GameMenu not found in MenuManager.");
            return;
        }
        gameMenu.SetResourceBar(DataUtil.Instance.GetDefaultResources());
        //gameMenu.SetAssetBar(DataUtil.Instance.GetDefaultAssets());
        //gameMenu.SetResearchesBar(DataUtil.Instance.GetDefaultResearch());
        //gameMenu.SetUpgradesBar(DataUtil.Instance.GetDefaultUpgrades());
    }
}