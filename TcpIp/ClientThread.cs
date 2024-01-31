using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

//&& ||
//&& ||
//https://stackoverflow.com/questions/12421989/networkstream-readasync-with-a-cancellation-token-never-cancels
namespace IngenicoTestTCP.TcpIp
{
    internal class ClientThread : IDisposable
    {
        #region public variables
        static public Action<string>? TcpError;
        public Action<StatusEnum, string>? MesageToIngenico;        
        #endregion
        #region private variables 
        private Thread? thread;
        private bool connected = false;
        private TcpClient mclient;
        //private Commands cmds;
        //private string host_ip = "127.0.0.1";
        private string host_ip = "192.168.0.16";
        private int host_port = 1234;
        //private int host_port = 1111;
        private int CONNECTION_TIMEOUT_CONNECTION = 3000; // msec
        //private int CONNECTION_TIMEOUT_RECEIVE = 1000; // msec
        private bool running = true;        
        TimeSpan waitTime = new TimeSpan(0, 0, 0, 1, 0);
        private bool isPaymentEndGood = false;
        #endregion
        #region public functions
        //-----------------------
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ClientThread(string host_ip_a, int host_port_a)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            host_ip = host_ip_a;
            host_port = host_port_a;
            thread = new Thread(executor);
            thread.Start();
        }

        //-----------------------
        public void Dispose()
        {
            try
            {
                if (thread!.IsAlive)
                {
                    running = false;
                    if (thread.Join(waitTime+waitTime))
                        Console.WriteLine("Thread has termminated.");
                    else
                        Console.WriteLine("The timeout has elapsed and Thread1 will resume.");
                }
            }
            catch (ThreadAbortException exp)
            {
                MesageToIngenico!(StatusEnum.CLIENT_DISPOSE, $"ERROR: Client Dispose: {exp.Message}");                
            }
        }

        public async Task<bool> ConnectAndSend(TcpClient? client, Command cmd)
        {
            try
            {
                connected = false;
                Task timeoutTask = Task.Delay(CONNECTION_TIMEOUT_CONNECTION);
                Task connectTask = client!.ConnectAsync(host_ip, host_port);
                mclient = client;
                connected = false;                  

                if (await Task.WhenAny(timeoutTask, connectTask).ConfigureAwait(false) != timeoutTask && client.Connected)
                {
                    connected = true;
                    var stream = client.GetStream();
                    await Task.WhenAll( stream.WriteAsync(cmd.Data, 0, cmd.Data.Length)).ConfigureAwait(false);
                    string _mess = UtilsPost.ByteArrayToHexOrAsciiString(cmd.Data, 0, cmd.Data.Length, Ingenico.Skeleton!.ASCII);
                    //Ingenico.Skeleton!.TcpException_func($"<-- OUT: count:{cmd.Data.Length} data:{_mess};{cmd.Description}", Color.Black);
                    MesageToIngenico!(StatusEnum.CLIENT_IMFO_1, $"{cmd.Description}<-- OUT: count:{cmd.Data.Length} data:{_mess};");
                }
                else
                {
                    MesageToIngenico!(StatusEnum.CLIENT_CONNECT_1, $"POS ConnectAsync 1");                  
                }
            }
            catch (Exception exp)
            {
                MesageToIngenico!(StatusEnum.CLIENT_CONNECT_2, $"POS ConnectAsync 2: {exp.Message}");               
            }
            return connected;
        }

        #endregion
        #region private functions  
        //-----------------------
        private async ValueTask<int> Read(byte[] rbuffer, NetworkStream stream, TimeSpan timeout = default)
        {
            if (timeout == default(TimeSpan))
                timeout = TimeSpan.FromSeconds(50);
            //int bytesRead = 0;
            using var cts = new CancellationTokenSource(timeout); //C# 8 syntax
            using (cts.Token.Register(() => stream.Close()))
            {               
                try
                {
                    byte[] buffer = new byte[rbuffer.Length];
                    int bytesRead = 0;
                    int receivedCount;
                    bool exit = false;
                    while ((receivedCount = await stream.ReadAsync(buffer, 0, 1024)) != 0)
                    {
                        int count = receivedCount;
                        for (int ii = 0; ii < count; ii++)
                        {
                            rbuffer[ii + bytesRead] = buffer[ii];
                            if ((receivedCount > 2) && (bytesRead>0))
                            {
                                if (rbuffer[ii + bytesRead] == 3)
                                {
                                    ii++;
                                    rbuffer[ii + bytesRead] = buffer[ii];
                                    exit = true;
                                    break;
                                }
                            }
                        }
                        bytesRead += receivedCount;
                     //   Console.WriteLine($"Receiving {bytesRead} bytes.");
                        if (exit)
                        {
                            break;
                        }
                    }
                  //  int bytesRead = await stream.ReadAsync(buffer, 0, 1024, cts.Token).ConfigureAwait(false);
                    return bytesRead;
                }
                catch (TimeoutException exp)
                {
                    return -1;
                }
            }
        }

