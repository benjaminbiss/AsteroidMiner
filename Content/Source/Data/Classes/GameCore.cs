using Godot;
using Godot.Collections;

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
    private Dictionary<string, AssetInfo> baseAssetValues;

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

        baseAssetValues = DataUtil.Instance.GetAssetsJSON();
    }

    public void UpgradeAsset(string asset)
    {        
        int level = gameData.Assets[asset].Level += 1;
        if (level % 10 == 0)
            UpgradeAssetSpeed(asset);
        else if (level > 1)
        {
            ChargeResourceCosts(gameData.Assets[asset].ResourceCost);
            UpgradeAssetHarvest(asset);
            UpgradeAssetCost(asset);
        }
        EmitSignal(SignalName.AssetUpdated, asset);
    }
    private void UpgradeAssetSpeed(string asset)
    {
        gameData.Assets[asset].DeploymentSpeed *= 2;
    }
    private void UpgradeAssetHarvest(string asset)
    {
        double value = baseAssetValues[asset].HarvestAmount;
        gameData.Assets[asset].HarvestAmount = value * gameData.Assets[asset].Level;
        // gameData.Assets[asset].HarvestAmount *= 1.2;
    }
    private void UpgradeAssetCost(string asset)
    {
        foreach (var cost in gameData.Assets[asset].ResourceCost)
        {
            double value = baseAssetValues[asset].ResourceCost[cost.Key];
            gameData.Assets[asset].ResourceCost[cost.Key] = Mathf.FloorToInt(value * Mathf.Pow(1.2, gameData.Assets[asset].Level));
        }
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
    public void ChargeResourceCosts(Dictionary<string, double> cost)
    {
        if (cost == null)
            return;
        foreach (var resource in cost)
        {
            if (resource.Value <= 0)
                continue;
            ChargeCost(resource.Key, resource.Value);
        }
    }
    public void ChargeCost(string resourceName, double cost)
    {
        AddResource(resourceName, -cost);
    }
    public void UpdateAsteroid(Array<int> points)
    {
        gameData.AsteroidPoints = points;
    }
}
