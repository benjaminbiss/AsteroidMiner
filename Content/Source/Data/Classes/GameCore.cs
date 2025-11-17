using Godot;
using Godot.Collections;
using System;

public partial class GameCore : Node
{
    [Signal]
    public delegate void ResourcesUpdatedEventHandler(string name);
    [Signal]
    public delegate void AssetUpdatedEventHandler(string name);
    [Signal]
    public delegate void PrerequisitesUpdatedEventHandler();

    public static GameCore Instance { get; private set; }
    public GameData gameData { get; private set; }

    public override void _Ready()
    {
        Instance = this;

        LoadGameData();
    }

    private void LoadGameData()
    {
        gameData = DataUtil.Instance.LoadGame();

        if (gameData == null)
        {
            GD.PrintErr("GameCore | Failed to load game data.");
            return;
        }
    }

    public void AddAsset(string asset)
    {
        AssetInfo assetInfo = gameData.Assets[asset];
        // Apply asset effects here
        EmitSignal(SignalName.AssetUpdated, asset);
    }
    public void AddResearch(string research)
    {
        gameData.Preresquisites.Add(research);
        ResearchInfo researchInfo = gameData.Researches[research];
        // Apply research effects here
        EmitSignal(SignalName.PrerequisitesUpdated);
    }
    public void AddResource(string resourceName, double amount)
    {
        gameData.Resources[resourceName].Current += amount;
    }
    public void AddUpgrade(string upgrade)
    {
        gameData.Preresquisites.Add(upgrade);
        UpgradeInfo upgradeInfo = gameData.Upgrades[upgrade];
        // Apply upgrade effects here
        EmitSignal(SignalName.PrerequisitesUpdated);
    }

    public double GetResourceAmount(string key)
    {
        if (gameData.Resources.ContainsKey(key))
        {
            return gameData.Resources[key].Current;
        }
        return 0d;
    }
    public void UpdateAsteroid(Array<int> points)
    {
        gameData.AsteroidPoints = points;
    }
}