        private  int processingAndexecution(Command sendCmd_a, Commands? cmds_a, byte[] receiveBuffer, int receiveBufferLength)
        {         
            int _result = 0;
            try
            {
                if ((receiveBuffer[0] == Content.ACK) && (receiveBuffer[1] == Content.ETX) && (receiveBuffer[2] == Content.LRC_ACK))
                { // ACK
                    _result = 1;
                }
                else
                if ((receiveBuffer[0] == Content.NACK) && (receiveBuffer[1] == Content.ETX) && (receiveBuffer[2] == Content.LRC_NACK))
                { // NACK
                    _result = 0;
                }
                
                if (receiveBufferLength >= 13)
                {
                    int recv = receiveBufferLength;
                    for (int i = 0; i < receiveBufferLength; i++)
                    {
                        if (receiveBuffer[i+3] == 0x03)
                        {
                            recv = i+4;
                            break;
                        }
                    }
                    byte _crc = UtilsPost.GetLRC(receiveBuffer, recv/* - 1*/, 0);
                    byte _rcrc = receiveBuffer[recv];
                    if (_rcrc == _crc)
                    {
                        _result = 1;
                    }
                    else
                    {
                        _result = 0;
                      //  return _result;
                    }
                    string _description = "---";
                    List<int> typeCodes;

                    if (receiveBuffer[3] == Content.ASYNC_START)
                    { // 54. page,  Asynchronous message
                        _result = 3;
                        _description = "Payment:";

                        string _msg = UtilsPost.ByteArrayToHexOrAsciiString(receiveBuffer, 0, recv+1, Ingenico.Skeleton!.ASCII);
                        MesageToIngenico!(StatusEnum.CLIENT_PAYMENT_RESULT, $"{_description}:-->IN:Datas: {_msg}");
                        //Type Message Codes
                        typeCodes = getTypeMessageCodes(receiveBuffer, recv, new List<int>());
                        int _len = typeCodes.Count;
                        if (_len > 0) 
                        {
                         //   _result = 4;
                            foreach (int item in typeCodes)
                            {
                                switch (item)
                                {
                                    case 67:
                                        MesageToIngenico!(StatusEnum.PAYMENT_ERROR_TYPECODE67, $"Stop key pressed");
                                        _result = 4;
                                        break;
                                    case 72:
                                        MesageToIngenico!(StatusEnum.PAYMENT_ERROR_TYPECODE72, $"Emv close context");
                                        _result = 4;
                                        break;
                                    case 74:
                                        MesageToIngenico!(StatusEnum.PAYMENT_ERROR_TYPECODE74, $"Card not managed");
                                        _result = 4;
                                        break;
                                    case 50:
                                        MesageToIngenico!(StatusEnum.PAYMENT_ERROR_TYPECODE50, $"Connection error");
                                        _result = 4;
                                        break;
                                }
                            }
                        }
                        return _result;
                    }
                    else
                    {
                     /*   if (cmds_a!.CommandList[receiveBuffer[13]] != null)
                        {
                             Command _cmd = cmds_a!.CommandList[receiveBuffer[10]];
                            if ( _cmd == null ) 
                            {
                                return 0;
                            }
                            _description = cmds_a!.CommandList[receiveBuffer[13]].Description;
                        }*/
                        string _st = UtilsPost.ByteArrayToHexOrAsciiString(receiveBuffer!, 0, recv+1, Ingenico.Skeleton!.ASCII);
                        MesageToIngenico!(StatusEnum.CLIENT_GOOD_RECEIVE, $"{_description}:-->IN:Datas: {_st}");
                    }
                }
            }
            catch (Exception exp)
            {                               
                MesageToIngenico!(StatusEnum.CLIENT_PROSSEING, $"ProcessingAndexecution: {exp.Message}");
                return 0;
            }
            return _result;
        }

