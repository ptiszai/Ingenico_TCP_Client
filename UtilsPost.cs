using System.Text;

namespace IngenicoTestTCP
{
    internal class UtilsPost
    {
        public static string ByteArray_Hex_ASCII_ToString(byte[] data_a, int len_a = 0)
        {
            // https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa
            int _length = (len_a <= 0) ? data_a.Length : len_a;            
            string sTemp0 = BitConverter.ToString(data_a, 0, _length).Replace("-", " ");            
            return sTemp0;
        }

        public static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string ByteArrayToString(byte[] bytes, int byteIndex, int byteCount)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            string _result = encoding.GetString(bytes, byteIndex, byteCount);
            return _result;
        }
        public static string ByteArrayToHexOrAsciiString(byte[] bytes_a, int byteIndex_a, int byteCount_a, bool ascii_a)
        {
            string _message = "";
            if (ascii_a)
            {
                _message = UtilsPost.ByteArrayToString(bytes_a, byteIndex_a, byteCount_a);
            }
            else
            {
                _message = UtilsPost.ByteArray_Hex_ASCII_ToString(bytes_a, byteCount_a);
            }
            return _message;
        }

        public static byte[] StringHexToByteArray(string hex_a)
        {
            if (string.IsNullOrEmpty(hex_a))
            {
                return new byte[0];
            }
            //string[] hexValues = new string[] {"10", "0F", "3E", "42"};
            //List < string> hexValues = new List<string> {"10", "0F", "3E", "42"};
            List<string> hexValues = new List<string>();
            char[] separators = new char[] { ' ', ',', '.' };
            string[] subs = hex_a.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sub in subs)
            {
                hexValues.Add(sub);               
            }

            byte[] result = hexValues
              .Select(value => Convert.ToByte(value, 16))
              .ToArray();
            return result;
        }

        public static string HexToASCIIString(string hexValues)
        {
            //string hexValues = "48 65 6C 6C 6F 20 57 6F 72 6C 64 21";
            // hexValues = "02 30 30 30 30 30 30 30 30 30 69 30 35 36 30 38 38 39 31 2E 32 30 38 2E 32 31 34 2E 31 30 30 30";
            string[] hexValuesSplit = hexValues.Split(' ');
            List<string> _tempList = new List<string>();
            // int ii = 0;
            string temphex = " ";
            foreach (string hex in hexValuesSplit)
            {
                if (string.IsNullOrEmpty(hex))
                {
                    continue;
                }
                temphex = hex;
                if (hex == "00")
                {
                    temphex = "30";
                }
                else
                {
                    temphex = hex;
                }        
                int value = Convert.ToInt32(temphex, 16);                
                string stringValue = Char.ConvertFromUtf32(value);               
                _tempList.Add(stringValue);

            }
            string _result = string.Join("", _tempList.ToArray());
            //Console.Write(_result);
            return _result;
        }

        public static string ASCIIStringToHexString(string Value)
        {
            StringBuilder sb = new StringBuilder();

            byte[] inputByte = Encoding.UTF8.GetBytes(Value);

            foreach (byte b in inputByte)
            {
                sb.Append(string.Format("{0:x2} ", b));
            }

            return sb.ToString();
        }
        /* public static Tuple<byte[], int> SendCommand(byte[] rdata_a, int rlen_a)
         {
             byte[] _sdata = new byte[512];
             //int _slen = 3;
             int _length = (rlen_a <= 0) ? rdata_a.Length : rlen_a;
             _sdata = Commands.ACK();
             int _slen = _sdata.Length;
             return Tuple.Create(_sdata, _slen);
         }*/
        public static byte GetLRC(byte[] message)
        {
            byte LRC = 0;
            foreach (byte a in message)
            {
                LRC ^= a;
            }
            return LRC;
        }
        public static byte GetLRC(byte[] cmd_a, int length_a, int offset_a = 0)
        {
            byte LRC = 0;
            int i = 0;
            byte temp = 0;
            for (i = 0; i < length_a; i++)            
            {
                temp = cmd_a[offset_a + i];
              //  Console.WriteLine($"i, temp:{i}, {temp}");
                LRC ^= temp;                
            }
            return LRC;
        }
    }
}
