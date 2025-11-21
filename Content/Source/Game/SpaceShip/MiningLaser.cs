using Godot;
using System;

public partial class MiningLaser : Node2D
{
    [Signal]
    public delegate void MiningActionEventHandler(double amount);
    [Signal]
    public delegate void MiningLaserProgressEventHandler(double value, double max);

    private const string ASSET_NAME = "Mining Laser";
    private GameCore gameCore;
    private AssetInfo assetInfo;
    public AssetSprite assetSprite { get; private set; }
    private Timer timer;

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr($"{ASSET_NAME} | Initialization failed.");
            return;
        }

        SetupTimer();
        SetupSprite();
    }
    private bool Initialize()
    {
        gameCore = GameCore.Instance;
        if (gameCore == null)
            return false;
        assetInfo = gameCore.gameData.Assets[ASSET_NAME];
        if (assetInfo == null)
            return false;
        assetSprite = GetNodeOrNull<AssetSprite>("Sprite2D");
        if (assetSprite == null)
            return false;

        return true;
    }
    private void SetupSprite()
    {
        assetSprite.Setup(ASSET_NAME);
    }
    public void UpdateAssetInfo(string asset)
    {
        if (ASSET_NAME != asset)
            return;

        assetInfo = gameCore.gameData.Assets[asset];
        UpdateTimer();
    }
    private void SetupTimer()
    {
        timer = new Timer();
        timer.WaitTime = 60 / assetInfo.DeploymentSpeed;
        timer.OneShot = false;
        timer.Timeout += OnLaserTimeout;
        AddChild(timer);
        if (gameCore.gameData.Assets[ASSET_NAME].Level > 0)
            timer.Start();
    }
    private void UpdateTimer()
    {
        double elapsed = 0;
        if (timer.IsStopped())
             elapsed = 0;
        else
            elapsed = timer.WaitTime - timer.TimeLeft;

        timer.Stop();
        timer.WaitTime = 60 / assetInfo.DeploymentSpeed;
        elapsed = Math.Max(0.01, timer.WaitTime - elapsed);
        if (!timer.IsConnected("timeout", new Callable(this, nameof(ResetTimer))))
            timer.Timeout += ResetTimer;
        timer.Start(elapsed);
        EmitSignal(SignalName.MiningLaserProgress, timer.TimeLeft, timer.WaitTime);
    }
    private void ResetTimer()
    {
        timer.Stop();
        timer.WaitTime = 60 / assetInfo.DeploymentSpeed;
        //GD.Print($"{ASSET_NAME} | Timer updated to {timer.WaitTime} seconds.");
        timer.Start();
        timer.Timeout -= ResetTimer;
        EmitSignal(SignalName.MiningLaserProgress, timer.TimeLeft, timer.WaitTime);
    }
    private void OnLaserTimeout()
    {
        EmitSignal(SignalName.MiningAction, assetInfo.HarvestAmount);
    }
}
