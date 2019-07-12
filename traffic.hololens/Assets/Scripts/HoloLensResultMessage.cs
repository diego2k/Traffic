using System;

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

    public bool TurnRight;

    public bool CompassTurnRight;

    public long TrafficStartTicks;

    public long CallTrafficTicks;

    public HoloLensTraffic CallTraffic;

    public long CallDecidedTicks;

    public HoloLensTraffic CallDecided;

}
