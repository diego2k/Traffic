using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace traffic.server.Data
{
    public class HoloLensResultMessage
    {
        public string SzenarioName { get; set; }

        public bool Collide { get; set; }

        /// <summary>
        /// 1: Traffic has the right of way
        /// 2: I have the right of way
        /// 3: Nobody has the right of way
        /// </summary>
        public int RightOfWay { get; set; }

        public bool TurnRight { get; set; }

        public bool CompassTurnRight { get; set; }

        public long TrafficStartTicks { get; set; }

        public long CallTrafficTicks { get; set; }

        public HoloLensTraffic CallTraffic { get; set; }

        public long CallDecidedTicks { get; set; }

        public HoloLensTraffic CallDecided { get; set; }
    }
}
