namespace traffic.server.Data
{
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

        public string Turn { get; set; }

        public int CompassCurrent { get; set; }

        public string CompassTarget { get; set; }

        public string CompassTurn { get; set; }
    }
}
