using UnityEngine;

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

    public string Turn;

    public int CompassCurrent;

    public string CompassTarget;

    public string CompassTurn;

    public override string ToString()
    {
        Debug.Log("Name " + Name + " Collide " + Collide + " RightOfWay " + RightOfWay + " Turn " + Turn + " CompassTurn " + CompassTurn);
        return base.ToString();
    }
}