using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using IngenicoTestTCP.TcpIp;
using Font = System.Drawing.Font;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Net;
using IniParser.Model;
using IniParser;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

//// Debug.WriteLine(BitConverter.ToString(System.Text.Encoding.Default.GetBytes(message)));  // Write HEX to the debug
namespace IngenicoTestTCP
{
    public partial class FormClient : Form
    {
        private Ingenico? ingenico;       
        Version? ver;
        private bool connected = false;
        private Commands cmds;
        private string ini_file = @"c:\Users\Public\IngenicoTestTCP\ingenicoTestTCP.ini";
        private bool pos_dhcp = false;
        private string pos_ip = "127.0.0.1";
        private string pos_mac = "";
        private int pos_port = 0;
        private string pos_tid = "";
        public FormClient()
        {
            InitializeComponent();

            ver = Assembly.GetExecutingAssembly().GetName().Version;
            FileIniDataParser fileparserINI = new FileIniDataParser();
            IniData dataIni = fileparserINI.ReadFile(ini_file);
            string _value1;
            _value1 = dataIni["General"]["pos_dhcp"];
            pos_dhcp = bool.Parse(_value1);
            _value1 = dataIni["General"]["pos_ip"];
            pos_ip = _value1;
            _value1 = dataIni["General"]["pos_port"];
            pos_port = Int32.Parse(_value1);
            _value1 = dataIni["General"]["pos_mac"];
            pos_mac = _value1;
            _value1 = dataIni["General"]["pos_tid"];
            pos_tid = _value1;

            /*  if (pos_dhcp)
              {
                  GetIpOrMacAddressDhcp _ipdhcp = new GetIpOrMacAddressDhcp();
                  string ip_address = _ipdhcp.getIpByMac(pos_mac);
                  if (ip_address != "") 
                  {
                      btnSend.Enabled = true;
                      btnPaymentStart.Enabled = true;
                  }
              }*/
            cmds = new Commands(pos_tid);
            
            ingenico = new Ingenico(pos_ip, pos_port, pos_mac, pos_tid, pos_dhcp);
            {
                ingenico.MesageToMainPage += (state, message) =>
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        switch (state)
                        {
                            case StatusEnum.PING_TIMEOUT_ERROR:
                            case StatusEnum.PING_GENERAL_ERROR:
                            case StatusEnum.GENERAL_ERROR:
                                {
                                    writeTotbxLog($"ERROR ping: {message}", Color.Red);
                                    goodOrError(false);
                                    break;
                                }
                            case StatusEnum.PING_SUCCESS:
                                {
                                    writeTotbxLog($"Ping: {message}", Color.Green);
                                    goodOrError(true);
                                    break;
                                }
                            case StatusEnum.PAYMENTIN:
                            case StatusEnum.CLIENT_PROSSEING:
                            case StatusEnum.DHCP_EXECUTOR:
                                {
                                    writeTotbxLog($"ERROR:Ingenico: {message}", Color.Red);
                                    break;
                                }
                            case StatusEnum.CLIENT_CONNECT_1:
                            case StatusEnum.CLIENT_CONNECT_2:
                                {
                                    writeTotbxLog($"ERROR:Client: {message}", Color.Red);
                                    break;
                                }
                            case StatusEnum.CLIENT_SEND_AND_WAIT1:
                            case StatusEnum.CLIENT_SEND_AND_WAIT2:
                            case StatusEnum.CLIENT_EXECUTOR:
                                {
                                    writeTotbxLog($"ERROR:Client: {message}", Color.Red);
                                    break;
                                }
                            case StatusEnum.CLIENT_PAYMENT_RESULT:
                                {
                                    writeTotbxLog($"{message}", Color.Brown);
                                    break;
                                }
                            case StatusEnum.CLIENT_PAYMENT_GOOD_END:
                                {
                                    writeTotbxLog($"{message}", Color.Brown);
                                    goodOrError(true);
                                    break;
                                }
                            case StatusEnum.CLIENT_PAYMENT_ERROR_END:
                                {
                                    writeTotbxLog($"ERROR:{message}", Color.Red);
                                    goodOrError(true);
                                    break;
                                }
                            case StatusEnum.CLIENT_IMFO_1:
                            case StatusEnum.CLIENT_IMFO_2:
                            case StatusEnum.CLIENT_IMFO_3:
                            case StatusEnum.CLIENT_IMFO_4:
                            case StatusEnum.CLIENT_IMFO_5:
                            case StatusEnum.CLIENT_IMFO_6:
                                {
                                    writeTotbxLog($"{message}", Color.Blue);
                                    break;
                                }
                            case StatusEnum.CLIENT_GOOD_RECEIVE:
                                {
                                    writeTotbxLog($"{message}", Color.Black);
                                    break;
                                }
                            case StatusEnum.CLIENT_UNKNOW:
                                {
                                    writeTotbxLog($"CLIENT_UNKNOW:  {message}", Color.Black);
                                    break;
                                }
                            case StatusEnum.PAYMENT_ERROR_TYPECODE67:
                            case StatusEnum.PAYMENT_ERROR_TYPECODE72:
                            case StatusEnum.PAYMENT_ERROR_TYPECODE74:
                            case StatusEnum.PAYMENT_ERROR_TYPECODE50:
                                {
                                    writeTotbxLog($"ERROR:  {message}", Color.Red);
                                    break;
                                }

                        }
                    }));
                };
            }
        }

        private void goodOrError(bool state_a)
        {
           // if (state_a)
          //  {
                btnSend.Enabled = true;
                btnSend.BackColor = SystemColors.Control;
                btnPaymentStart.Enabled = true;
                btnPaymentStart.BackColor = SystemColors.Control;
                btnConfig.Enabled = true;
                btnConfig.BackColor = SystemColors.Control;
                //writeTotbxLog(message, Color.Green);
                Text = $"Ingenico TCP Client, version {ver}: {ingenico!.GethostIp()}:{pos_port.ToString()}";
          //  }
           /* else
            {
                btnSend.Enabled = false;
                btnSend.BackColor = SystemColors.ButtonShadow;
                btnPaymentStart.Enabled = false;
                btnPaymentStart.BackColor = SystemColors.ButtonShadow;
                btnConfig.Enabled = false;
                btnConfig.BackColor = SystemColors.ButtonShadow;
                // writeTotbxLog(message, Color.Red);
                Text = $"Ingenico TCP Client, version {ver}: -------:{pos_port.ToString()}";
            }*/
        }


        private void writeTotbxLog(string text, Color fgcolor, bool newline = true)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                tbxLog.SelectionFont = new Font("Verdana", 10, FontStyle.Regular);
                tbxLog.SelectionColor = fgcolor;
                tbxLog.AppendText(text);
                if (newline)
                {
                    tbxLog.AppendText(Environment.NewLine);
                }
            }));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Version? _ver = Assembly.GetExecutingAssembly().GetName().Version;
            Text = $"Adomány Táble Client, version {ver}: {pos_ip}:{pos_port.ToString()}";
            foreach (KeyValuePair<byte, Command> cmdd in cmds.CommandList)
            {
                cmdComboBox.Items.Add(cmdd.Value.Description);
            }
            cmdComboBox.SelectedIndex = 0;
            toolTip1.SetToolTip(txtBoxHexDatas, "Elsö két karakter hexánál 02");
            toolTip1.SetToolTip(BtnCreateCrc, "Hexánál müködik\n02 - ... - 7F");
            /*  ingenico!.VariableCompAction2 += (componentClass) =>
              {
                  if (componentClass != null)
                  {
                      switch (componentClass!.CompValue)
                      {
                          case ComponentEnum.btnPaymentStart:
                              {
                                  this.Invoke(new MethodInvoker(() =>
                                  {
                                      btnPaymentStart.Enabled = true;
                                  }));                               
                                  break;
                              }
                          case ComponentEnum.none:
                              {                                
                                  break;
                              }
                      }
                  }
              };*/

            try
            {
                TcpException.TcpStatusError += (message, color) =>
                {
                    writeTotbxLog(message, color);
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR Exception: {ex.Message}");
                return;
            }
        }
        private async void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
               // btnSend.Enabled = false;
                //Command _cmd = cmds.CommandList[Content.ACK];
                Command _cmd = cmds.CommandList.Values.ElementAt(cmdComboBox.SelectedIndex);
                await ingenico!.SendAndWaitForResponse(_cmd);
            }
            finally
            {
                btnSend.Enabled = true;
            }
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            ingenico!.Dispose();
            Close();
        }

        private void clearBtn_Click(object sender, EventArgs e)
        {
            tbxLog.Clear();
        }

        private void cmdComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Console.WriteLine($"cmdComboBox_SelectedIndexChanged: {cmdComboBox.SelectedIndex}");
            //Command ccc = cmds.CommandList.Values.ElementAt(cmdComboBox.SelectedIndex);
            //writeTotbxLog($"cmdComboBox_SelectedIndexChanged: {cmdComboBox.SelectedIndex}", Color.Blue);            
        }

        private void ascii_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ingenico!.ASCII = ascii_CheckBox.Checked;
            if (string.IsNullOrEmpty(txtBoxHexDatas.Text))
            {
                return;
            }
            bool hex = false;
            //int hex = txtBoxHexDatas.Text.IndexOf("30");
            if ((txtBoxHexDatas.Text[0] == '0') && (txtBoxHexDatas.Text[1] == '2') && (txtBoxHexDatas.Text[3] == ' '))
            {
                hex = true;
            }
            if (hex)
            { // hex
                if (ascii_CheckBox.Checked)
                {
                    txtBoxHexDatas.Text = UtilsPost.HexToASCIIString(txtBoxHexDatas.Text);
                }
            }
            else
            { // ascii
                if (!ascii_CheckBox.Checked)
                {
                    txtBoxHexDatas.Text =  UtilsPost.ASCIIStringToHexString(txtBoxHexDatas.Text);
                }
            }
        }

        private void BtnCreateCrc_Click(object sender, EventArgs e)
        {
            //byte[] temp = Utils.StringHexToByteArray("10 0F 3E 42");
            //if ((txtBoxHexDatas.Text[0] == '0') && (txtBoxHexDatas.Text[1] == '2') && (txtBoxHexDatas.Text[2] == ' '))
            {
                byte[] _bhex = UtilsPost.StringHexToByteArray(txtBoxHexDatas.Text);
                byte _crc = UtilsPost.GetLRC(_bhex);

                byte[] _crcArray = { _crc };
                string _crcstring = UtilsPost.ByteArray_Hex_ASCII_ToString(_crcArray, 1);
                txtBoxCRC.Text = _crcstring;
            }
            return;
        }

        private void btnPaymentStart_Click(object sender, EventArgs e)
        {
        //    btnPaymentStart.Enabled = false;
            Ingenico.Skeleton!.SetPaymentAmountFt(mTextBoxPayment.Text);
        }

        private async void btnSendRaw_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBoxCRC.Text))
            {
                var result = MessageBox.Show("Empty", "Warning",
                                MessageBoxButtons.OKCancel);
                return;
            }

            try
            {
                byte[] aa = UtilsPost.StringHexToByteArray(txtBoxHexDatas.Text + " " + txtBoxCRC.Text);
                Command _cmd = new Command(0, aa, "\nAny Raw Cmd:");
                await ingenico!.SendAndWaitForResponse(_cmd);
            }
            catch (Exception ex)
            {
                writeTotbxLog($"ERROR: {ex.Message}", Color.Red);
            }
            return;
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            ConfigForm _cal = new ConfigForm(this);
            {
                _cal.Show(this);
            }
        }
    }
}