using Godot;
using Godot.Collections;
using System;

public partial class GameMenu : Control
{
    [Signal]
    public delegate void OnTabClickedEventHandler(Node sender);
    [Signal]
    public delegate void ResearchDeselectEventHandler();

    private GameCore gameCore;

    [Export]
    private NodePath assetManagerPath;
    private AssetManager assetManager;
    [Export]
    private NodePath researchManagerPath;
    private ResearchManager researchManager;
    [Export]
    private NodePath resourceManagerPath;
    private ResourceManager resourceManager;
    [Export]
    private NodePath upgradeManagerPath;
    private UpgradeManager upgradeManager;

    // Initialization
    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("GameMenu | Initialization failed.");
            return;
        }
    }
    private bool Initialize()
    {
        gameCore = GameCore.Instance;
        if (gameCore == null)
            return false;
        assetManager = GetNodeOrNull<AssetManager>(assetManagerPath);
        if (assetManager == null)
            return false;
        researchManager = GetNodeOrNull<ResearchManager>(researchManagerPath);
        if (researchManager == null)
            return false;
        resourceManager = GetNodeOrNull<ResourceManager>(resourceManagerPath);
        if (resourceManager == null)
            return false;
        upgradeManager = GetNodeOrNull<UpgradeManager>(upgradeManagerPath);
        if (upgradeManager == null)
            return false;

        return true;
    }
    public void SetupTabManagers()
    {
        assetManager.Setup(this);
        assetManager.AssetUpgraded += RelayTabClicked;
        researchManager.Setup(this);
        researchManager.SelectedResearch += RelayTabClicked;
        researchManager.DeselectedResearch += RelayDeselectResearch;
        resourceManager.Setup(this);
        upgradeManager.Setup(this);
        upgradeManager.UnlockedNewUpgrade += RelayTabClicked;
    }

    // Runtime
    private void RelayTabClicked(Node sender)
    {
        EmitSignal(nameof(OnTabClicked), sender);
    }
    private void RelayDeselectResearch()
    {
        EmitSignal(nameof(ResearchDeselect));
    }
    public bool HasAllPrerequisites(Array<string> requiredPrereqs)
    {
        if (requiredPrereqs == null)
            return false;

        if (requiredPrereqs.Count == 0)
            return true;

        foreach (string prereq in requiredPrereqs)
        {
            if (!gameCore.gameData.Prerequisites.Contains(prereq))
                return false;
        }        
        return true;
    }
    public bool HasAnyPrerequisites(Array<string> requiredPrereqs)
    {
        if (requiredPrereqs == null)
            return true;

        foreach (string prereq in requiredPrereqs)
        {
            if (gameCore.gameData.Prerequisites.Contains(prereq))
                return true;
        }
        return false;
    }

    public void HandleUpdateResearchEvent(string research, string resource, double amount)
    {
       researchManager.UpdateResearchProgress(research, resource, amount);
    }

    public void HandleUnlockResearchEvent(string research)
    {
        researchManager.UnlockResearch(research);
    }
}
