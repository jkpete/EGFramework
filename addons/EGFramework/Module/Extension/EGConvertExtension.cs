using System;
using System.Linq;
using System.Text;

namespace EGFramework {
    //协议规则解析通用方法扩展
    public static class EGConvertExtension
    {
        /// <summary>
        /// Hex string data to byte array，such as a string like "0x00 0xff 0x06"
        /// </summary>
        /// <param name="self">Only include A-F，0-9,hex</param>
        /// <returns></returns>
        public static byte[] ToByteArrayByHex(this string self) {
            int hexLen = self.Length;
            byte[] result;
            if (hexLen % 2 == 1)
            {
                //奇数  
                hexLen++;
                result = new byte[(hexLen / 2)];
                self += "0" ;
            }
            else
            {
                //偶数  
                result = new byte[(hexLen / 2)];
            }
            int j = 0;
            for (int i = 0; i < hexLen; i += 2)
            {
                result[j] = (byte)int.Parse(self.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                j++;
            }
            return result;
        }


        /// <summary>
        /// get string from hex array ,like hex array {0x0a,0x11} => "0x0a 0x11"
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static string ToStringByHex(this byte[] self)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in self)
            {
                sb.Append(b.ToString("X2") + " ");
            }
            string result = sb.ToString().Trim();
            return result;
        }
        public static string ToStringByHex0x(this byte[] self)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in self)
            {
                sb.Append("0x" + b.ToString("X2") + " ");
            }
            string result = sb.ToString().Trim();
            return result;
        }

        /// <summary>
        /// get hex from string ,like string "0x0a 0x11" => {0x0a,0x11}
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static byte[] ToHexByString(this string self)
        {
            string[] hexStrings = self.Split(' ');
            byte[] byteArray = new byte[hexStrings.Length];
            for (int i = 0; i < hexStrings.Length; i++)
            {
                byteArray[i] = Convert.ToByte(hexStrings[i], 16);
            }
            return byteArray;
        }
        public static byte[] ToHexByString0x(this string self)
        {
            if (self.Length <= 2 && self.Substring(0, 2) != "0x") {
                return null;
            }
            return self.ToHexByString();
        }

        public static byte[] ToBytes(this ushort self){
            byte[] byteArray = BitConverter.GetBytes(self);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteArray);
            }
            return byteArray;
        }

        public static ushort ToUShort(this byte[] self){
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(self);
            }
            return BitConverter.ToUInt16(self, 0);
        }

        public static byte[] ToBytes(this uint self){
            byte[] byteArray = BitConverter.GetBytes(self);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteArray);
            }
            return byteArray;
        }

        public static uint ToUINT(this byte[] self){
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(self);
            }
            return BitConverter.ToUInt32(self, 0);
        }

        /// <summary>
        /// convert and resize byte array,such as uint is 0x00FF7799 => byte array {0xFF,0x77,0x99} 
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static byte[] ToBytesAndResizeArray(this uint self){
            byte[] byteArray = BitConverter.GetBytes(self);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteArray);
            }
            int startIndex = Array.FindIndex(byteArray, b => b != 0);
            if (startIndex == -1)
            {
                byteArray = new byte[1];
            }
            else
            {
                byteArray = byteArray.Skip(startIndex).ToArray();
            }
            return byteArray;
        }

        public static byte[] ToByteArray(this bool[] boolArray)
        {
            int numBool = boolArray.Length;
            int numBytes = (numBool + 7) / 8;
            byte[] byteArray = new byte[numBytes];

            for (int i = 0; i < numBool; i++)
            {
                int byteIndex = i / 8;
                int bitIndex = i % 8;
                if (boolArray[i])
                {
                    byteArray[byteIndex] |= (byte)(1 << bitIndex);
                }
            }

            return byteArray;
        }

        public static bool[] ToBoolArray(this byte[] byteArray)
        {
            bool[] boolArray = new bool[byteArray.Length * 8];
            for (int i = 0; i < byteArray.Length; i++)
            {
                byte currentByte = byteArray[i];

                for (int j = 0; j < 8; j++)
                {
                    boolArray[i * 8 + j] = (currentByte & (1 << j)) != 0;
                }
            }

            return boolArray;
        }
        public static bool[] ToBoolArray(this int value)
        {
            string binaryString = Convert.ToString(value, 2); 
            bool[] boolArray = new bool[binaryString.Length];
            if(binaryString.Length < 8){
                boolArray = new bool[8];
            }
            for (int i = 0; i < binaryString.Length; i++)
            {
                boolArray[binaryString.Length - i - 1] = binaryString[i] == '1';
            }
            return boolArray;
        }

        public static int ToInt(this bool[] boolArray)
        {
            int result = 0;
            for (int i = 0; i < boolArray.Length; i++)
            {
                if (boolArray[i])
                {
                    result |= (1 << i);
                }
            }
            return result;
        }

        public static ushort[] ToUShortArray(this byte[] byteArray){
            ushort[] ushortArray = new ushort[byteArray.Length / 2];
            for (int i = 0, j = 0; i < byteArray.Length; i += 2, j++)
            {
                ushortArray[j] = (ushort)((byteArray[i] << 8) | byteArray[i + 1]);
            }
            return ushortArray;
        }

    }
}
