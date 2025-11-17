using Godot;
using Godot.Collections;
using System;

public partial class ResearchManager : Node
{
    [Signal]
    public delegate void UnlockedNewResearchEventHandler(Node sender);

    private GameCore gameCore;
    private GameMenu gameMenu;

    [Export]
    private PackedScene researchTabScene;
    [Export]
    private NodePath availableResearchPath;
    private VBoxContainer availableResearchBar;
    [Export]
    private NodePath ownedResearchPath;
    private VBoxContainer ownedResearchBar;
    
    private Dictionary<string, ResearchTab> researchTabs = new Dictionary<string, ResearchTab>();

    // Initialization
    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr("ResearchManager | Initialization failed.");
            return;
        }

        gameCore.PrerequisitesUpdated += CheckPrerequisites;
    }
    private bool Initialize()
    {
        gameCore = GameCore.Instance;
        if (gameCore == null)
            return false;
        availableResearchBar = GetNodeOrNull<VBoxContainer>(availableResearchPath);
        if (researchTabScene == null)
            return false;
        ownedResearchBar = GetNodeOrNull<VBoxContainer>(ownedResearchPath);
        if (ownedResearchBar == null)
            return false;

        return true;
    }
    public void Setup(GameMenu menu)
    {
        gameMenu = menu;
        PopulateResearchBars();
    }

    // Runtime
    private void PopulateResearchBars()
    {
        if (gameCore == null || gameCore.gameData == null)
            return;

        foreach (var research in gameCore.gameData.Researches)
        {
            ResearchTab researchTab = researchTabScene.Instantiate<ResearchTab>();
            if (gameMenu.HasAllPrerequisites(research.Value.Prerequisites))
            {
                ownedResearchBar.AddChild(researchTab);
            }
            else
            {
                availableResearchBar.AddChild(researchTab);
                if (!gameMenu.HasAnyPrerequisites(research.Value.Prerequisites))
                {
                    researchTab.Hide();
                }
            }
            researchTabs.Add(research.Key, researchTab);
            researchTab.SetResearchInfo(research.Value);
            researchTab.ResearchButtonClicked += HandleResearchTabClicked;
        }
    }
    private void HandleResearchTabClicked(Node sender)
    {
        ResearchTab researchTab = sender as ResearchTab;
        foreach (var cost in researchTab.researchInfo.ResourceCost)
        {
            if (cost.Value > gameCore.GetResourceAmount(cost.Key))
                return;
        }
        researchTab.RequestAccepted();
        availableResearchBar.RemoveChild(sender);
        ownedResearchBar.AddChild(sender);
        gameCore.AddResearch(researchTab.researchInfo.Name);
        EmitSignal(SignalName.UnlockedNewResearch, sender);
        CheckPrerequisites();
    }
    private void CheckPrerequisites()
    {
        foreach (ResearchTab researchTab in researchTabs.Values)
        {
            if (gameMenu.HasAllPrerequisites(researchTab.researchInfo.Prerequisites))
            {
                researchTab.Show();
            }
        }
    }
    private void UpdateResearchTab(string research)
    {
    }
}
