using Godot;

public partial class ResearchTab : MarginContainer
{
    private ResearchInfo researchInfo;

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

        UpdateUI();
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

    public void SetResearchInfo(ResearchInfo info)
    {
        researchInfo = info;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (researchInfo == null)
            return;
        
        researchLabel.Text = researchInfo.Name;
        //ResearchTextureRect.Texture = ResearchInfo.IconPath;     
    }
}