        private List<int> getTypeMessageCodes(byte[] receiveBuffer, int recv, List<int> typeCodeassync)
        {
            //List<int> _result = new List<int>();
            char[] type = new char[3];
            type[2] = (char)0;
            try
            {
                int size = recv-3;
                if (size > 0)
                {
                    for (int i = 0; i < size; i++)
                    {
                        if (receiveBuffer[i + 3] == Content.ASYNC_START)
                        {
                            type[0] = (char)receiveBuffer[i + 4];   
                            type[1] = (char)receiveBuffer[i + 5];
                            if (type[0] < '0' || type[0] > '9' || type[1] < '0' || type[1] > '9')
                            {
                                Console.WriteLine($"bad type: {new string(type)} pos: {i + 3}.");
                                continue;
                            }
                            int _iTemp = Int32.Parse(type);
                            typeCodeassync.Add( _iTemp );
                            Console.WriteLine($"Good type: {_iTemp} pos: {i + 3}.");
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                MesageToIngenico!(StatusEnum.ASSZYNC_ERROR, $"getTypeMessageCodes: {exp.Message}");
                typeCodeassync.Clear();
                return typeCodeassync;
            }

            return typeCodeassync;
        }

        public async Task SendAndWaitForResponse(Command cmd_a)
        {
            try
            {             
                using (var client = new TcpClient())
                {
                    await ConnectAndSend(client, cmd_a);
                                        
                    if (connected)
                    {                        
                        var stream = client.GetStream();
                        byte[] byteBuffer = new byte[2048];               
                        int bytesRead = 0;                      
                        //var _cmd = cmd_a;                                               
                        string _sdescription = "";
                        byte[] _sdata = new byte[0];
                        Commands _cmds = Ingenico.Skeleton!.cmds;

                        bytesRead = await Read(byteBuffer, client.GetStream(), default).ConfigureAwait(false);
                        if (bytesRead > 0)
                        {
                      
                            int _result = processingAndexecution(cmd_a, _cmds, byteBuffer, bytesRead);
                            if (_result == 1)
                            {
                                _sdescription = "ACK";
                                _sdata = _cmds.ACK();
                            }
                            else
                            if (_result == 0)
                            {
                                _sdescription = "NACK";
                                _sdata = _cmds.NACK();
                            }
                            else
                            if (_result == 3)
                            { // payment assync good
                                isPaymentEndGood = true;
                            }
                            else
                            if (_result == 4)
                            { // payment assync wrong
                                isPaymentEndGood = false;
                            }
                            if (_sdata.Length > 0)
                            {
                                await stream.WriteAsync(_sdata, 0, _sdata.Length);
                                string _msg = UtilsPost.ByteArrayToHexOrAsciiString(_sdata, 0, _sdata.Length, Ingenico.Skeleton!.ASCII);
                                MesageToIngenico!(StatusEnum.CLIENT_IMFO_5, $"{_sdescription}<-- OUT: count:{_sdata.Length}:data: {_msg}");
                            }

                            // return;
                        }
                        stream.Close();
                        if (!_cmds.NextCmd && (_cmds.PaymentListIndex >= 0) && (_cmds.PaymentListIndex < _cmds.PaymentList.Length))
                        {
                            _cmds.PaymentListIndex++;
                            if (_cmds.PaymentListIndex < _cmds.PaymentList.Length)
                            {
                                _cmds.NextCmd = true;
                            }
                            else
                            {
                                if (isPaymentEndGood)
                                {
                                    MesageToIngenico!(StatusEnum.CLIENT_PAYMENT_GOOD_END, $"Payment end!");
                                }
                                else
                                {
                                    MesageToIngenico!(StatusEnum.CLIENT_PAYMENT_ERROR_END, $"Payment end!");
                                }
                                _cmds.NextCmd = false;
                                _cmds.PaymentListIndex = -1;                                                                
                            }
                        }
                       // return;                       
                    }
                }
            }
            catch (Exception exp)
            {                
                Ingenico.Skeleton!.cmds.PaymentListIndex = -1;
                MesageToIngenico!(StatusEnum.CLIENT_SEND_AND_WAIT2, $"ERROR: SendAndWaitForResponse 2: {exp.Message}");
            }
        }

        private void executor()
        {
            Commands _cmds = Ingenico.Skeleton!.cmds;
            Command? _cmd;
            byte _cmdIndex = 0x0;
            try
            {
                while (running)
                {
                    if (_cmds.NextCmd && (_cmds.PaymentListIndex >= 0) && (_cmds.PaymentListIndex < _cmds.PaymentList.Length))
                    {
                        _cmdIndex = _cmds.PaymentList[_cmds.PaymentListIndex];
                        _cmd = _cmds.CommandList[_cmdIndex];                            
                        _ = SendAndWaitForResponse(_cmd);
                        _cmds.NextCmd = false;
                    }
                    Thread.Sleep(waitTime);                   
                }
            }
            catch (Exception exp)
            {
                MesageToIngenico!(StatusEnum.CLIENT_EXECUTOR, $"ERROR: executor(): {exp.Message}");
                //Console.WriteLine($"executor: {exp.Message}");
            }            
        }
        #endregion
    }
}
