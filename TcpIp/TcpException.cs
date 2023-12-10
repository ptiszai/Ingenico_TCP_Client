using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngenicoTestTCP.TcpIp
{    
    internal class TcpException : Exception
    {
        static public Action<string, Color>? TcpStatusError;
        public TcpException(string msg_a, Color clr_a) : base(msg_a)
        {
            try
            {
                if (TcpStatusError != null)
                {
                    TcpStatusError?.Invoke(msg_a, clr_a);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TcpIp Exception: {ex.Message}");
            }
        }
       /* public static void TcpException_func(string message)
        {
            try
            {
                throw new TcpException(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TcpIp Exception: {ex.Message}");
            }
        }*/
    }
}
