using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using traffic.server.Helper;

namespace traffic.server.Data
{
    public class AircraftKinematicsData
    {
        public double SimulatedTime { get; set; }
        public double longitude;
        public double latitude;
        public double altitude;
        public double true_heading;
        public double pitch_angle;
        public double roll_angle;
        public float absolute_velocity_x;
        public float absolute_velocity_y;
        public float absolute_velocity_z;
        public float roll_rate;
        public float pitch_rate;
        public float yaw_rate;
        public float center_of_mass_acceler_x;
        public float center_of_mass_acceler_y;
        public float center_of_mass_acceler_z;
        public float roll_acceleration;
        public float pitch_acceleration;
        public float yaw_acceleration;
        public float ground_speed;
        public float true_track;

        public AircraftKinematicsData() { }

        public AircraftKinematicsData(List<double> doubleList, List<float> floatList)
        {

            this.SimulatedTime = doubleList[0];
            this.longitude = Utils.ConvertRadiansToDegrees(doubleList[1]);
            this.latitude = Utils.ConvertRadiansToDegrees(doubleList[2]);
            this.altitude = doubleList[3];

            this.true_heading = Utils.ConvertRadiansToDegrees(System.Convert.ToDouble(floatList[0]));
            this.pitch_angle = Utils.ConvertRadiansToDegrees(System.Convert.ToDouble(floatList[1]));
            this.roll_angle = Utils.ConvertRadiansToDegrees(System.Convert.ToDouble(floatList[2]));
            this.absolute_velocity_x = floatList[3];
            this.absolute_velocity_y = floatList[4];
            this.absolute_velocity_z = floatList[5];
            this.roll_rate = floatList[6];
            this.pitch_rate = floatList[7];
            this.yaw_rate = floatList[8];
            this.center_of_mass_acceler_x = floatList[9];
            this.center_of_mass_acceler_y = floatList[10];
            this.center_of_mass_acceler_z = floatList[11];
            this.roll_acceleration = floatList[12];
            this.pitch_acceleration = floatList[13];
            this.yaw_acceleration = floatList[14];
            this.ground_speed = floatList[15];
            this.true_track = floatList[16];
        }

        public string ConvertData(Byte[] data)
        {
            List<double> doubles = new List<double>();
            List<float> floats = new List<float>();
            string byteString = Utils.ByteArrayToString(data);

            //VALUES FOR AIRCRAFT Kinematics DATA
            int[] bytelengthList = { 16, 16, 16, 16, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8 };
            int[] offsetList = { 0, 8, 16, 24, 32, 36, 40, 44, 48, 52, 56, 60, 64, 68, 72, 76, 80, 84, 88, 92, 96 };

            for (var i = 0; i < bytelengthList.Length; i++)
            {
                var hexString = byteString.Substring(8 + offsetList[i] * 2, bytelengthList[i]);
                //Console.WriteLine(hexString.Length);
                if (hexString.Length > 8)
                {
                    long hexnumber = Convert.ToInt64(hexString, 16);
                    var res = Utils.LongBitsToDouble(hexnumber);
                    doubles.Add(res);
                }
                else if (hexString.Length > 4)
                {
                    int hexnumber = Convert.ToInt32(hexString, 16);
                    var res = Utils.BitsToFloat(hexnumber);
                    floats.Add(res);
                }

            }
            AircraftKinematicsData result = new AircraftKinematicsData(doubles, floats);
            string json_send = result.ToString();

            var json = new { Content = json_send, type = "aircraft" };
            return JsonConvert.SerializeObject(json);
        }

        public string checkSimulationRunning(Byte[] data)
        {
            byte running = data[4];
            string binary = Convert.ToString(running, 2).PadLeft(8, '0');
            var bit = (running >> 0) & 1;
            bool Isrunning = bit == 0 ? false : true;

            var json = new { Content = Isrunning, type = "start" };

            return JsonConvert.SerializeObject(json);
        }

        public override string ToString()
        {
            string json = JsonConvert.SerializeObject(this);
            return json;
        }

    }
}
