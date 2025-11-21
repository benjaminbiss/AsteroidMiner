using Godot;

public partial class MenuManager : Control
{
    [Signal]
    public delegate void OnStartGameEventHandler();
    [Signal]
    public delegate void OnResearchSelectedEventHandler(string research);

    public MainMenu mainMenu;
    public GameMenu gameMenu;

    private Control activeMenu;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("MenuManager | Initialization failed.");
            return;
        }

        gameMenu.Visible = false;

        SetupBindings();
        SwitchMenu(mainMenu);
    }
    private bool Initialize()
    {
        mainMenu = GetNodeOrNull<MainMenu>("MainMenu");
        if (mainMenu == null)
            return false;

        gameMenu = GetNodeOrNull<GameMenu>("GameMenu");
        if (gameMenu == null)
            return false;

        return true;
    }
    private void SetupBindings()
    {
        mainMenu.OnStartGameButton += gameMenu.SetupTabManagers;
        mainMenu.OnStartGameButton += StartGame;
    }
    private void SwitchMenu(Control newMenu)
    {
        if (newMenu == null)
            return;

        if (activeMenu != null)        
            activeMenu.Visible = false;
        
        activeMenu = newMenu;
        activeMenu.Visible = true;
    }
    private void StartGame()
    {
        SwitchMenu(gameMenu);
        EmitSignal(nameof(OnStartGame));
    }
}
