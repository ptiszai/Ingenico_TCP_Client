using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IngenicoTestTCP.TcpIp
{
    internal class ServerTask
    {
        #region public variables
        public Action<StatusEnum, string>? MesageToIngenico;
        #endregion
        #region private variables 
        private TcpListener? listener;
        private CancellationTokenSource? cts;
        private CancellationToken token;
        private int host_port = 1234;
        private string host_ip = "127.0.0.1";
        #endregion
        #region public functions
        public ServerTask(string host_ip_a, int host_port_a)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            host_ip = host_ip_a;
            host_port = 1234;
            Start();
          //  thread = new Thread(executor);
          //  thread.Start();
        }
        public void Start()
        {
            try
            {
                cts = new CancellationTokenSource();
                token = cts.Token;
              //  IPAddress _hostAddr = IPAddress.Parse(host_ip);
                listener = new TcpListener(IPAddress.Any, host_port);
                listener.Start();

                // Note that we're not awaiting here - this is going to return almost immediately. 
                // We're storing the task in a variable to make it explicit that this is not a case of forgotten await :)
                var t = Listen();
            }
            catch (Exception exp)
            {
                MesageToIngenico!(StatusEnum.SERVER_ERROR_1, $"ip bad: {exp.Message}");
            }
        }
        #endregion
        #region private functions  
        private async Task Listen()
        {
            try
            {
                var client = default(TcpClient);
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        client = await listener!.AcceptTcpClientAsync().ConfigureAwait(false);
                    }
                    catch (ObjectDisposedException exp)
                    {
                        // The listener has been stopped.                       
                        MesageToIngenico!(StatusEnum.SERVER_ERROR_1, $"The listener has been stopped: {exp.Message}");
                        return;
                    }

                       if (client == null)
                    {                       
                        MesageToIngenico!(StatusEnum.SERVER_ERROR_2, $"ERROR:Client == null");                       
                        return;
                    }
                    //TcpException_func($"Client READY");
                    // writeTotbxLog($"Client READY", Color.Blue);
                    // Again, there's no await - the Accept handler is going to return immediately so that we can handle the next client.
                    var t = Accept(client);
                }
            }
            catch (Exception exp)
            {
                //TcpException_func($"ERROR: Listen(): {ex.Message}", Color.Red);
                MesageToIngenico!(StatusEnum.SERVER_ERROR_3, $"Listen():  {exp.Message}");
            }
        }
        private async Task Accept(TcpClient client)
        {
            try
            {
                // The using makes sure we're going to dispose of the client. This is very easy thanks to await :)
                using (client)
                {
                    byte[]? byteBuffer = new byte[64];
                    char[]? charBuffer = new char[64];
                    var decoder = UTF8Encoding.UTF8.GetDecoder();
                    var bytesRead = 0;
                    var stream = client.GetStream();

                    // First, we need to know how much data to read. We've got a 4-byte fixed-size header to handle that.
                    // It's unlikely we'd read the header in multiple ReadAsync calls (it's only 4 bytes :)), but it's good practice anyway.
                    var headerRead = 0;
                    while (headerRead < 3 && (bytesRead = await stream.ReadAsync(byteBuffer, headerRead, 3 - headerRead).ConfigureAwait(false)) > 0)
                    {
                        headerRead += bytesRead;
                    }

                    if (headerRead < 3)
                    {                     
                        MesageToIngenico!(StatusEnum.SERVER_ERROR_4, $"We failed to read the header {headerRead} bytes.");
                        return; // We failed to read the header.
                    }

                    var bytesRemaining = BitConverter.ToInt32(byteBuffer, 0);
                    //    Console.WriteLine($"Receiving {bytesRemaining} bytes.");
                    //writeTotbxLog($"Receiving {bytesRemaining} bytes.", Color.Black);
                    int bytescharsRead = bytesRemaining;

                    // Now we know how much we have to read, so let's read everything and write it back out.
                    while (bytesRemaining > 0 && (bytesRead = await stream.ReadAsync(byteBuffer, 0, byteBuffer.Length)) != 0)
                    {
                        //  var charsRead = decoder.GetChars(byteBuffer, 0, bytesRead, charBuffer, 0);
                        // await stream.WriteAsync(buffer, 0, bytesRead);
                        Console.WriteLine($"Receiving {bytesRead} bytes.");
                        bytesRemaining -= bytesRead;
                    }
                    int charsRead = decoder.GetChars(byteBuffer, 0, bytesRead, charBuffer, 0);
                    //string _rcount = string.Format("{0}", charsRead);
                   // string _sdescription = "ACK";

                    if (bytesRead > 0)
                    //if (!string.IsNullOrEmpty(_rcount))
                    {
                        string data = UtilsPost.ByteArray_Hex_ASCII_ToString(byteBuffer, bytesRead);

                 /*       byte[] _sdata = new byte[0];
                        // int _slen;
                        // Tuple<byte[], int>? scommend;
                        if (byteBuffer[0] == Content.ACK)
                        {
                            // _sdescription = "ACK";
                            _sdata = cmds.ACK();
                        }
                        else
                        if (byteBuffer[0] == Content.NACK)
                        {
                            _sdescription = "NACK";
                            _sdata = cmds.NACK();
                        }
                        else
                        if (byteBuffer[0] == Content.STX)
                        {
                            byte _rcrc = byteBuffer[bytesRead - 1];
                            // byteBuffer[bytesRead - 1] = 0x0;
                            byte _crc = Utils.GetLRC(byteBuffer, bytesRead - 1);
                            if (_rcrc == _crc)
                            {
                                // rdescription = "ACK";
                                _sdata = cmds.ACK();
                            }
                            else
                            {
                                _sdescription = "NACK";
                                _sdata = cmds.NACK();
                            }
                        }
                        else
                        {
                            //   _sdata = Commands.NACK();
                            // _sdata = Commands.
                        }
                     */
                       // writeTotbxLog($"-->    IN: count:{bytesRead}:data: {data}", Color.DarkGreen);
                        MesageToIngenico!(StatusEnum.SERVER_INFO_1, $"-->    IN: count:{bytesRead}:data: {data}");
                        // await stream.WriteAsync(_sdata, 0, _sdata.Length);

                        // writeTotbxLog($"<-- OUT: count:{_sdata.Length}:data: {Utils.ByteArray_Hex_ASCII_ToString(_sdata, _sdata.Length)}, {_sdescription}", Color.Black);
                    }
                    // If ReadAsync returns zero, it means the connection was closed from the other side. If it doesn't, we have to close it ourselves.
                   /* if (bytesRead != 0)
                    {
                        //   client.Close(); // Do a graceful shutdown
                        //  writeTotbxLog($"Client CLOSE", Color.Blue);
                    }*/
                }
            }
            catch (Exception exp)
            {
                //TcpException_func($"ERROR: Accept(): {ex.Message}", Color.Red);
                MesageToIngenico!(StatusEnum.SERVER_ERROR_5, $"Accept(): {exp.Message}");
            }
        }
        public void Stop()
        {
            cts!.Cancel();
            listener!.Stop();
        }
        #endregion
    }
}
