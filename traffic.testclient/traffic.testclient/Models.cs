using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace traffic.testclient
{
    public class HoloLensTraffic
    {
        public float PosX;
        public float PosY;
        public float PosZ;

        public float RotationX;
        public float RotationY;
        public float RotationZ;
    }

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

    }

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

        public long CallDecidedTicks;

        public HoloLensTraffic CallDecided;

        public int NumberOfAttempts;

        public float ScanningPatternResult;
    }

    public class Envelope
    {
        public string content;

        public string type;
    }

    public class NetworkSpeechCommand
    {
        public string Text;
    }

}
