using Godot;

public partial class MenuManager : Control
{
    [Signal]
    public delegate void GameStartedEventHandler();

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

        mainMenu.StartGame += StartGame;
        SwitchMenu(mainMenu);
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

    private void StartGame()
    {
        SwitchMenu(gameMenu);
        EmitSignal(nameof(GameStarted));
    }
}
