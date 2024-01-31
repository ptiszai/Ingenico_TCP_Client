namespace IngenicoTestTCP
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            checkBox1 = new CheckBox();
            label1 = new Label();
            send1btn = new Button();
            label2 = new Label();
            CloseBtn = new Button();
            SuspendLayout();
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = CheckState.Checked;
            checkBox1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            checkBox1.ForeColor = Color.Blue;
            checkBox1.Location = new Point(12, 27);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(219, 19);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "SET TID, HostCode 88105, 90. page";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(12, 53);
            label1.Name = "label1";
            label1.Size = new Size(1056, 15);
            label1.TabIndex = 1;
            label1.Text = "02 30 30 30 30 30 30 38 30 30 6E 30 30 32 34 34 39 35 38 38 38 38 31 30 35 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 30 03 7F";
            // 
            // send1btn
            // 
            send1btn.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            send1btn.ForeColor = Color.Red;
            send1btn.Location = new Point(1231, 43);
            send1btn.Name = "send1btn";
            send1btn.Size = new Size(65, 25);
            send1btn.TabIndex = 2;
            send1btn.Text = "-->";
            send1btn.UseVisualStyleBackColor = true;
            send1btn.Click += send1btn_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.Red;
            label2.Location = new Point(411, 9);
            label2.Name = "label2";
            label2.Size = new Size(426, 21);
            label2.TabIndex = 3;
            label2.Text = "External interface protocol for iSelf EFT/POS terminals.";
            // 
            // CloseBtn
            // 
            CloseBtn.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            CloseBtn.Location = new Point(1231, 14);
            CloseBtn.Name = "CloseBtn";
            CloseBtn.Size = new Size(75, 23);
            CloseBtn.TabIndex = 4;
            CloseBtn.Text = "Close";
            CloseBtn.UseVisualStyleBackColor = true;
            CloseBtn.Click += CloseBtn_Click;
            // 
            // ConfigForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1308, 450);
            Controls.Add(CloseBtn);
            Controls.Add(label2);
            Controls.Add(send1btn);
            Controls.Add(label1);
            Controls.Add(checkBox1);
            Name = "ConfigForm";
            Text = "Config";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox checkBox1;
        private Label label1;
        private Button send1btn;
        private Label label2;
        private Button CloseBtn;
    }
}