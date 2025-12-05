using Godot;
using System;

public partial class MiningLaser : Node2D
{
    [Signal]
    public delegate void LaserFiredEventHandler(int index, int radius, double amount);

    private const string ASSET_NAME = "Mining Laser";
    public AssetSprite assetSprite { get; private set; }

    public override void _Ready()
    {
        if (!Initialize())
        {
            GD.PrintErr($"{ASSET_NAME} | Initialization failed.");
            return;
        }

        SetupSprite();
    }
    private bool Initialize()
    {
        assetSprite = GetNodeOrNull<AssetSprite>("Sprite2D");
        if (assetSprite == null)
            return false;

        return true;
    }
    private void SetupSprite()
    {
        assetSprite.Setup(ASSET_NAME);
    }
    
    public void Deploy(int index, Vector2 target, double amount)
    {        
        EmitSignal(SignalName.LaserFired, index, 1, amount);
    }
}
