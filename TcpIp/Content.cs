﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
//3_Command Messages : 14 pages
//3_1_1 TABLE 1 (Command Available) : 16- 18 page
namespace IngenicoTestTCP.TcpIp
{
    static internal class Content
    {
        public static byte[] set_stringTobyte(string value_a, int requestLength_a)
        {
            if (value_a.Length != requestLength_a)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < requestLength_a; i++)
                {
                    sb.Append("0");
                }
                //sb.Append(str);
                string val = sb.ToString();
              //  return Encoding.Default.GetBytes(val);
            }
            byte[] _result = Encoding.Default.GetBytes(value_a);            
            return _result;
        }
        public static byte STX = (byte)'\x02';
        public static byte ETX = (byte)'\x03';
        public static string FS = ((char)0x1c).ToString();
        public static byte ANY = 0x01;
        public static byte LRC = 0x7f;
        public static byte ACK = 0x06;
        public static byte LRC_ACK = 0x7A;
        public static byte NACK = 0x15;
        public static byte LRC_NACK = 0x69;
        public static byte STATUS = 0x73;
        public static byte CARD_STATUS = 0x47;
        public static byte ACTIVATE = 0x61; // 'a'  41 page
        public static byte DEACTIVATE = 0x64; // 'd'
        public static byte CANCEL = 0x58; // X
        public static byte RESTART = 0x7A; // z
        public static byte CLOSE_SESSION = 0x43; // C
        public static byte ADVANCED_TERMINAL_CONFIGURATION = 0x41; // A
        public static byte ACQUIRER_INFORMATION = 0x65; // e
        public static byte ACQUIRER_TOTAL_AMOUNTS = 0x6C; // l
        public static byte CONFIRM = 0x49; // I in
        public static byte RE_LAST_PAYMENT = 0x48; // H
        public static byte PAYMENT = 0x50; // P
        public static byte PAYMENTTAG = 0x4D;// M KESOBB
        public static byte REVERSAL = 0x53; // S
        public static byte SETID = 0x6E; // n
