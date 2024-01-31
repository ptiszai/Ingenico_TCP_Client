
namespace IngenicoTestTCP.TcpIp
{
    public enum StatusEnum : int
    {
        NONE = 0,
        // ping
        PING_START = 1,
        PING_RUNNING = 2,
        PING_TIMEOUT_ERROR = 3,
        PING_GENERAL_ERROR = 4,
        PING_SUCCESS = 5,
        PING_FINISHED = 6,
        GENERAL_ERROR = 7,
        PING_WAIT = 8,
        // Ingenico.css
        DHCP_EXECUTOR = 20,
        PAYMENTIN = 21,
        ASSZYNC_ERROR = 22,

        // Client
        CLIENT_DISPOSE = 30,
        CLIENT_CONNECT_1 = 31,
        CLIENT_CONNECT_2 = 32,
        CLIENT_SEND_AND_WAIT1 = 33,
        CLIENT_SEND_AND_WAIT2 = 34,
        CLIENT_EXECUTOR = 35,
        CLIENT_PAYMENT_RESULT = 36,
        CLIENT_PAYMENT_GOOD_END = 37,
        CLIENT_PAYMENT_ERROR_END = 38,
        CLIENT_PROSSEING = 39,       

        CLIENT_IMFO_1 = 40,
        CLIENT_IMFO_2 = 41,
        CLIENT_IMFO_3 = 42,
        CLIENT_IMFO_4 = 43,
        CLIENT_IMFO_5 = 44,
        CLIENT_IMFO_6 = 45,
        CLIENT_UNKNOW = 46,
        CLIENT_GOOD_RECEIVE = 47,

        PAYMENT_ERROR_TYPECODE67 = 60,
        PAYMENT_ERROR_TYPECODE72 = 61,
        PAYMENT_ERROR_TYPECODE74 = 62,
        PAYMENT_ERROR_TYPECODE50 = 63,

        // Server
        SERVER_ERROR_1 = 100,
        SERVER_ERROR_2 = 101,
        SERVER_ERROR_3 = 102,
        SERVER_ERROR_4 = 103,
        SERVER_ERROR_5 = 104,

        SERVER_INFO_1 = 101,
    }
}
