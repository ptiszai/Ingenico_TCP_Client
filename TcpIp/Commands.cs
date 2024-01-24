using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngenicoTestTCP;

//20211117-InterfaceProtocol_iSelf-v3.27.pdf
//4_ACK/NAK : 51 page 
//Prot95salewithinfoMy.ptp
namespace IngenicoTestTCP.TcpIp
{
    public class Command
    {
        public Command(byte id_a, byte[] data_a, string description_a)
        {
            Id = id_a;
            Data = data_a;
            Description = description_a;
        }
        public byte Id { get; set; }
        //public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Data { get; set; }
    }

    public class Commands
    {
        // public string AmountFt = "31323335";
        //public string AmountFt = "0000000000012345";
        // public string AmountFt = "0000000000010000";
        public string AmountFt { get; set; }
        //public string Id = "00000000"; // 8 chars
        public string Id = "01000065"; // 8 chars, 30 31 30 30 30 30 36 35
       // string Host = "192.168.0.1500000000"; // 20 chars
        string Host = "192.168.0.1500000000"; // 20 chars
        //https://www.geeksforgeeks.org/c-sharp-dictionary-with-examples/
        public Dictionary<byte, Command> CommandList = new Dictionary<byte, Command>();
        public byte[] PaymentList = { Content.ACTIVATE, Content.PAYMENT, Content.DEACTIVATE, Content.CLOSE_SESSION };
       // List<byte[]> paymentList = new List<byte[]>(cont);
        //public List<byte> paymentList = { Content.ACTIVATE };
        public int PaymentListIndex = -1;
        public bool NextCmd { get; set; }

