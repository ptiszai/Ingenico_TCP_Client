using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;


namespace IngenicoTestTCP.TcpIp
{
    public class PingThread
    {
        #region public variables 
        public bool StopThread;
        #endregion

        #region private variables       
        private string ipAddress;
        private Ping? pingSender;
        private int sleepTimeInMsec;
        private StatusEnum state;
        private int repeatedNumber;   
        private bool success = false;
        private bool failed = false;
        #endregion

        #region public eventHandlers      
        public Action<StatusEnum, string>? InfoErrorContent;    
        #endregion

        #region public functions 
        //--------------------------------------------
        public PingThread(string ipAddress_a)
        {
            ipAddress = ipAddress_a.Trim();
            StopThread = false;
            sleepTimeInMsec = 1000;
            state = StatusEnum.PING_START;
            repeatedNumber = 0;
        }

        //--------------------------------------------
        public void SetIp(string ipAddress_a)
        {
            ipAddress = ipAddress_a;
            state = StatusEnum.PING_START;
        }

        //--------------------------------------------
        // This method that will be called when the thread is started
        public void Procedure()
        {
            while (!StopThread)
            {
                switch (state)
                {
                    case StatusEnum.PING_START:
                        {
                            repeatedNumber = 0;
                            ping();
                            state = StatusEnum.PING_RUNNING;
                            break;
                        }
                    case StatusEnum.PING_RUNNING:
                        {
                      //      Console.WriteLine($"PING_RUNNING");
                            break;
                        }
                    case StatusEnum.PING_TIMEOUT_ERROR:
                        {
                            if (!failed)
                            {
                                if (InfoErrorContent != null)
                                {
                                    InfoErrorContent(state, "ping request timed out : " + ipAddress);
                                }
                            }
                            sleepTimeInMsec = 3000;
                           // state = PingStatusEnum.FINISHED;
                            state = StatusEnum.PING_START;
                            success = false;
                            failed = true;
                            break;
                        }
                    case StatusEnum.PING_GENERAL_ERROR:
                        {
                            if (!failed)
                            {
                                if (InfoErrorContent != null)
                                {
                                    InfoErrorContent(state, "ping tunnel:ping failer : " + ipAddress);
                                    success = false;
                                }
                            }
                            sleepTimeInMsec = 3000;
                           // state = PingStatusEnum.FINISHED;
                            state = StatusEnum.PING_START;
                            failed = true;
                            success = false;
                            break;
                        }
                    case StatusEnum.PING_SUCCESS:
                        {
                            if (InfoErrorContent != null)
                            {
                                if (!success)
                                {
                                    InfoErrorContent(state, "Success: " + ipAddress);                                    
                                }
                            }                        
                            sleepTimeInMsec = 3000;
                            state = StatusEnum.PING_START;
                            failed = false;
                            success = true;
                            break;
                        }
                    case StatusEnum.GENERAL_ERROR:
                        {
                            if (!failed)
                            {
                                if (InfoErrorContent != null)
                                {
                                    InfoErrorContent(state, "general failer : " + ipAddress);
                                }
                            }
                            sleepTimeInMsec = 3000;
                            state = StatusEnum.PING_START;
                            failed = true;
                            success = false;
                            break;
                        }
                   /* case PingStatusEnum.FINISHED:
                        {
                          //  StopThread = true;
                            sleepTimeInMsec = 100;                            
                            InfoErrorContent(new InfoErrorEventArgs(EventHandlers.Type.TYPE_INFO, "Finished: " + ipAddress));                         
                            state = PingStatusEnum.WAIT;
                            break;
                        }
                    case PingStatusEnum.WAIT:
                        {                          
                            break;
                        }*/
                }                       
              
                Thread.Sleep(sleepTimeInMsec);
            }
        }
        #endregion

        #region private functions 

        //----------------------------------
        private void ping()
        {
            try
            {
                //pingSender = null;
                pingSender = new Ping();
                AutoResetEvent waiter = new AutoResetEvent(false);
                PingOptions _options = new PingOptions(64, true);
                _options.DontFragment = true;
                pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);
                //  int _repeated = Options.PingRetryCount;
                int _timeout = 2000;
                // int _ii = 1;
                string _data = "0aaaaaaaaa1aaaaaaaaaXaaaaa5aaaab"; // 32 bytes           
                byte[] _buffer = Encoding.ASCII.GetBytes(_data);
                //  Console.WriteLine("Ping start: {0}", ipAddress);
                pingSender.SendAsync(ipAddress, _timeout, _buffer, _options);
            }
            catch (Exception msg)
            {
                 Console.WriteLine($"ERROR:Ping start: {msg}");
            }
        }
     
        #endregion

        #region private events functions
        //---------------------------
        // Ping event
        //---------------------------
        private void PingCompletedCallback(object sender, PingCompletedEventArgs eArgs)
        {
            if (eArgs.Cancelled)
            {
                ping();
                return;
            }
            //Options.PingRetryCount


            if (eArgs.Error != null)
            {
                if (repeatedNumber >= 5)
                {
                    repeatedNumber = 0;
                    state = StatusEnum.PING_GENERAL_ERROR;
                }
                else
                {
                    repeatedNumber++;
                    ping();
                }
                return;
            }

            if (eArgs.Reply.Status == IPStatus.Success)
            {
                repeatedNumber = 0;
                state = StatusEnum.PING_SUCCESS;
                return;
            }

            if (eArgs.Reply.Status == IPStatus.TimedOut)
            {
                if (repeatedNumber >= 5)
                {
                    repeatedNumber = 0;
                    state = StatusEnum.PING_TIMEOUT_ERROR;
                }
                else
                {
                    repeatedNumber++;
                    ping();
                }
                return;
            }
            if (repeatedNumber >= 5)
            {
                repeatedNumber = 0;
                state = StatusEnum.GENERAL_ERROR;
            }
            else
            {
                repeatedNumber++;
                ping();
            }
        }
        #endregion
    }
}
