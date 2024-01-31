using IngenicoTestTCP.TcpIp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace IngenicoTestTCP
{
    public partial class ConfigForm : Form
    {
        #region private variables
        FormClient parent;
        #endregion
        public ConfigForm(FormClient parent_a)
        {
            InitializeComponent();
            parent = parent_a;
        }


        #region private eventHandler
        private void sendbtns(bool enabled)
        {
            send1btn.Enabled = enabled;
        }        

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void send1btn_Click(object sender, EventArgs e)
        { // SET TID, HostCode 88105
            try
            {
                sendbtns(false);
                //Command _cmd = cmds.CommandList[Content.ACK];
                //Command _cmd = cmds.CommandList.Values.ElementAt(cmdComboBox.SelectedIndex);
                //await ingenico!.SendAndWaitForResponse(_cmd);
            }
            finally
            {
                sendbtns(true);
            }
        }
       
        #endregion
    }
}