        // ACK
        public Commands(string tid_a)
        {
            Id = tid_a;
            AmountFt = "00000000";
            NextCmd = false;

            // any
            Command _cmd = new Command(Content.ANY, ANY(), "\nANY");// 51 page
            // ACK
            _cmd = new Command(Content.ACK, ACK(), "\nACK");// 51 page
            CommandList.Add(Content.ACK, _cmd);
            // NACK
            _cmd = new Command(Content.NACK, NACK(), "\nNACK"); // 51 page
            CommandList.Add(Content.NACK, _cmd);            
            //Activate EFT/POS
            _cmd = new Command(Content.ACTIVATE, ActivateCmd(false), "\nActivate"); // 39 page
            CommandList.Add(Content.ACTIVATE, _cmd);
            //Deactivate EFT/POS
            _cmd = new Command(Content.DEACTIVATE, ActivateCmd(true), "\nDeactivate"); // 39 page
            CommandList.Add(Content.DEACTIVATE, _cmd);
            //Get the current EFT/POS status
            _cmd = new Command(Content.STATUS, StatusCmd(), "\nGet the EFT/POS status");
            CommandList.Add(Content.STATUS, _cmd);
            //Cancel.
            //This message is sent by the PC to cancel a financial transaction already started, when the terminal is waiting
            //for a card. The terminal returns in the idle state(“TERMINALE PRONTO”)
            _cmd = new Command(Content.CANCEL, CancelCmd(), "\nCancel");
            CommandList.Add(Content.CANCEL, _cmd);
            // Restart
            _cmd = new Command(Content.RESTART, RestartCmd(), "\nRestart");
            CommandList.Add(Content.RESTART, _cmd);
            // Get information about card status, if the card is present in the
            // terminal or not;
            _cmd = new Command(Content.CARD_STATUS, CardStatusCmd(), "\nGet card status");
            CommandList.Add(Content.CARD_STATUS, _cmd);
            // Close Session
            _cmd = new Command(Content.CLOSE_SESSION, CardStatusCmd(), "\nClose Session");
            CommandList.Add(Content.CLOSE_SESSION, _cmd);
            // Get Advanced Terminal Configuration
            _cmd = new Command(Content.ADVANCED_TERMINAL_CONFIGURATION, ConfigCmd(), "\nGet Advanced Terminal Configuration");
            CommandList.Add(Content.ADVANCED_TERMINAL_CONFIGURATION, _cmd);
            //Acquirer Information
            _cmd = new Command(Content.ACQUIRER_INFORMATION, AcqInformationCmd(), "\nAcquirer Information");
            CommandList.Add(Content.ACQUIRER_INFORMATION, _cmd);
            //Get Acquirer Total Amounts
            _cmd = new Command(Content.ACQUIRER_TOTAL_AMOUNTS, AcqTotalAmountsCmd(), "\nAcquirer Total Amounts");
            CommandList.Add(Content.ACQUIRER_TOTAL_AMOUNTS, _cmd);
            // Transaction Confirm message.
            // This message is sent by the PC following a request of the transaction amount coming from the EFT/POS
            _cmd = new Command(Content.CONFIRM, ConfirmCmd(), "\nTransaction Confirm");
            CommandList.Add(Content.CONFIRM, _cmd);
            // Payment
            _cmd = new Command(Content.PAYMENT, PaymentCmd(AmountFt), "\nPayment");
            CommandList.Add(Content.PAYMENT, _cmd);
            // Payment with additional tag
            /*_cmd = new Command(Content.PAYMENTTAG, PaymentTagCmd(AmountFt), "\nPayment with additional tag");
            CommandList.Add(Content.PAYMENTTAG, _cmd);*/
            // Retrieving Last Payment Result
            _cmd = new Command(Content.RE_LAST_PAYMENT, ReLastPaymentCmd(), "\nRetrieving Last Payment Result");
            CommandList.Add(Content.RE_LAST_PAYMENT, _cmd);
            // Start a reversal of the last purchase; if successful, the last purchase is cancelled by the bank.
            _cmd = new Command(Content.REVERSAL, ReversalCmd(), "\nReversal last purchase");
            CommandList.Add(Content.REVERSAL, _cmd);
            // Config: Setup/Switch Terminal Id.
            _cmd = new Command(Content.SETID, SetIdCmd(false), "\nConfig: Setup/Switch Terminal Id");
            CommandList.Add(Content.SETID, _cmd);
            _cmd = new Command(Content.SETID, SetIdCmd(true), "\nConfig: Setup/Switch Terminal Id, CLEAR");
            CommandList.Add(0x80, _cmd);
            // Config: Setup Options
            _cmd = new Command(Content.SETUPOPTIONS, SetupOptionsCmd(), "\nConfig: Setup Options-HU");
            CommandList.Add(Content.SETUPOPTIONS, _cmd);
            // Config: Setup Ethernet Parameters.
            _cmd = new Command(Content.SETUPETHERNET, SetupEthernetCmd(), "\nConfig: Setup Ethernet Parameters");
            CommandList.Add(Content.SETUPETHERNET, _cmd);
            // Read Card Command message
            _cmd = new Command(Content.READCARD, ReadCardCmd(), "\nRead Card Command message");
            CommandList.Add(Content.READCARD, _cmd);
            // First Dll
            _cmd = new Command(Content.FIRSTDLL, FirstDllCmd(), "\nFirst Dll");
            CommandList.Add(Content.FIRSTDLL, _cmd);

        }

        public byte[] ANY()
        {
            byte[] _aa = Content.set_stringTobyte(Id, 8);
            return _aa;
        }


