using Godot;

public partial class Main : Node2D
{
    private GameCore gameCore;

    [Export]
    private NodePath menuManagerPath;
    private MenuManager menuManager;
    [Export]
    private NodePath gameManagerPath;
    private GameManager gameManager;

    private bool bIsGameLaunched = false;
    private float currentPlayTime = 0f;
    

    // Initialization
    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("Main | Initialization failed.");
            return;
        }

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
        menuManager.OnStartGame += LaunchGame;
        menuManager.gameMenu.OnTabClicked += gameManager.HandleTabClickedEvent;
        menuManager.gameMenu.ResearchDeselect += gameManager.HandleResearchDeselection;

        gameManager.UpdateResearch += menuManager.gameMenu.HandleUpdateResearchEvent;
        gameManager.ResearchUnlocked += menuManager.gameMenu.HandleUnlockResearchEvent;
    }

    // Runtime
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!bIsGameLaunched)
            return;

        AutoSave(delta);
    }   
    private void LaunchGame()
    {
        bIsGameLaunched = true;
        gameManager.StartGame(menuManager.gameMenu.assetManager);   
    }
    private void AutoSave(double delta)
    {
        currentPlayTime += (float)delta;
        if (currentPlayTime >= gameCore.gameData.Defaults["autoSaveInterval"])
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
}