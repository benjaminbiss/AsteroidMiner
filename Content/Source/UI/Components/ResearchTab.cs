using Godot;
using Godot.Collections;
using System.Linq;

public partial class ResearchTab : MarginContainer
{
    [Signal]
    public delegate void ResearchButtonClickedEventHandler(Node sender);

    [Export]
    private NodePath buttonPath;
    private Button button;
    [Export]
    private NodePath researchLabelPath;
    private Label researchLabel;
    [Export]
    private NodePath researchTexturePath;
    private TextureRect researchTextureRect;
    [Export]
    private NodePath rateLabelPath;
    private Label rateLabel;
    [Export]
    private NodePath costLabelPath;
    private Label costLabel;
    [Export]
    private NodePath progressBarPath;
    private ProgressBar progressBar;
    [Export]
    private NodePath descriptionPath;
    private Label descriptionLabel;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("ResearchTab | Initialization failed.");
            return;
        }

        button.Pressed += OnResearchTabButtonPressed;
    }
    private bool Initialize()
    {
        button = GetNodeOrNull<Button>(buttonPath);
        if (button == null)
            return false;
        researchLabel = GetNodeOrNull<Label>(researchLabelPath);
        if (researchLabel == null)
            return false;
        researchTextureRect = GetNodeOrNull<TextureRect>(researchTexturePath);
        if (researchTextureRect == null)
            return false;
        rateLabel = GetNodeOrNull<Label>(rateLabelPath);
        if (rateLabel == null)
            return false;
        costLabel = GetNodeOrNull<Label>(costLabelPath);
        if (costLabel == null)
            return false;
        progressBar = GetNodeOrNull<ProgressBar>(progressBarPath);
        if (progressBar == null)
            return false;
        descriptionLabel = GetNodeOrNull<Label>(descriptionPath);
        if (descriptionLabel == null)
            return false;

        return true;
    }
    public void SetupUI(ResearchInfo info)
    {
        researchLabel.Text = info.Name;
        if (info.IconPath != "")
            researchTextureRect.Texture = GD.Load<Texture2D>(info.IconPath);
        costLabel.Text = ParseCost(info.ResourceCost);
        descriptionLabel.Text = info.Description;
        rateLabel.Text = progressBar.Value.ToString();
        if (info.ResourceCost != null)
            progressBar.MaxValue = info.ResourceCost.First().Value;
    }
    public void UpdateResearchTab(string resource, double amount)
    {
        progressBar.Value += amount;
        rateLabel.Text = progressBar.Value.ToString("N2");
    }
    private void OnResearchTabButtonPressed()
    {
        EmitSignal(nameof(ResearchButtonClicked), this);
    }
    public void RequestAccepted()
    {
        button.Disabled = true;
        costLabel.Hide();
        progressBar.Hide();
    }
    private string ParseCost(Dictionary<string, double> cost)
    {
        string costString = "";
        if (cost == null)
        {
            progressBar.Hide();
            return "Free";
        }
        foreach (var item in cost)
        {
            costString += $"{item.Key}: {item.Value.ToString("N0")} \n";
        }
        return costString.Trim();
    }
    public string GetResearchName()
    {
        return researchLabel.Text;
    }
}
