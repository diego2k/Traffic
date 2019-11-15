using System;

namespace traffic.server.Data
{
    class AircraftControlsData
    {
        private const int HEADER_SIZE = 4;

        public bool APDisengageButton { get; set; }

        public AircraftControlsData(byte[] data)
        {
            Buffer.BlockCopy(data, HEADER_SIZE, data, 0, data.Length - HEADER_SIZE);

            byte[] buttons = new byte[1];

            Buffer.BlockCopy(data, 14, buttons, 0, 1);

            APDisengageButton = (buttons[0] & 2) >= 1;
        }
    }
}
