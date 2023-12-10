using IngenicoTestTCP.TcpIp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IngenicoTestTCP
{
    internal class Utils
    {
        public static string ByteArray_Hex_ASCII_ToString(byte[] data_a, int len_a = 0)
        {
            // https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa
            int _length = (len_a <= 0) ? data_a.Length : len_a;
            var encoding = Encoding.GetEncoding("US-ASCII");
            string sTemp0 = BitConverter.ToString(data_a, 0, _length).Replace("-", " ");
            // var encoding = Encoding.GetEncoding("US-ASCII");
            return sTemp0;
        }
       /* public static string ByteArray_to_ASCII_String(byte[] data_a, int startPos_a, int len_a = 0)
        {
            return ASCIIEncoding.GetString(data_a, startPos_a, len_a);
        }*/
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

                _message = Utils.ByteArrayToString(bytes_a, byteIndex_a, byteCount_a);
            }
            else
            {
                _message = Utils.ByteArray_Hex_ASCII_ToString(bytes_a, byteCount_a);
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
               // Console.WriteLine($"Substring: {sub}");
            }

            byte[] result = hexValues
              .Select(value => Convert.ToByte(value, 16))
              .ToArray();
            return result;
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
                LRC ^= temp;                
            }
            return LRC;
        }
        /*public static byte GetLRC(byte[] cmd_a, int length_a)
        {
        }*/
    }
}
