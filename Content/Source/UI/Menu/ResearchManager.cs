using Godot;
using Godot.Collections;
using System;
using System.Xml.Linq;

public partial class ResearchManager : Node
{
    [Signal]
    public delegate void SelectedResearchEventHandler(Node sender);
    [Signal]
    public delegate void DeselectedResearchEventHandler();

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
    private ResearchTab activeResearch;

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
            researchTab.SetupUI(research.Value);
            researchTab.ResearchButtonClicked += HandleResearchTabClicked;
        }
    }
    private void HandleResearchTabClicked(Node sender)
    {
        if (activeResearch == sender)
        {
            activeResearch = null;
            EmitSignal(SignalName.DeselectedResearch);
        }
        else
        {
            activeResearch = sender as ResearchTab;
            EmitSignal(SignalName.SelectedResearch, activeResearch);
        }

    }
    public void UnlockResearch(string name)
    {
        activeResearch.RequestAccepted();
        availableResearchBar.RemoveChild(activeResearch);
        ownedResearchBar.AddChild(activeResearch); 
        gameCore.AddResearch(activeResearch.GetResearchName());
        //EmitSignal(SignalName.UnlockedNewResearch, sender);
        CheckPrerequisites();
        activeResearch = null;
    }
    public void UpdateResearchProgress(string research, string resource, double progress)
    {
        if (researchTabs.ContainsKey(research))
        {
            researchTabs[research].UpdateResearchTab(resource, progress);
        }
    }
    private void CheckPrerequisites()
    {
        foreach (var tab in researchTabs)
        {
            ResearchTab researchTab = tab.Value;
            if (researchTab.Visible == false)
            {
                if (gameMenu.HasAllPrerequisites(gameCore.gameData.Researches[tab.Key].Prerequisites))
                    researchTab.Show();
            }
        }      
    }
}
