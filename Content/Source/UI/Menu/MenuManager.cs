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
        bool result = true;

        mainMenu = GetNodeOrNull<MainMenu>("MainMenu");
        result = mainMenu != null;

        gameMenu = GetNodeOrNull<GameMenu>("GameMenu");
        result = gameMenu != null;

        return result;
    }

    private void GameStarted()
    {
        SwitchMenu(gameMenu);
    }
}
