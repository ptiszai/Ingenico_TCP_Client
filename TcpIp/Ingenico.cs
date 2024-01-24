using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.CodeDom.Compiler;

// Jelszo 0107

namespace IngenicoTestTCP.TcpIp
{
    public class Ingenico : IDisposable
    {
        #region public variables
        public static Ingenico? Skeleton;
        public bool ASCII = true;
        public Commands cmds;     
        public Action<StatusEnum, string>? MesageToMainPage;    
        #endregion
        #region private variables 
        private ClientThread clientThr;                       
        private string host_ip;
        private int host_port;
        private string mac;
        private string tid;
        private bool dhcp;
        private Thread? dhcpThr;
        private PingThread pingThreadObject;
        private Thread pingThread;
        private GetIpOrMacAddressDhcp ipdhcp;
        private bool running = true;
        #endregion
        #region public functions
        public Ingenico(string host_ip_a, int host_port_a, string mac_a, string tid_a, bool dhcp_a = false)
        {
            host_ip = host_ip_a;
            host_port = host_port_a;
            mac = mac_a;
            tid = tid_a;
            dhcp = dhcp_a;
            Skeleton = this;
            ipdhcp = new GetIpOrMacAddressDhcp();
            cmds = new Commands(tid);
            if (dhcp)
            {                              
                dhcpThr = new Thread(dhcp_executor);
                dhcpThr.Start();
            }
            // client part
            clientThr = new ClientThread(host_ip_a, host_port_a);
            clientThr.MesageToIngenico += (state, message) =>
            {
                MesageToMainPage!(state, message);                            
            };         
            // ping part
            pingThreadObject = new PingThread(host_ip);
            pingThreadObject.InfoErrorContent = (state, message) =>   
           // Skeleton!.TcpException_func($"Status: Start POSterminal", Color.Black);
            {                
                switch (state)
                {
                    case StatusEnum.PING_TIMEOUT_ERROR:
                    case StatusEnum.PING_GENERAL_ERROR:
                    case StatusEnum.GENERAL_ERROR:
                    {
                        ipdhcp.DeleteIpFromArpCashe(host_ip);
                        break;
                    }
                    case StatusEnum.PING_SUCCESS:
                    {                            
                        break;
                    }
                };                   
                MesageToMainPage!(state, message);
            };
            pingThread = new Thread(new ThreadStart(pingThreadObject.Procedure));
            pingThread.Start();
        }

        public void Dispose()
        {
            clientThr.Dispose();
            try
            {
                if (pingThread != null)
                {
                    pingThreadObject.StopThread = true;
                    if (pingThread.Join(2000))
                        Console.WriteLine("pingThread:Thread has termminated.");
                    else
                        Console.WriteLine("pingThread:The timeout has elapsed and Thread1 will resume.");                   
                }
                if ((dhcpThr != null) && dhcpThr!.IsAlive)
                {
                    running = false;
                    if (dhcpThr.Join(2000))
                        Console.WriteLine("dhcpThr:Thread has termminated.");
                    else
                        Console.WriteLine("dhcpThr:The timeout has elapsed and Thread1 will resume.");
                }                
            }
            catch (ThreadAbortException exp)
            {
                Console.WriteLine("Dispose error");
            }
        }
        public string GethostIp()
        {
            return host_ip;

        } 
        /*static public string SearchDhcpIp()
        {
            string _result = "";
    
            IPGlobalProperties network = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connections = network.GetActiveTcpConnections();

            foreach (TcpConnectionInformation connection in connections)
            {

            }
            return _result;
        }*/

        public async Task IsPOSTerminal()
        { 
            Command _cmd = cmds.CommandList[Content.CARD_STATUS];        
            await SendAndWaitForResponse(_cmd);
        }

        public async Task Cansel()
        {
            Command _cmd = cmds.CommandList[Content.CANCEL];
            await SendAndWaitForResponse(_cmd);
        }

        public void SetPaymentAmountFt(string amountFt_a)
        {
            try
            {               
                if (string.IsNullOrEmpty(amountFt_a))
                {
                        throw new Exception("Ft amount empty");                   
                }
                int _size = amountFt_a.Length;
                var amountString = "00000000";
                var aStringBuilder = new StringBuilder(amountString);
                aStringBuilder.Remove(amountString.Length - amountFt_a.Length, amountFt_a.Length);
                aStringBuilder.Insert(amountString.Length - amountFt_a.Length, amountFt_a);
                string tempstring = aStringBuilder.ToString();
                string[] _subs = tempstring.Split(' ', '\t', ',', '.');
                string _result = "";
                foreach (string _sub in _subs)
                {
                    _result += _sub;                
                }
                Console.WriteLine($"Pay amount Ft: {_result}");
                cmds.AmountFt = _result;           
                Command _cmd = new Command(Content.PAYMENT, cmds.PaymentCmd(cmds.AmountFt), "\nPayment");
                if (cmds.CommandList.ContainsKey(Content.PAYMENT))
                { // check key before removing it
                    cmds.CommandList.Remove(Content.PAYMENT);
                    cmds.CommandList.Add(Content.PAYMENT, _cmd);
                }           
                cmds.NextCmd = true;
                cmds.PaymentListIndex = 0;
            }
            catch (Exception exp)
            {
               // Console.WriteLine($"TcpIp Exception: {ex.Message}");
                MesageToMainPage!(StatusEnum.PAYMENTIN, exp.Message);
            }
            return;
        }

        public async Task SendAndWaitForResponse(Command cmd_a)
        {
            await clientThr.SendAndWaitForResponse(cmd_a);            
        }

        public  void TcpException_func(string message_a, Color clr_a)
        {
            try
            {
                throw new TcpException(message_a, clr_a);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TcpIp Exception: {ex.Message}");
            }
        }
        #endregion
        #region private functions
        private void dhcp_executor()
        {
            try
            {
                string _ip_address = "0";
                while (running)
                {
                    _ip_address = ipdhcp.getIpByMac(mac);
                    if (_ip_address != "")
                    {
                        if (!_ip_address.Equals(host_ip))
                        {
                            host_ip = _ip_address;                            
                            Console.WriteLine($"MacAddress: {mac}");
                            Console.WriteLine($"IpAddress: {host_ip}");
                            pingThreadObject.SetIp(host_ip);
                        }
                    }                   
                    Thread.Sleep(3000);
                }
            }
            catch (Exception exp)
            {
                MesageToMainPage!(StatusEnum.DHCP_EXECUTOR, exp.Message);
                return;
            }           
        }
        #endregion
    }
}
