using Godot;

public partial class MenuManager : Control
{
    private MainMenu mainMenu;
    private GameMenu gameMenu;

    private Control activeMenu;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("MenuManager | Initialization failed.");
            return;
        }

        gameMenu.Visible = false;

        mainMenu.StartGame += GameStarted;
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

    private void GameStarted()
    {
        SwitchMenu(gameMenu);
    }
}
