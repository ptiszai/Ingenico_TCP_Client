namespace IngenicoTestTCP
{
    partial class FormClient
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            btnSend = new Button();
            exitBtn = new Button();
            tbxLog = new RichTextBox();
            clearBtn = new Button();
            cmdComboBox = new ComboBox();
            label1 = new Label();
            ascii_CheckBox = new CheckBox();
            BtnCreateCrc = new Button();
            txtBoxHexDatas = new TextBox();
            label2 = new Label();
            txtBoxCRC = new TextBox();
            label3 = new Label();
            btnConfig = new Button();
            label4 = new Label();
            label5 = new Label();
            mTextBoxPayment = new MaskedTextBox();
            btnPaymentStart = new Button();
            btnSendRaw = new Button();
            toolTip1 = new ToolTip(components);
            SuspendLayout();
            // 
            // btnSend
            // 
            btnSend.BackColor = SystemColors.ButtonShadow;
            btnSend.Location = new Point(353, 5);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(75, 23);
            btnSend.TabIndex = 0;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = false;
            btnSend.Click += btnSend_Click;
            // 
            // exitBtn
            // 
            exitBtn.Location = new Point(921, 12);
            exitBtn.Name = "exitBtn";
            exitBtn.Size = new Size(75, 23);
            exitBtn.TabIndex = 3;
            exitBtn.Text = "Exit";
            exitBtn.UseVisualStyleBackColor = true;
            exitBtn.Click += exitBtn_Click;
            // 
            // tbxLog
            // 
            tbxLog.BackColor = SystemColors.Info;
            tbxLog.Location = new Point(10, 204);
            tbxLog.Name = "tbxLog";
            tbxLog.Size = new Size(984, 436);
            tbxLog.TabIndex = 4;
            tbxLog.Text = "";
            // 
            // clearBtn
            // 
            clearBtn.Location = new Point(921, 162);
            clearBtn.Name = "clearBtn";
            clearBtn.Size = new Size(75, 23);
            clearBtn.TabIndex = 6;
            clearBtn.Text = "Clear";
            clearBtn.UseVisualStyleBackColor = true;
            clearBtn.Click += clearBtn_Click;
            // 
            // cmdComboBox
            // 
            cmdComboBox.FormattingEnabled = true;
            cmdComboBox.Location = new Point(77, 6);
            cmdComboBox.Name = "cmdComboBox";
            cmdComboBox.Size = new Size(270, 23);
            cmdComboBox.TabIndex = 8;
            cmdComboBox.SelectedIndexChanged += cmdComboBox_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(4, 9);
            label1.Name = "label1";
            label1.Size = new Size(67, 15);
            label1.TabIndex = 9;
            label1.Text = "Command:";
            // 
            // ascii_CheckBox
            // 
            ascii_CheckBox.AutoSize = true;
            ascii_CheckBox.Location = new Point(7, 165);
            ascii_CheckBox.Name = "ascii_CheckBox";
            ascii_CheckBox.Size = new Size(54, 19);
            ascii_CheckBox.TabIndex = 10;
            ascii_CheckBox.Text = "ASCII";
            ascii_CheckBox.UseVisualStyleBackColor = true;
            ascii_CheckBox.CheckedChanged += ascii_CheckBox_CheckedChanged;
            // 
            // BtnCreateCrc
            // 
            BtnCreateCrc.Location = new Point(921, 673);
            BtnCreateCrc.Name = "BtnCreateCrc";
            BtnCreateCrc.Size = new Size(75, 23);
            BtnCreateCrc.TabIndex = 11;
            BtnCreateCrc.Text = "Creata CRC";
            BtnCreateCrc.UseVisualStyleBackColor = true;
            BtnCreateCrc.Click += BtnCreateCrc_Click;
            // 
            // txtBoxHexDatas
            // 
            txtBoxHexDatas.Location = new Point(77, 673);
            txtBoxHexDatas.Name = "txtBoxHexDatas";
            txtBoxHexDatas.Size = new Size(838, 23);
            txtBoxHexDatas.TabIndex = 12;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 676);
            label2.Name = "label2";
            label2.Size = new Size(61, 15);
            label2.TabIndex = 13;
            label2.Text = "Hes datas:";
            // 
            // txtBoxCRC
            // 
            txtBoxCRC.BackColor = SystemColors.Info;
            txtBoxCRC.Location = new Point(77, 702);
            txtBoxCRC.Name = "txtBoxCRC";
            txtBoxCRC.ReadOnly = true;
            txtBoxCRC.Size = new Size(40, 23);
            txtBoxCRC.TabIndex = 14;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 705);
            label3.Name = "label3";
            label3.Size = new Size(33, 15);
            label3.TabIndex = 15;
            label3.Text = "CRC:";
            // 
            // btnConfig
            // 
            btnConfig.BackColor = SystemColors.ButtonShadow;
            btnConfig.Location = new Point(465, 6);
            btnConfig.Name = "btnConfig";
            btnConfig.Size = new Size(75, 23);
            btnConfig.TabIndex = 16;
            btnConfig.Text = "Configs";
            btnConfig.UseVisualStyleBackColor = false;
            btnConfig.Click += btnConfig_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(4, 51);
            label4.Name = "label4";
            label4.Size = new Size(57, 15);
            label4.TabIndex = 17;
            label4.Text = "Payment:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(161, 50);
            label5.Name = "label5";
            label5.Size = new Size(17, 15);
            label5.TabIndex = 19;
            label5.Text = "Ft";
            // 
            // mTextBoxPayment
            // 
            mTextBoxPayment.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            mTextBoxPayment.Location = new Point(77, 45);
            mTextBoxPayment.Mask = "0000000";
            mTextBoxPayment.Name = "mTextBoxPayment";
            mTextBoxPayment.Size = new Size(78, 25);
            mTextBoxPayment.TabIndex = 20;
            mTextBoxPayment.Text = "100";
            mTextBoxPayment.ValidatingType = typeof(int);
            // 
            // btnPaymentStart
            // 
            btnPaymentStart.BackColor = SystemColors.ButtonShadow;
            btnPaymentStart.Location = new Point(187, 48);
            btnPaymentStart.Name = "btnPaymentStart";
            btnPaymentStart.Size = new Size(97, 23);
            btnPaymentStart.TabIndex = 21;
            btnPaymentStart.Text = "Payment Start";
            btnPaymentStart.UseVisualStyleBackColor = false;
            btnPaymentStart.Click += btnPaymentStart_Click;
            // 
            // btnSendRaw
            // 
            btnSendRaw.BackColor = SystemColors.ButtonShadow;
            btnSendRaw.Location = new Point(919, 701);
            btnSendRaw.Name = "btnSendRaw";
            btnSendRaw.Size = new Size(75, 23);
            btnSendRaw.TabIndex = 22;
            btnSendRaw.Text = "Send Raw";
            btnSendRaw.UseVisualStyleBackColor = false;
            btnSendRaw.Click += btnSendRaw_Click;
            // 
            // FormClient
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1008, 729);
            Controls.Add(btnSendRaw);
            Controls.Add(btnPaymentStart);
            Controls.Add(mTextBoxPayment);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(btnConfig);
            Controls.Add(label3);
            Controls.Add(txtBoxCRC);
            Controls.Add(label2);
            Controls.Add(txtBoxHexDatas);
            Controls.Add(BtnCreateCrc);
            Controls.Add(ascii_CheckBox);
            Controls.Add(label1);
            Controls.Add(cmdComboBox);
            Controls.Add(clearBtn);
            Controls.Add(tbxLog);
            Controls.Add(exitBtn);
            Controls.Add(btnSend);
            MaximizeBox = false;
            Name = "FormClient";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ingenico TCP Client";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnSend;
        private Button exitBtn;
        private RichTextBox tbxLog;
        private Button clearBtn;
        private ComboBox cmdComboBox;
        private Label label1;
        private CheckBox ascii_CheckBox;
        private Button BtnCreateCrc;
        private TextBox txtBoxHexDatas;
        private Label label2;
        private TextBox txtBoxCRC;
        private Label label3;
        private Button btnConfig;
        private Label label4;
        private Label label5;
        private MaskedTextBox mTextBoxPayment;
        private Button btnPaymentStart;
        private Button btnSendRaw;
        private ToolTip toolTip1;
    }
}