public static byte TRANEND = 0x45; // n
        

        // Activate Asynchronous Messages, 12. byte
        /*
         * “1” (0x31) request the EFT/POS to send asynchronous messages (see Chap. 4); this
                function can be activated only for the operations
                “P” (payment), “S” (reversal) and “C” (Close Session)
           “0” (0x30) the PC don’t ask for asynchronous messages; this is the normal situation
         */
        public static byte ACTIVATEASYNC = 0x30;
        /*
         * Operation type, 13. byte
         Codes for “P” command:
         “0” (0x30) purchase transaction
         “1” (0x31) pre-authorization for fuel service
         “2” (0x32) pre-authorization for other services
         “3” (0x33) notification of transaction with final amount for fuel service
         “4” (0x34) notification of transaction with final amount for other services
         “5” (0x35) notification of transaction with final amount for other services without card presentation (needed HOST customization) (see 3_2)
        “6” (0x36) notification of transaction with final amount for fuel service without card presentation in silent mode (i.e. without
            any display on the terminal)
        “8” (0x38) purchase transaction with card in.
        The following values are valid only for “P”, “M”, “S” and “w” commands.
         */
        public static byte OPERATIONTYPE = 0x30;
        /* Transaction identifier,  
         *  This identifier is used to identify a complete
            operation of split payment (pre-authorization + notification).
            The PC has to specify this identifier in the preauthorization requests (“P”,”M” with type=”1” or ”2”), 
            in the notification requests (“P” with type =”3” or “4” or “6”) and in the reversal requests (“S” with type = “2”).
            The PC shall store the identifier used in the preauthorization request and use the same identifier when will send the notification requests or the
            pre-authorization reversal requests.
            The identifier is coded on 2 digit, in ASCII: example “05” (0x30 0x35) with the Most
            significant digit is in position 14 and the less significant digit is in position 15 .
            Valid values are from “01” to “99”
         */
        public static byte[] TRANS_ID = new byte[] { 0x30, 0x30 };
        /* Confirmation mode, 14-15. byte
           “0” STANDARD: “I” message only for chip or mag cards.
           “1” FORCED: “I” message for each card type.
           “2” DISABLED: “I” message never transmitted 
         */
        public static byte CONFIRMATION_MODE = 0x30;
        /* Preauth code  28. byte, 16 byte
           This field is used only for notification operation for other vending machines ( “P”,”M” command
           with type = “4”) . If not used it has to be filled with “0”.
            For backword compatibility the field can be missing (deprecated)
         */
        public static byte[]  PREAUTH = new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30,
                                                         0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30};
        //Terminal Identifier, 25. byte
     //   public static byte[] ID = new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };

        // Ativate
        public static byte[] ACTIVATE1 = new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 
                                                      0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };
        // RFU8
        public static byte[] RFU8 = new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };
        // RFU16
        public static byte[] RFU16 = new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30,
                                                  0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30};
        // Ticket number
        public static byte[] TICKET_NUMBER = new byte[] { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };

        /*Operation Type, Setup/Switch Terminal Id, config
         * 12. byte, 90. page
            “0” Config
            “1” Config by Slot
            “2” Delete (only configuration, not transactions)
            “3” Switch to TermId
            “9” Reset the whole configuration,filled with blanks for Operation Type “9”.
        */
        public static byte ID_OPERATIONTYPE = 0x30;
        /*public static byte[] set_HostCode(string host_a)
        {
            if (host_a.Count() != 20)
            {
                return Encoding.Default.GetBytes("0000000000000000000000000000000000000000");
            }
            byte[] _host = Encoding.Default.GetBytes(host_a);
           // byte[] _amuont = Utils.StringToByteArray(host_a);
            return _host;
        }*/
        /* Setup Options, configs
           92 page
            Customer Customization: 12. byte
            “*” Don’t Care
            “0” No customization*/
        public static byte CUSTOMERCUST = 0x30;
        /* PAN Truncation: 13. byte
            “*” Don’t Care
            “0” Type 0 
            “1” Type 1
            “2” Type 2*/
        public static byte PANTRUNC = 0x32;
        /* Technical Parameter Ignore:15. byte,
            “*” Don’t Care
            “0” NO
            “1” YES
         */
        public static byte TECHNICALPAR = 0x30;
        /* Cless Parameters:16. byte,
             “*” Don’t Care
            “0” DEFAULT
            “O” DEFAULT OFFLINE
         */
        public static byte CLESSPAR = 0x30;
        /* Language : 18-19. byte
         “**” Don’t Care 
             “IT”, “EN”, “DE”, "HU"…
         */
        public static byte[] LANGUAGE = { 0x48, 0x55 };
        /* Card Holder Language : 20. byte
            “*” Don’t Care
            “0” NO
            “1” YES
         */
        public static byte CARDHOLDERLANG = 0x30;
        /* 1^: 22. byte
            “*” Don’t Care
            “0” L only cless
            “1” L always
         */
        public static byte FIELD22 = 0x30;
        public static byte SETUPOPTIONS = 0x6A; //j
        public static byte READCARD = 0x71;
        public static byte FIRSTDLL = 0x4C;// L
        public static byte SETUPETHERNET = 0x68; // h 
        /* Setup Ethernet Parameters, 90. pages
         DHCP , Field 12.
        “0” NO
        “1” YES
         * */
        public static byte SETUPETH_DHCP = 0x31;

        //Config: Setup Line Parameters

        //Read Line Parameters request

        // async reseive part
        public static byte ASYNC_START = 0x4D; // M
        public static byte[] FINETRANSAZIONE = { 0x30, 0x34 }; // 14
        public static byte FINANCIALEND = 45; // E

    }
}

/* 16. page
 * Commands code.
 * ! is ready
    “P” 50 : Payment, PROBALNI KELL
    "M” 4D : Payment with additional tag, KESOBB
!   “X” 58 : Cansel
-   “V” 56 : Get EMV Transaction Data. Deprecated.
    “S” 53 : Reversal 
    “Q” 51 : Bank Totals
    “T” 54 : Local Totals
!   “C” 43 : Close Session
    “D” 44 : DLL
    “L” 4C : First Dll 
-   “U” 55 : LOG Download
!   “a” 61 : Activate EFT/POS
!   “d” 64 : Deactivate EFT/POS
!   “s” 73 : Get EFT/POS status
!   ”z” 7A : Restart
-   “y” 79 : Start Software Maintenance
!   “G” 36 : Get Card status
-   “r” 72 : Reset Log
-   “c” 63 : Get Terminal Configuration. Deprecated.
!   “A” 41 : Get Advanced Terminal Configuration.
!   “e” 65 : Get Acquirer Information.
!   “l” 6C : Get Acquirer Total Amounts 
-   “p” 70 : Sleep Mode 
-   “w” 77 : Sleep Mode (Extended)
!   "I" 49 : Transaction Confirm
!   “H” 48 : Retrieving Last Payment Result
    "n" 6E : Config: Setup/Switch Terminal Id, PROBALNI KELL
!   "j" 6A : Config: Setup Options
    "i" 69 : Config: Setup Line Parameters, KESOBB
    "h" 68 : Config: Setup Ethernet Parameters, PROBALNI KELL
    “%” 25 : Read Line Parameters request
!   "q" 71 : Read Card Command message
   async reseive part: 53. page
    "M" 4D  : start.
    Type Message Code: 2. field 2 byte
    14 : End of transaction
    “E” 45 :  Financial Transaction End Response message
 */
