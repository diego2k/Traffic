using System;
using traffic.server.Helper;

namespace traffic.server.Data
{
    public class TrafficData
    {
        private const int HEADER_SIZE = 4;

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Altitude { get; set; }
        public double YawAngle { get; set; }
        public double PitchAngle { get; set; }
        public double RollAngle { get; set; }

        public TrafficData(byte[] data)
        {
            Buffer.BlockCopy(data, HEADER_SIZE, data, 0, data.Length - HEADER_SIZE);

            byte[] dataValidity = new byte[1];
            byte[] longitude = new byte[4];
            byte[] latitude = new byte[4];
            byte[] altitude = new byte[4];
            byte[] yawAngle = new byte[4];
            byte[] pitchAngle = new byte[4];
            byte[] rollAngle = new byte[4];
            Buffer.BlockCopy(data, 18, dataValidity, 0, 1);

            if ((dataValidity[0] & 206) != 206)
            {
                throw new Exception("Traffic data invalid!");
            }

            Buffer.BlockCopy(data, 28, longitude, 0, 4);
            Buffer.BlockCopy(data, 32, latitude, 0, 4);
            Buffer.BlockCopy(data, 36, altitude, 0, 4);
            Buffer.BlockCopy(data, 42, yawAngle, 0, 2);
            Buffer.BlockCopy(data, 44, pitchAngle, 0, 2);
            Buffer.BlockCopy(data, 46, rollAngle, 0, 2);

            int longitude_ = Utils.GetLittleEndianIntegerFromByteArray(longitude, 0);
            int latitude_ = Utils.GetLittleEndianIntegerFromByteArray(latitude, 0);
            int altitude_ = Utils.GetLittleEndianIntegerFromByteArray(altitude, 0);
            int yawAngle_ = Utils.GetLittleEndianIntegerFromByteArray(yawAngle, 0);
            int pitchAngle_ = Utils.GetLittleEndianIntegerFromByteArray(pitchAngle, 0);
            int rollAngle_ = Utils.GetLittleEndianIntegerFromByteArray(rollAngle, 0);

            Longitude = (longitude_ / Math.Pow(2, 30)) * (Math.PI / 2);
            Latitude = (latitude_ / Math.Pow(2, 30)) * (Math.PI / 4);
            Altitude = (altitude_ / Math.Pow(2, 9)) * 1;
            YawAngle = (yawAngle_ / Math.Pow(2, 14)) * (Math.PI / 2);
            PitchAngle = (pitchAngle_ / Math.Pow(2, 14)) * (Math.PI / 4);
            RollAngle = (rollAngle_ / Math.Pow(2, 14)) * (Math.PI / 2);
        }

    }
}
