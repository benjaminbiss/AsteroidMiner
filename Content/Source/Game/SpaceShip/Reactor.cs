using Godot;

public partial class Reactor : Node2D
{
    private const string ASSET_NAME = "Reactor";
    private GameCore gameCore;
    private AssetInfo assetInfo;
    public AssetSprite assetSprite { get; private set; }

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr($"{ASSET_NAME} | Initialization failed.");
            return;
        }
            
        SetupSprite();
    }
    private bool Initialize()
    {
        gameCore = GameCore.Instance;
        if (gameCore == null)
            return false;
        assetSprite = GetNodeOrNull<AssetSprite>("Sprite2D");
        if (assetSprite == null)
            return false;

        return true;
    }
    private void SetupSprite()
    {
        assetSprite.Setup(ASSET_NAME);
    }    
    public void UpdateAssetInfo(string asset)
    {
        if (ASSET_NAME != asset)
            return;

        assetInfo = gameCore.gameData.Assets[asset];
    }
}
