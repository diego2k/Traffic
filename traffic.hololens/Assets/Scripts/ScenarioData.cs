public class ScenarioData
{
    public string Name { get; set; }

    public bool Collide { get; set; }

    /// <summary>
    /// 1: Traffic has the right of way
    /// 2: I have the right of way
    /// 3: Nobody has the right of way
    /// </summary>
    public int RightOfWay { get; set; }

    public bool TurnRight { get; set; }

    public int CompassCurrent { get; set; }

    public int CompassTarget { get; set; }

    public bool CompassTurnRight { get; set; }
}