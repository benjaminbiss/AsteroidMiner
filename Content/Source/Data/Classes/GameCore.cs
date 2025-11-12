using Godot;
using System;

public partial class GameCore : Node
{
    public static GameCore Instance { get; private set; }
    public GameData gameData { get; set; }
    public RunningData runningData { get; set; }

    public override void _Ready()
    {
        Instance = this;
    }
}
