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
        if (level > 1)
        {
            ChargeResourceCosts(gameData.Assets[asset].ResourceCost);
            if (level % 10 == 0)
                UpgradeAssetSpeed(asset);
            else
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
        double modifier = 1.4;
        if (asset == "Reactor")
            modifier = 1.3;

        foreach (var cost in gameData.Assets[asset].ResourceCost)
        {
            double value = baseAssetValues[asset].ResourceCost[cost.Key];
            gameData.Assets[asset].ResourceCost[cost.Key] = Mathf.FloorToInt(value * Mathf.Pow(modifier, gameData.Assets[asset].Level));
        }
    }
    private void UpdateAssetModifiers(Dictionary<string, Dictionary<string, Dictionary<bool, double>>> modifiers)
    {
        if (modifiers == null)
            return;

        foreach (var asset in modifiers)
        {
            foreach (var modType in asset.Value)
            {
                foreach (var mod in modType.Value)
                {
                    if (mod.Key) // true = additive
                    {
                        gameData.Assets[asset.Key].Modifiers[modType.Key][mod.Key] += mod.Value;
                    }
                    else // false = multiplicative
                    {
                        gameData.Assets[asset.Key].Modifiers[modType.Key][mod.Key] *= mod.Value;
                    }
                }
            }
            EmitSignal(SignalName.AssetUpdated, asset.Key);
        }
    }
    public void AddResearch(string research)
    {
        gameData.Prerequisites.Add(research);
        UpdateAssetModifiers(gameData.Researches[research].AssetModifiers);
        EmitSignal(SignalName.PrerequisitesUpdated);
    }
    private void AddResource(string resourceName, double amount)
    {
        gameData.Resources[resourceName].Current += amount;
        EmitSignal(SignalName.ResourcesUpdated, resourceName);
    }
    public void AddAssetCredits(string assetName)
    {
        string resourceName = gameData.Assets[assetName].HarvestedResource;
        double amount = gameData.Assets[assetName].HarvestAmount;
        amount += gameData.Assets[assetName].Modifiers["HarvestAmount"][true]; // additive
        amount *= gameData.Assets[assetName].Modifiers["HarvestAmount"][false]; // multiplicative
        AddResource(resourceName, amount);
    }
    public void AddAssetPower(string assetName, double delta)
    {
        string resourceName = gameData.Assets[assetName].HarvestedResource;
        double amount = gameData.Assets[assetName].HarvestAmount;
        amount += gameData.Assets[assetName].Modifiers["HarvestAmount"][true]; // additive
        amount *= gameData.Assets[assetName].Modifiers["HarvestAmount"][false]; // multiplicative
        double speed = gameData.Assets[assetName].DeploymentSpeed;
        speed += gameData.Assets[assetName].Modifiers["DeploymentSpeed"][true]; // additive
        speed *= gameData.Assets[assetName].Modifiers["DeploymentSpeed"][false]; // multiplicative
        amount = delta * (amount * speed) / 60d;
        AddResource(resourceName, amount);
    }
    public void ChargeCost(string resourceName, double cost)
    {
        AddResource(resourceName, -cost);
    }
    public void AddUpgrade(string upgrade)
    {
        gameData.Prerequisites.Add(upgrade);
        UpdateAssetModifiers(gameData.Upgrades[upgrade].AssetModifiers);
        EmitSignal(SignalName.PrerequisitesUpdated);
    }
    public void AddInfiniteUpgrade(string upgrade)
    {
        UpdateAssetModifiers(gameData.InfiniteUpgrades[upgrade].AssetModifiers);
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
    public void UpdateAsteroid(Array<int> points)
    {
        gameData.AsteroidPoints = points;
    }
}
