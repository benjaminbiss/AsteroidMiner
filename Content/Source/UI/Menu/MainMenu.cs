using Godot;

public partial class MainMenu : Control
{
    [Signal]
    public delegate void StartGameEventHandler();

    [Export]
    private NodePath playButtonPath;
    private Button playButton;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("MainMenu | Initialization failed.");
            return;
        }

        playButton.Pressed += OnStartButtonPressed;
    }

    private bool Initialize()
    {
        playButton = GetNodeOrNull<Button>(playButtonPath);
        if (playButton == null)
            return false;

        return true;
    }

    private void OnStartButtonPressed()
    {
        EmitSignal(nameof(StartGame));
    }
}
