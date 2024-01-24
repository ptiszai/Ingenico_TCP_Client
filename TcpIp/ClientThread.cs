using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//&& ||
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
        TimeSpan waitTime = new TimeSpan(0, 0, 0, 0, 500);
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
                    string _mess = Utils.ByteArrayToHexOrAsciiString(cmd.Data, 0, cmd.Data.Length, Ingenico.Skeleton!.ASCII);
                    //Ingenico.Skeleton!.TcpException_func($"<-- OUT: count:{cmd.Data.Length} data:{_mess};{cmd.Description}", Color.Black);
                    MesageToIngenico!(StatusEnum.CLIENT_IMFO_1, $"<-- OUT: count:{cmd.Data.Length} data:{_mess};{cmd.Description}");
                }
                else
                {
                    MesageToIngenico!(StatusEnum.CLIENT_CONNECT_1, $"ERROR: POS ConnectAsync 1");
                  //  Ingenico.Skeleton!.TcpException_func($"ERROR: POS not connected", Color.Red);
                }
            }
            catch (Exception exp)
            {
                MesageToIngenico!(StatusEnum.CLIENT_CONNECT_2, $"ERROR: POS ConnectAsync 2: {exp.Message}");
                //Ingenico.Skeleton!.TcpException_func($"ERROR: POS ConnectAsync(): {ex.Message}", Color.Red);
            }
            return connected;
        }

        #endregion
        #region private functions  
        //-----------------------
        public async Task SendAndWaitForResponse(Command cmd_a)
        {
            try
            {             
                using (var client = new TcpClient())
                {
                    await ConnectAndSend(client, cmd_a);
                    // }
                    if (connected)
                    {
                        var stream = client.GetStream();
                        byte[] byteBuffer = new byte[512];
                        var charBuffer = new char[512];

                        int bytesRead = 0;
                        // Decoder is useful in properly handling multi-byte character encodings - it will only emit "complete" characters, so we're not going to
                        // mangle multi-byte characters by accident. Do not use something like StreamReader - it would work for our exact scenario, but it breaks down
                        // on more complicated streams.
                        var decoder = UTF8Encoding.UTF8.GetDecoder();
                        // var charBuffer = new char[512];
                        var data = cmd_a;
                        //int _bytesread = 0;
                        byte[] _bytebuffer = new byte[4];

                        /*while ((bytesRead =  stream.Read(buffer, 0, 29)) != 0)
                        {
                            var charsRead = decoder.GetChars(buffer, 0, bytesRead, charBuffer, 0);

                            Command cmd = cmds.CommandList[buffer[0]];
                            var rdescription = cmd.Description;
                            writeTotbxLog($"-->    IN: count:{bytesRead} data: {Utils.ByteArray_Hex_ASCII_ToString(buffer, bytesRead)};{rdescription} ", Color.DarkGreen);
                        }*/



                        //Task timeoutTask = Task.Delay(CONNECTION_TIMEOUT_RECEIVE);
                        //&& ||
                        /*   if (await Task.WhenAny(timeoutTask).ConfigureAwait(false) == timeoutTask && ((bytesRead = await stream.ReadAsync(byteBuffer, 0, byteBuffer.Length)) > 0))                       
                           {
                               if (bytesRead == 0) 
                               {
                                   stream.Flush();
                                   Ingenico.Skeleton!.TcpException_func($"-->   IN: empty", Color.DarkGreen);
                                   return;
                               }
                               var charsRead = decoder.GetChars(byteBuffer, 0, bytesRead, charBuffer, 0);
                               string _mess = Utils.ByteArrayToHexOrAsciiString(byteBuffer, 0, bytesRead, Ingenico.Skeleton!.ASCII);        
                               Ingenico.Skeleton!.TcpException_func($"-->   IN: count:{bytesRead} data: {_mess}", Color.DarkGreen);
                           }*/
                        TimeSpan timeout = default;
                        timeout = TimeSpan.FromSeconds(40);
                        var cts = new CancellationTokenSource(timeout); //C# 8 syntax
                        var buffer = new byte[511];
                        int receivedCount;
                        using (cts.Token.Register(() => stream.Close()))
                        {
                            //int receivedCount;
                            try
                            {
                                //var buffer = new byte[256];
                                receivedCount = await stream.ReadAsync(buffer, 0, 511, cts.Token).ConfigureAwait(false);
                                if (receivedCount == 3 && mclient.Connected)
                                {
                                   // if (receivedCount > 0)
                                   // {
                                        for (int ii = 0; ii < receivedCount; ii++)
                                        {
                                            byteBuffer[ii + bytesRead] = buffer[ii];
                                        }
                                        bytesRead += receivedCount;
                                   // }
                                    bytesRead = receivedCount;
                                   // timeout = TimeSpan.FromSeconds(2);
                                    //var cts1 = new CancellationTokenSource(timeout);
                                    receivedCount = await stream.ReadAsync(buffer, 0, 511, cts.Token).ConfigureAwait(false);
                                    if (receivedCount > 0)
                                    {                                        
                                        for (int ii = 0; ii < receivedCount; ii++)
                                        {
                                            byteBuffer[ii+ bytesRead] = buffer[ii];
                                        }
                                        bytesRead += receivedCount;
                                    }
                                }
                           //     return;
                            }
                            catch (TimeoutException exp)
                            {
                                receivedCount = -1;
                                MesageToIngenico!(StatusEnum.CLIENT_SEND_AND_WAIT1, $"ERROR: SendAndWaitForResponse 1: {exp.Message}");
                                return;
                            }
                        }
                     /*   if (!mclient.Connected)
                        {
                            return;
                        }*/
                        //await Read(stream);
                        string _sdescription = "ACK";
                        byte[] _sdata = new byte[0];
                        Commands _cmds = Ingenico.Skeleton!.cmds;
                        if (bytesRead == 0)
                        {
                            _sdescription = "NACK";
                            _sdata = _cmds.NACK();
                        }
                        else
                        if (bytesRead == 3)
                        {
                            if (byteBuffer[0] == Content.ACK)
                            {
                                _sdescription = "ACK";
                                _sdata = _cmds.ACK();
                            }
                            else
                            if (byteBuffer[0] == Content.NACK)
                            {
                                _sdescription = "NACK";
                                _sdata = _cmds.NACK();
                            }
                        }
                        else
                        if (bytesRead >= 13) 
                        {
                            if (byteBuffer[3] == Content.ASYNC_START)
                            { // async reseive message                                
                                string _st = Utils.ByteArrayToString(byteBuffer!, 4, 2);
                                MesageToIngenico!(StatusEnum.CLIENT_IMFO_6, $"Datas: {_st}");
                                /*int _index=Int32.Parse(_st);
                                string _type = AsyncMessages.Messages[_index-1];
                                string _messag = UtilsPost.ByteArrayToString(byteBuffer!, 6, 20);
                                Ingenico.Skeleton!.TcpException_func($"Type:{_type}", Color.Blue);                                
                                Ingenico.Skeleton!.TcpException_func($"Message: {_messag}", Color.Blue);
                                string _datas = UtilsPost.ByteArrayToString(byteBuffer!, 4, bytesRead-6);
                                Ingenico.Skeleton!.TcpException_func($"Datas: {_datas}", Color.Brown);*/
                                return;
                            }
                            else
                            {
                                if ((byteBuffer[0] == Content.ACK) && (byteBuffer[1] == Content.ETX) && (byteBuffer[2] == Content.LRC_ACK))
                                {
                                    byte _crc = Utils.GetLRC(byteBuffer, bytesRead - 1, 0);
                              //      byte _crc = Utils.GetLRC(byteBuffer, bytesRead - 3, 4);
                                    byte _rcrc = byteBuffer[bytesRead - 1];
                                    if (_rcrc == _crc)
                                    {
                                        _sdescription = "ACK";
                                        _sdata = _cmds.ACK();
                                    }
                                    else
                                    {
                                        _sdescription = "NACK";
                                        _sdata = _cmds.NACK();
                                    }
                                    string _st = Utils.ByteArrayToString(byteBuffer!, 0, bytesRead);
                                    Ingenico.Skeleton!.TcpException_func($"Datas: {_st}", Color.Red);
                                    //MesageToIngenico!(StatusEnum.CLIENT_PAYMENT_RESULT, $"Payment: card:{_messag1},amount:{_messag2},date and time:{_messag3}\ncard name:{_messag4},message:{_messag5}\n");
                                    if (byteBuffer[13] == Content.CARD_STATUS)
                                    {

                                        //Ingenico.Skeleton!.TcpException_func($"Status: POS terminal connected", Color.Green);
                                        MesageToIngenico!(StatusEnum.CLIENT_IMFO_3, $"POS terminal connected");
                                    }                                
                                    else
                                    if (byteBuffer[13] == Content.TRANEND)
                                    {
                                        string _messag1 = Utils.ByteArrayToString(byteBuffer!, 27, 3); // Technology used :
                                        string _messag2 = Utils.ByteArrayToString(byteBuffer!, 43, 8); // Amount
                                        string _messag3 = Utils.ByteArrayToString(byteBuffer!, 51, 12); // Transaction data and time
                                        string _messag4 = Utils.ByteArrayToString(byteBuffer!, 64, 16); // Acquirer Name
                                        string _messag5 = Utils.ByteArrayToString(byteBuffer!, 117, 120); // Message for POS 
                                                                                                              //string _mess = UtilsPost.ByteArrayToHexOrAsciiString(byteBuffer!, 0, bytesRead, Ingenico.Skeleton!.ASCII);
                                                                                                              //Ingenico.Skeleton!.TcpException_func($"Datas: {_mess}", Color.Red);
                                        MesageToIngenico!(StatusEnum.CLIENT_PAYMENT_RESULT, $"Payment result: card:{_messag1},amount:{_messag2},date and time:{_messag3} card name:{_messag4}\nmessage:{_messag5}");
                                       // MesageToIngenico!(StatusEnum.CLIENT_PAYMENT_RESULT, $"Payment result:");
                                       // string _datas = UtilsPost.ByteArrayToString(byteBuffer!, 4, bytesRead - 6);
                                       // Ingenico.Skeleton!.TcpException_func($"Datas: {_datas}", Color.Red);
                                    }
                                /*if (byteBuffer[13] == Content.FIRSTDLL)
                                {
                                    string _datas = Utils.ByteArrayToString(byteBuffer!, 4, bytesRead - 6);
                                    Ingenico.Skeleton!.TcpException_func($"Datas: {_datas}", Color.Red);
                                } else
                                if (byteBuffer[13] == Content.CLOSE_SESSION)
                                {

*/

                            }
                                else
                                if (byteBuffer[13] == Content.FIRSTDLL)
                                {

                                }
                                
                               
                                //if (byteBuffer[13] == Content.ACK)

                            }
                            //Ingenico.Skeleton!.TcpException_func($"ERROR: not connected", Color.Red);

                            //  }
                        }
                        await stream.WriteAsync(_sdata, 0, _sdata.Length);
                        string _message = Utils.ByteArrayToHexOrAsciiString(_sdata, 0, _sdata.Length, Ingenico.Skeleton!.ASCII);
                        MesageToIngenico!(StatusEnum.CLIENT_IMFO_5, $"<-- OUT: count:{_sdata.Length}:data: {_message}, {_sdescription}");
                        //Ingenico.Skeleton!.TcpException_func($"<-- OUT: count:{_sdata.Length}:data: {_message}, {_sdescription}", Color.Black);
                        if (!_cmds.NextCmd && (_cmds.PaymentListIndex >= 0) && (_cmds.PaymentListIndex < _cmds.PaymentList.Length))
                        {
                            _cmds.PaymentListIndex++;
                            if (_cmds.PaymentListIndex < _cmds.PaymentList.Length)
                            {
                                _cmds.NextCmd = true;
                            }
                            else
                            {
                                _cmds.NextCmd = false;
                                _cmds.PaymentListIndex = -1;
                               // var _temp = new ComponentClass(ComponentEnum.btnPaymentStart, true);                           
                               // VariableCompAction1!.Invoke(_temp);                                
                            }                           
                        }                        
                    }
                }
            }
            catch (Exception exp)
            {
                Ingenico.Skeleton!.cmds.PaymentListIndex = -1;
                MesageToIngenico!(StatusEnum.CLIENT_SEND_AND_WAIT2, $"ERROR: SendAndWaitForResponse 2: {exp.Message}");

                // var _temp = new ComponentClass(ComponentEnum.btnPaymentStart, true);              
                // VariableCompAction1!.Invoke(_temp);                
                // Ingenico.Skeleton!.TcpException_func($"ERROR: SendAndWaitForResponse(): {ex.Message}", Color.Red);
            }
        }

        /*      async ValueTask Read(NetworkStream stream, TimeSpan timeout = default)
              {
                  //if (timeout == default(TimeSpan))
                      timeout = TimeSpan.FromSeconds(1);

                  using var cts = new CancellationTokenSource(timeout); //C# 8 syntax
                  var buffer = new byte[256];
                  int receivedCount;
                  using (cts.Token.Register(() => stream.Close()))
                  {
                      //int receivedCount;
                      try
                      {
                          //var buffer = new byte[256];
                          receivedCount = await stream.ReadAsync(buffer, 0, 256, cts.Token).ConfigureAwait(false);
                          if (receivedCount == 3 &&  mclient.Connected) 
                          {
                              receivedCount = await stream.ReadAsync(buffer, 0, 256, cts.Token).ConfigureAwait(false);
                          }

                      }
                      catch (TimeoutException)
                      {
                          receivedCount = -1;
                      }
                  }
                  if(!mclient.Connected)
                  {
                      return;
                  }    
                  return;
              }*/

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
                   // var _temp = new ComponentClass(ComponentEnum.btnPaymentStart, true);
                   // VariableCompAction1!.Invoke(_temp);
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
