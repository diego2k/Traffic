[System.Serializable]
public class ScenarioData
{
    public string Name;

    public bool Collide;

    /// <summary>
    /// 1: Traffic has the right of way
    /// 2: I have the right of way
    /// 3: Nobody has the right of way
    /// </summary>
    public int RightOfWay;

    public bool TurnRight;

    public int CompassCurrent;

    public string CompassTarget;

    public bool CompassTurnRight;
}