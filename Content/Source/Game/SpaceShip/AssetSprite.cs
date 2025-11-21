using Godot;

public partial class AssetSprite : Sprite2D
{
    private string ASSET_NAME = "AssetSprite";
    private GameCore gameCore;    
    
    public void Setup(string name)
    {
        ASSET_NAME = name;

        gameCore = GameCore.Instance;
        if (gameCore == null)
        {
            GD.PrintErr($"{ASSET_NAME} | Initialization failed.");
            return;
        }

        if (gameCore.gameData.Prerequisites.Contains(ASSET_NAME))
            Activate(ASSET_NAME);
        else
            Visible = false;
    }
    public void Activate(string asset)
    {
        if (asset != ASSET_NAME)
            return;

        Visible = true;
        ModulateSprite();
        gameCore.AssetUpdated -= Activate;
    }
    private async void ModulateSprite()
    {
        for (int i = 0; i < 10; i++)
        {
            Modulate = new Color(1, 1, 1, 0); // Transparent
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");

            Modulate = new Color(1, 1, 1, 1); // White (normal)
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        }
    }
}
