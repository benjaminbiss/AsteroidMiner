using Godot;
using Godot.Collections;

public partial class DevTools : Node2D
{
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            // CREDITS CHEAT
            if (keyEvent.Keycode == Key.F1)
            {
                GameCore.Instance.gameData.Resources["Credits"].Current += 1000000;
            }
            // POWER CHEAT
            if (keyEvent.Keycode == Key.F2)
            {
                GameCore.Instance.gameData.Resources["Power"].Current += 1000000;
            }
            // UNLOCK ALL RESEARCH CHEAT
            if (keyEvent.Keycode == Key.F3)
            {
                Dictionary<string, ResearchInfo> researches = DataUtil.Instance.GetResearchJSON();
                foreach (var research in researches)
                {
                    if (!GameCore.Instance.gameData.Prerequisites.Contains(research.Key))
                    {
                        GameCore.Instance.gameData.Prerequisites.Add(research.Key);
                    }
                }
                GameCore.Instance.EmitSignal(nameof(GameCore.PrerequisitesUpdated));
            }
        }
    }
}
