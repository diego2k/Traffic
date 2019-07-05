using System;
using System.Collections.Generic;
using System.Linq;

namespace traffic.server.Helper
{
    public class Utils
    {
        const string formatter = "{0,20}{1,27}";

        public static double ConvertRadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return (degrees);
        }

        public double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public static double LongBitsToDouble(long argument)
        {
            double doubleValue;
            doubleValue = BitConverter.Int64BitsToDouble(argument);
            return doubleValue;
        }

        public static float BitsToFloat(long argument)
        {
            byte[] bytes = BitConverter.GetBytes(argument);
            float myFloat = BitConverter.ToSingle(bytes, 0);
            return myFloat;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static string ConvertHex(String hexString)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = System.Convert.ToChar(decval);
                    ascii += character;
                }
                return ascii;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return string.Empty;
        }

        public static List<int> GetBits(byte data)
        {
            var bits = new List<int>();

            string binarystring = Convert.ToString(data, 2).PadLeft(8, '0');
            string binaryReverse = Reverse(binarystring);
            for (int i = 0; i < binaryReverse.Length - 2; i++)
            {
                var val = binaryReverse[i] == '0' ? 0 : 1;
                bits.Add(val);
            }
            return bits;
        }

        public static string GetStringFromArray(List<bool> answers)
        {
            string str = "";
            foreach (var answer in answers)
            {
                if (answer == true)
                {
                    str += "1";
                }
                else
                {
                    str += "0";
                }
            }
            str += "0";
            str += "0";
            return str;
        }

        public static byte[] GetBitsFromString(string answer)
        {
            byte[] bytearray = new byte[1];
            var bitArray = new System.Collections.BitArray(bytearray);

            var index = 0;
            foreach (char c in answer)
            {
                if (c == '1')
                {
                    bitArray.Set(index, true);
                }
                else
                {
                    bitArray.Set(index, false);
                }
                index++;
            }
            bitArray.Set(6, false);
            bitArray.Set(7, false);
            bitArray.CopyTo(bytearray, 0);

            return bytearray;
        }

        public static byte[] GetBitsfromArray(List<bool> answers)
        {
            byte[] bytearray = new byte[1];
            var bitArray = new System.Collections.BitArray(bytearray);

            var index = 0;
            foreach (var answer in answers)
            {
                if (answer == true)
                {
                    bitArray.Set(index, true);
                }
                else
                {
                    bitArray.Set(index, false);
                }
                index++;
            }
            bitArray.Set(6, false);
            bitArray.Set(7, false);
            bitArray.CopyTo(bytearray, 0);

            return bytearray;
        }

        public static string GetBitString(byte data)
        {
            var bits = new List<int>();
            // var bit =  (data >> 0) & 1;

            string binarystring = Convert.ToString(data, 2).PadLeft(8, '0');
            string binaryReverse = Reverse(binarystring);
            return binaryReverse.Substring(0, 6);
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string LittleEndian(string num)
        {
            int number = Convert.ToInt32(num, 16);
            byte[] bytes = BitConverter.GetBytes(number);
            string retval = "";
            foreach (byte b in bytes)
                retval += b.ToString("X2");
            return retval;
        }

        public static string HexToBinary(string hexValue)
        {
            ulong number = UInt64.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
            byte[] bytes = BitConverter.GetBytes(number);
            string binaryString = string.Empty;
            foreach (byte singleByte in bytes)
            {
                binaryString += Convert.ToString(singleByte, 2);
            }
            return binaryString;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