        public byte[] StartCommand(byte cmd_a)
        {
            byte[] _a1 = { Content.STX };
            byte[] _a2 = Content.set_stringTobyte(Id, 8);//Content.ID;
            byte[] _a3 = { 0x30, cmd_a };                     
            byte[] _as = new byte[_a1.Length + _a2.Length + _a3.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            Buffer.BlockCopy(_a3, 0, _as, _a1.Length + _a2.Length, _a3.Length);
          //  _as[10] = cmd_a;
            return _as;
        }
        // respond
        public byte[] ACK()
        {
            byte[] _cmd = {Content.ACK,Content.ETX,Content.LRC_ACK};
            return _cmd;
        }
        public byte[] NACK()
        {
            byte[] _cmd = { Content.NACK, Content.ETX, Content.LRC_NACK };
            return _cmd;
        }

        public byte[] ActivateCmd(bool deactivate_a)
        {
            byte _cmd = Content.ACTIVATE;
            if (deactivate_a)
            {
                _cmd = Content.DEACTIVATE;
            }
            byte[] _a1 = StartCommand(_cmd);
            byte[] _a2 = Content.ACTIVATE1;
            byte[] _a3 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length + _a3.Length];            
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);           
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            Buffer.BlockCopy(_a3, 0, _as, _a1.Length + _a2.Length, _a3.Length);            
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length-1] = crc;//crc;
            return _as;
        }
      
        public byte[] StatusCmd()
        {
            //https://stackoverflow.com/questions/415291/best-way-to-combine-two-or-more-byte-arrays-in-c-sharp

            byte[] _a1 = StartCommand(Content.STATUS);                      
            byte[] _a2 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);           
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length-1] = crc;            
            return _as;
        }

        public byte[] CardStatusCmd()
        {            
            byte[] _a1 = StartCommand(Content.CARD_STATUS);
            byte[] _a2 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);            
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }

        public byte[] CloseSessionCmd()
        {
            byte[] _a1 = StartCommand(Content.CLOSE_SESSION);
            byte[] _a2 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }

        public byte[] CancelCmd()
        {
            byte[] _a1 = StartCommand(Content.CANCEL);           
            byte[] _a2 = {Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);            
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }

        public byte[] RestartCmd()
        {
            byte[] _a1 = StartCommand(Content.RESTART);
            byte[] _a2 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);            
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }

        public byte[] ConfigCmd()
        {
            byte[] _a1 = StartCommand(Content.ADVANCED_TERMINAL_CONFIGURATION);
            byte[] _a2 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }
        
        public byte[] AcqInformationCmd()
        {
            byte[] _a1 = StartCommand(Content.ACQUIRER_INFORMATION);
            byte[] _a2 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }

        public byte[] AcqTotalAmountsCmd()
        {
            byte[] _a1 = StartCommand(Content.ACQUIRER_TOTAL_AMOUNTS);
            byte[] _a2 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }

        // receive 
        public byte[] PaymentCmd(string ftValue_a)
        {
            byte[] _a1 = StartCommand(Content.PAYMENT);            
            byte[] _a2 = { Content.ACTIVATEASYNC, Content.OPERATIONTYPE };
            byte[] _a3 = Content.TRANS_ID;
            byte[] _a4 = Content.set_stringTobyte(ftValue_a, 8);//Content.set_amountTodebit(ftValue_a);
            byte[] _a5 = { 0x30 };
            byte[] _a6 = { Content.CONFIRMATION_MODE, 0x30, 0x2A };
            byte[] _a7 =  Content.PREAUTH;
            byte[] _a8 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length+_a2.Length+_a3.Length+_a4.Length+_a5.Length+_a6.Length+_a7.Length+_a8.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            Buffer.BlockCopy(_a3, 0, _as, _a1.Length+_a2.Length, _a3.Length);
            Buffer.BlockCopy(_a4, 0, _as, _a1.Length+_a2.Length+_a3.Length, _a4.Length);
            Buffer.BlockCopy(_a5, 0, _as, _a1.Length+_a2.Length+_a3.Length+_a4.Length, _a5.Length);
            Buffer.BlockCopy(_a6, 0, _as, _a1.Length+_a2.Length+_a3.Length+_a4.Length+_a5.Length, _a6.Length);
            Buffer.BlockCopy(_a7, 0, _as, _a1.Length+_a2.Length+_a3.Length+_a4.Length+_a5.Length+_a6.Length, _a7.Length);
            Buffer.BlockCopy(_a8, 0, _as, _a1.Length+_a2.Length+_a3.Length+_a4.Length+_a5.Length+_a6.Length+_a7.Length, _a8.Length);
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }

        public byte[] PaymentTagCmd(string ftValue_a)
        {
            byte[] _a1 = StartCommand(Content.PAYMENTTAG);
            return new byte[0];
        }

        public byte[] ReLastPaymentCmd()
        {
            byte[] _a1 = StartCommand(Content.RE_LAST_PAYMENT);
            byte[] _a2 = { 0x01 };
            byte[] _a3 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length + _a3.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            Buffer.BlockCopy(_a3, 0, _as, _a1.Length + _a2.Length, _a3.Length);
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }
        
        public byte[] ConfirmCmd()
        {           
            byte[] _a1 = StartCommand(Content.CONFIRM);            
            byte[] _a2 = Content.RFU8;
            byte[] _a3 = Content.TICKET_NUMBER;
            byte[] _a4 = Content.RFU16;
            byte[] _a5 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            Buffer.BlockCopy(_a3, 0, _as, _a1.Length + _a2.Length, _a3.Length);
            Buffer.BlockCopy(_a4, 0, _as, _a1.Length + _a2.Length + _a3.Length, _a4.Length);
            Buffer.BlockCopy(_a5, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length, _a5.Length);
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }        

        public byte[] ReversalCmd()
        {
            byte[] _a1 = StartCommand(Content.REVERSAL);
           /* byte[] _a2 = { 0x01 };
            byte[] _a3 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length + _a3.Length];
            System.Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            System.Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            System.Buffer.BlockCopy(_a3, 0, _as, _a1.Length + _a2.Length, _a3.Length);
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;*/
            return  new byte[1];
        }
        // config part
        public byte[] SetIdCmd(bool clear)
        {            
            //Content.set_ID(Id);
            Content.set_stringTobyte(Id, 8);
            byte[] _a1 = StartCommand(Content.SETID);
            _a1[9] =  0x30;
            if (clear) 
            {
                Content.ID_OPERATIONTYPE = 0x9;                             
            }            
            byte[] _a2 = { Content.ID_OPERATIONTYPE };
            byte[] _a3 = Content.set_stringTobyte(Id, 8);//Content.set_ID(Id);
            byte[] _a4 = Content.set_stringTobyte(Host, 20);//Content.set_HostCode(Host);
            byte[] _a5 = { 0x30 };
            byte[] _a6 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length + _a6.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            Buffer.BlockCopy(_a3, 0, _as, _a1.Length + _a2.Length, _a3.Length);
            Buffer.BlockCopy(_a4, 0, _as, _a1.Length + _a2.Length + _a3.Length, _a4.Length);
            Buffer.BlockCopy(_a5, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length, _a5.Length);
            Buffer.BlockCopy(_a6, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length, _a6.Length);
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }
        public byte[] SetupOptionsCmd()
        {
            byte[] _a1 = StartCommand(Content.SETUPOPTIONS);
            byte[] _a2 = { Content.CUSTOMERCUST, Content.PANTRUNC, 0x2A, Content.TECHNICALPAR, Content.CLESSPAR, 0x2A };
            byte[] _a3 = Content.LANGUAGE;
            byte[] _a4 = { Content.CARDHOLDERLANG, 0x2A, Content.FIELD22 };
            byte[] _a5 = Content.set_stringTobyte("0", 18);
            byte[] _a6 = Content.set_stringTobyte("0", 42);
            byte[] _a7 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length + _a6.Length + _a7.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            Buffer.BlockCopy(_a3, 0, _as, _a1.Length + _a2.Length, _a3.Length);
            Buffer.BlockCopy(_a4, 0, _as, _a1.Length + _a2.Length + _a3.Length, _a4.Length);
            Buffer.BlockCopy(_a5, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length, _a5.Length);
            Buffer.BlockCopy(_a6, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length, _a6.Length);
            Buffer.BlockCopy(_a7, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length +_a6.Length, _a7.Length);
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }

        public byte[] SetupEthernetCmd()
        {
            byte[] _a1 = StartCommand(Content.SETUPETHERNET);
            byte[] _a2 = { Content.SETUPETH_DHCP }; //DHCP
            byte[] _a3 = Content.set_stringTobyte("0", 15);// IP
            byte[] _a4 = Content.set_stringTobyte("0", 15);// Mask
            byte[] _a5 = Content.set_stringTobyte("0", 15);// Gateway 1 
            byte[] _a6 = Content.set_stringTobyte("0", 15);// Gateway 2 
            byte[] _a7 = Content.set_stringTobyte("0", 15);// DNS 1 
            byte[] _a8 = Content.set_stringTobyte("0", 15);// DNS 2 
            byte[] _a9 = Content.set_stringTobyte("0", 32);// RFU
            byte[] _a10 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length + _a6.Length + _a7.Length + _a8.Length +_a8.Length + _a9.Length + _a10.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            Buffer.BlockCopy(_a3, 0, _as, _a1.Length + _a2.Length, _a3.Length);
            Buffer.BlockCopy(_a4, 0, _as, _a1.Length + _a2.Length + _a3.Length, _a4.Length);
            Buffer.BlockCopy(_a5, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length, _a5.Length);
            Buffer.BlockCopy(_a6, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length, _a6.Length);
            Buffer.BlockCopy(_a7, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length + _a6.Length, _a7.Length);
            Buffer.BlockCopy(_a8, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length + _a6.Length + _a7.Length, _a8.Length);
            Buffer.BlockCopy(_a9, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length + _a6.Length + _a7.Length + _a8.Length, _a9.Length);
            Buffer.BlockCopy(_a10, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length + _a6.Length + _a7.Length + _a8.Length + _a9.Length, _a10.Length);
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }

        public byte[] ReadCardCmd()
        {
            byte[] _a1 = StartCommand(Content.READCARD);
            _a1[9] = 0x36; // english
            byte[] _a2 = { 0x31, /*Activate Asynchronous */  0x30 /* Operation type */};
            byte[] _a3 = Content.set_stringTobyte(" ", 32);
            byte[] _a4 = { 0x31, 0x35, 0x0}; //Card Hunting Time “001” -> “250”: 150
            byte[] _a5 = Content.set_stringTobyte("0", 4);
            byte[] _a6 = Content.set_stringTobyte("0", 12);
            byte[] _a7 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length + _a6.Length + _a7.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            Buffer.BlockCopy(_a3, 0, _as, _a1.Length + _a2.Length, _a3.Length);
            Buffer.BlockCopy(_a4, 0, _as, _a1.Length + _a2.Length + _a3.Length, _a4.Length);
            Buffer.BlockCopy(_a5, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length, _a5.Length);
            Buffer.BlockCopy(_a6, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length, _a6.Length);
            Buffer.BlockCopy(_a7, 0, _as, _a1.Length + _a2.Length + _a3.Length + _a4.Length + _a5.Length + _a6.Length, _a7.Length);
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }
        public byte[] FirstDllCmd()
        {
            byte[] _a1 = StartCommand(Content.FIRSTDLL);
            byte[] _a2 = Content.set_stringTobyte("0", 15);
            byte[] _a3 = { Content.ETX, Content.LRC };
            byte[] _as = new byte[_a1.Length + _a2.Length + _a3.Length];
            Buffer.BlockCopy(_a1, 0, _as, 0, _a1.Length);
            Buffer.BlockCopy(_a2, 0, _as, _a1.Length, _a2.Length);
            Buffer.BlockCopy(_a3, 0, _as, _a1.Length + _a2.Length, _a3.Length);
            byte crc = Utils.GetLRC(_as, _as.Length);
            _as[_as.Length - 1] = crc;
            return _as;
        }
        // response my card:
        /*06 03 7A 
        4D 31 34 45 4E 44 20 54 52 41 4E 53 41 43 54 49 4F 4E 20 20 20 20 20 02 30 30 30 30 30 30 30 30 30 71 39 30 
        20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 
        20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 
        30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 03 16*/


        public bool ReceiveCommand(byte[] byteBuffer_a, int bytesRead_a)
        {
            return true;
        }
    }
}
