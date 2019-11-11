using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using traffic.server.Helper;

namespace traffic.server.Data
{
    class AircraftControlsData
    {
        private const int HEADER_SIZE = 4;

        public bool FDSynchronizeButton { get; set; }
        public bool APDisengageButton { get; set; }

        public AircraftControlsData(byte[] data)
        {
            Buffer.BlockCopy(data, HEADER_SIZE, data, 0, data.Length - HEADER_SIZE);

            byte[] buttons = new byte[1];

            Buffer.BlockCopy(data, 26, buttons, 0, 1);

            FDSynchronizeButton = (buttons[0] & 64) > 1;
            APDisengageButton = (buttons[0] & 128) > 1;
        }
    }
}
