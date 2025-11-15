using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameCore : Node
{
    [Signal]
    public delegate void ResourcesUpdatedEventHandler(string resource, double current, double max);
    public static GameCore Instance { get; private set; }
    public GameData gameData { get; private set; }
    public Dictionary<string, int> defaultInfos { get; private set; }
    public Dictionary<string, AssetInfo> assetInfos { get; private set; }
    public Dictionary<string, ResearchInfo> researchInfos { get; private set; }
    public Dictionary<string, ResourceInfo> resourceInfos { get; private set; }
    public Dictionary<string, UpgradeInfo> upgradeInfos { get; private set; }

    public override void _Ready()
    {
        Instance = this;

        LoadGameData();
    }

    private void LoadGameData()
    {
        gameData = DataUtil.Instance.LoadGame();
        defaultInfos = DataUtil.Instance.GetGameDefaults();
        assetInfos = DataUtil.Instance.GetDefaultAssets();
        researchInfos = DataUtil.Instance.GetDefaultResearch();
        resourceInfos = DataUtil.Instance.GetDefaultResources();
        upgradeInfos = DataUtil.Instance.GetDefaultUpgrades();

        if (gameData == null)
        {
            GD.PrintErr("GameCore | Failed to load game data.");
            return;
        }
    }

    public void AddResource(string resourceName, string paramater, double amount)
    {
        if (gameData.resources.ContainsKey(resourceName))
        {
            if (gameData.resources[resourceName].ContainsKey(paramater))
            {
                gameData.resources[resourceName][paramater] += amount;
            }
        }
        else
        {
            gameData.resources[resourceName][paramater] = amount;
        }
        EmitSignal(SignalName.ResourcesUpdated, resourceName, gameData.resources[resourceName]["Current"], gameData.resources[resourceName]["Max"]);
    }
    public void AddAsset(string assetName, int quantity)
    {
        if (gameData.assets.ContainsKey(assetName))
        {
            gameData.assets[assetName] += quantity;
            return;
        }
        gameData.assets[assetName] = quantity;
    }
    public void AddResearch(string research)
    {
        gameData.researches = gameData.researches.Append(research).ToArray();

        ResearchInfo researchInfo = researchInfos[research];
        if (researchInfo == null || researchInfo.UpgradeEffects == null)
            return;

        foreach (var name in researchInfo.UpgradeEffects)
        {
            if (CheckAssetsKey(name.Key))
            {
                foreach (var param in name.Value)
                {
                    foreach (var modifier in param.Value)
                    {
                        AddModifier(gameData.assetModifiers, name.Key, param.Key, modifier.Key, modifier.Value);
                    }
                }
            }
            else if (CheckResourcesKey(name.Key))
            {
                foreach (var param in name.Value)
                {
                    foreach (var modifier in param.Value)
                    {
                        AddModifier(gameData.resourceModifiers, name.Key, param.Key, modifier.Key, modifier.Value);
                    }
                }
            }
        }
    }
    public void AddUpgrade(string upgrade)
    {
        gameData.upgrades = gameData.upgrades.Append(upgrade).ToArray();

        UpgradeInfo upgradeInfo = upgradeInfos[upgrade];
        if (upgradeInfo == null || upgradeInfo.UpgradeEffects == null)
            return;

        foreach (var name in upgradeInfo.UpgradeEffects)
        {
            if (CheckAssetsKey(name.Key))
            {
                foreach (var param in name.Value)
                {
                    foreach (var modifier in param.Value)
                    {
                        AddModifier(gameData.assetModifiers, name.Key, param.Key, modifier.Key, modifier.Value);
                    }
                }
            }
            else if (CheckResourcesKey(name.Key))
            {
                foreach (var param in name.Value)
                {
                    foreach (var modifier in param.Value)
                    {
                        AddModifier(gameData.resourceModifiers, name.Key, param.Key, modifier.Key, modifier.Value);
                    }
                }
            }
        }
    }
    private bool CheckAssetsKey(string name)
    {
        if (gameData.assets.ContainsKey(name))
            return true;

        return false;
    }
    private bool CheckResourcesKey(string name)
    {
        if (gameData.resources.ContainsKey(name))
            return true;

        return false;
    }
    private void AddModifier(Dictionary<string, Dictionary<string, Dictionary<bool, double>>> dictionary, string name, string paramater, bool isAdditive, double modifier)
    {
        if (dictionary.ContainsKey(name))
        {
            if (dictionary[name].ContainsKey(paramater))
            {
                if (dictionary[name][paramater].ContainsKey(isAdditive))
                {
                    dictionary[name][paramater][isAdditive] += modifier;
                    return;
                }
                dictionary[name][paramater][isAdditive] += modifier;
                return;
            }
        }
        dictionary[name][paramater][isAdditive] = modifier;
    }

    public double GetResourceAmount(string key)
    {
        if (gameData.resources.ContainsKey(key))
        {
            if (gameData.resources[key].ContainsKey("Current"))
            {
                return gameData.resources[key]["Current"];
            }
        }
        return 0d;
    }
}
