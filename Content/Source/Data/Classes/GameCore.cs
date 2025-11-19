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
        int level = gameData.Assets[asset].Level += 1;
        if (level % 10 == 0)
            UpgradeAssetSpeed(asset);
        else if (level > 0)
            UpgradeAssetHarvest(asset);
        EmitSignal(SignalName.AssetUpdated, asset);
    }
    private void UpgradeAssetSpeed(string asset)
    {
        gameData.Assets[asset].DeploymentSpeed *= 1.2;
    }
    private void UpgradeAssetHarvest(string asset)
    {
        gameData.Assets[asset].HarvestAmount *= 1.2;
    }
    public void AddResearch(string research)
    {
        gameData.Prerequisites.Add(research);
        EmitSignal(SignalName.PrerequisitesUpdated);
    }
    public void AddResource(string resourceName, double amount)
    {
        gameData.Resources[resourceName].Current += amount;
        EmitSignal(SignalName.ResourcesUpdated, resourceName);
    }
    public void AddUpgrade(string upgrade)
    {
        gameData.Prerequisites.Add(upgrade);
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
    public void ChargeResourceCost(Dictionary<string, double> cost)
    {
        foreach (var resource in cost)
        {
            AddResource(resource.Key, -resource.Value);
        }
    }
    public void UpdateAsteroid(Array<int> points)
    {
        gameData.AsteroidPoints = points;
    }
}
