using Godot;

public partial class AssetTab : MarginContainer
{
    public AssetInfo assetInfo { get; private set; }

    [Export]
    private NodePath assetLabelPath;
    private Label assetLabel;
    [Export]
    private NodePath assetTexturePath;
    private TextureRect assetTextureRect;
    [Export]
    private NodePath rateLabelPath;
    private Label rateLabel;
    [Export]
    private NodePath progressBarPath;
    private ProgressBar progressBar;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("AssetTab | Initialization failed.");
            return;
        }

        UpdateUI();
    }
    private bool Initialize()
    {
        assetLabel = GetNodeOrNull<Label>(assetLabelPath);
        if (assetLabel == null)
            return false;
        assetTextureRect = GetNodeOrNull<TextureRect>(assetTexturePath);
        if (assetTextureRect == null)
            return false;
        rateLabel = GetNodeOrNull<Label>(rateLabelPath);
        if (rateLabel == null)
            return false;
        progressBar = GetNodeOrNull<ProgressBar>(progressBarPath);
        if (progressBar == null)
            return false;

        return true;
    }
    public void SetAssetInfo(AssetInfo info)
    {
        assetInfo = info;
        UpdateUI();
    }
    private void UpdateUI()
    {
        if (assetInfo == null)
            return;
        
        assetLabel.Text = assetInfo.Name;
        //assetTextureRect.Texture = assetInfo.IconPath;     
    }
}
