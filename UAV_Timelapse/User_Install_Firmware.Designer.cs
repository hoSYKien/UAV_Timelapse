namespace UAV_Timelapse
{
    partial class User_Install_Firmware
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxFile = new System.Windows.Forms.TextBox();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.lblFilePath = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.comboBoxPorts = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblBaudrate = new System.Windows.Forms.Label();
            this.comboBoxBaudrate = new System.Windows.Forms.ComboBox();
            this.listBoxStatus = new System.Windows.Forms.ListBox();
            this.btnFlash = new System.Windows.Forms.Button();
            this.label43 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxFile
            // 
            this.textBoxFile.Location = new System.Drawing.Point(22, 48);
            this.textBoxFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxFile.Name = "textBoxFile";
            this.textBoxFile.Size = new System.Drawing.Size(264, 20);
            this.textBoxFile.TabIndex = 10;
            this.textBoxFile.Text = "path/to/file.bin";
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(292, 46);
            this.btnSelectFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(60, 22);
            this.btnSelectFile.TabIndex = 11;
            this.btnSelectFile.Text = "Chọn File";
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // lblFilePath
            // 
            this.lblFilePath.AutoSize = true;
            this.lblFilePath.Location = new System.Drawing.Point(368, 50);
            this.lblFilePath.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(84, 13);
            this.lblFilePath.TabIndex = 12;
            this.lblFilePath.Text = "Chưa chọn file...";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(22, 84);
            this.lblPort.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(35, 13);
            this.lblPort.TabIndex = 13;
            this.lblPort.Text = "Cổng:";
            // 
            // comboBoxPorts
            // 
            this.comboBoxPorts.Location = new System.Drawing.Point(76, 82);
            this.comboBoxPorts.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBoxPorts.Name = "comboBoxPorts";
            this.comboBoxPorts.Size = new System.Drawing.Size(114, 21);
            this.comboBoxPorts.TabIndex = 14;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(196, 82);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(60, 20);
            this.btnRefresh.TabIndex = 15;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblBaudrate
            // 
            this.lblBaudrate.AutoSize = true;
            this.lblBaudrate.Location = new System.Drawing.Point(22, 113);
            this.lblBaudrate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBaudrate.Name = "lblBaudrate";
            this.lblBaudrate.Size = new System.Drawing.Size(53, 13);
            this.lblBaudrate.TabIndex = 16;
            this.lblBaudrate.Text = "Baudrate:";
            // 
            // comboBoxBaudrate
            // 
            this.comboBoxBaudrate.Location = new System.Drawing.Point(76, 110);
            this.comboBoxBaudrate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBoxBaudrate.Name = "comboBoxBaudrate";
            this.comboBoxBaudrate.Size = new System.Drawing.Size(114, 21);
            this.comboBoxBaudrate.TabIndex = 17;
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.Items.AddRange(new object[] {
            "Bootloader started..."});
            this.listBoxStatus.Location = new System.Drawing.Point(22, 145);
            this.listBoxStatus.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.Size = new System.Drawing.Size(331, 121);
            this.listBoxStatus.TabIndex = 18;
            // 
            // btnFlash
            // 
            this.btnFlash.Location = new System.Drawing.Point(368, 145);
            this.btnFlash.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnFlash.Name = "btnFlash";
            this.btnFlash.Size = new System.Drawing.Size(90, 28);
            this.btnFlash.TabIndex = 19;
            this.btnFlash.Text = "Nạp Firmware";
            this.btnFlash.Click += new System.EventHandler(this.btnFlash_Click);
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.Location = new System.Drawing.Point(2, 7);
            this.label43.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(132, 24);
            this.label43.TabIndex = 81;
            this.label43.Text = "Tải Firmware";
            // 
            // User_Install_Firmware
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label43);
            this.Controls.Add(this.textBoxFile);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.lblFilePath);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.comboBoxPorts);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lblBaudrate);
            this.Controls.Add(this.comboBoxBaudrate);
            this.Controls.Add(this.listBoxStatus);
            this.Controls.Add(this.btnFlash);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "User_Install_Firmware";
            this.Size = new System.Drawing.Size(807, 523);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxFile;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.ComboBox comboBoxPorts;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblBaudrate;
        private System.Windows.Forms.ComboBox comboBoxBaudrate;
        private System.Windows.Forms.ListBox listBoxStatus;
        private System.Windows.Forms.Button btnFlash;
        private System.Windows.Forms.Label label43;
    }
}
