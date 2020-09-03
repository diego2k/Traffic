using System;

namespace traffic.server.Data
{
    class TUG2SimCenterConsoleOutputData
    {
        private const int HEADER_SIZE = 4;

        public bool GoAroundButton { get; set; }

        public TUG2SimCenterConsoleOutputData(byte[] data)
        {
            Buffer.BlockCopy(data, HEADER_SIZE, data, 0, data.Length - HEADER_SIZE);

            byte[] buttons = new byte[1];

            Buffer.BlockCopy(data, 0, buttons, 0, 1);

            GoAroundButton = (buttons[0] & 16) >= 1;
        }
    }
}
