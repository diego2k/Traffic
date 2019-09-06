using System;
using UnityEngine;

public class HoloLensResultMessage
{
    public string SzenarioName;

    public bool Collide;

    /// <summary>
    /// 1: Traffic has the right of way
    /// 2: I have the right of way
    /// 3: Nobody has the right of way
    /// </summary>
    public int RightOfWay;

    public string Turn;

    public string CompassTurn;

    public long TrafficStartTicks;

    public long CallTrafficTicks;

    public HoloLensTraffic CallTraffic;

    public long CallDecidedTicks;

    public HoloLensTraffic CallDecided;

    public int NumberOfAttempts;

    public override string ToString()
    {
        Debug.Log("SzenarioName " + SzenarioName + " Collide " + Collide + " RightOfWay " + RightOfWay + " Turn " + Turn + " CompassTurn " + CompassTurn);
        return base.ToString();
    }
}
