using Godot;

public partial class MenuManager : Control
{
    private Control MainMenu;

    private Control ActiveMenu;

    public override void _Ready()
    {
       SwitchMenu(MainMenu);
    }

    private void SwitchMenu(Control newMenu)
    {
        if (ActiveMenu != null)        
            ActiveMenu.Visible = false;
        
        ActiveMenu = newMenu;
        ActiveMenu.Visible = true;
    }
}
