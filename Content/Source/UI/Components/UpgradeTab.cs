using Godot;
using Godot.Collections;

public partial class UpgradeTab : MarginContainer
{
    [Signal]
    public delegate void UpgradeButtonClickedEventHandler(Node sender);

    public UpgradeInfo upgradeInfo { get; private set; }

    [Export]
    private NodePath buttonPath;
    private Button button { get; set; }
    [Export]
    private NodePath upgradeLabelPath;
    private Label upgradeLabel;
    [Export]
    private NodePath upgradeTexturePath;
    private TextureRect upgradeTextureRect;
    [Export]
    private NodePath costLabelPath;
    private Label costLabel;
    [Export]
    private NodePath descriptionPath;
    private Label descriptionLabel;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("UpgradeTab | Initialization failed.");
            return;
        }

        UpdateUI();
        button.Pressed += OnUpgradeTabButtonPressed;
    }
    private bool Initialize()
    {
        button = GetNodeOrNull<Button>(buttonPath);
        if (button == null)
            return false;
        upgradeLabel = GetNodeOrNull<Label>(upgradeLabelPath);
        if (upgradeLabel == null)
            return false;
        upgradeTextureRect = GetNodeOrNull<TextureRect>(upgradeTexturePath);
        if (upgradeTextureRect == null)
            return false;
        costLabel = GetNodeOrNull<Label>(costLabelPath);
        if (costLabel == null)
            return false;
        descriptionLabel = GetNodeOrNull<Label>(descriptionPath);
        if (descriptionLabel == null)
            return false;

        return true;
    }

    public void SetUpgradeInfo(UpgradeInfo info)
    {
        upgradeInfo = info;
        UpdateUI();
    }
    private void UpdateUI()
    {
        if (upgradeInfo == null)
            return;
        
        upgradeLabel.Text = upgradeInfo.Name;
        costLabel.Text = ParseCost(upgradeInfo.ResourceCost);
        //upgradeTextureRect.Texture = upgradeInfo.IconPath;     
    }
    private void OnUpgradeTabButtonPressed()
    {
        EmitSignal(nameof(UpgradeButtonClicked), this);
    }
    public void RequestAccepted()
    {
        button.Disabled = true;
        costLabel.Hide();
    }
    private string ParseCost(Dictionary<string, double> cost)
    {
        string costString = "";
        foreach (var item in cost)
        {
            costString += $"{item.Key}: {item.Value} \n";
        }
        return costString.Trim();
    }
}
