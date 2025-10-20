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
            this.textBoxFile.Location = new System.Drawing.Point(30, 59);
            this.textBoxFile.Name = "textBoxFile";
            this.textBoxFile.Size = new System.Drawing.Size(350, 22);
            this.textBoxFile.TabIndex = 10;
            this.textBoxFile.Text = "path/to/file.bin";
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(390, 57);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(80, 27);
            this.btnSelectFile.TabIndex = 11;
            this.btnSelectFile.Text = "Chọn File";
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // lblFilePath
            // 
            this.lblFilePath.AutoSize = true;
            this.lblFilePath.Location = new System.Drawing.Point(490, 62);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(99, 16);
            this.lblFilePath.TabIndex = 12;
            this.lblFilePath.Text = "Chưa chọn file...";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(30, 104);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(42, 16);
            this.lblPort.TabIndex = 13;
            this.lblPort.Text = "Cổng:";
            // 
            // comboBoxPorts
            // 
            this.comboBoxPorts.Location = new System.Drawing.Point(101, 101);
            this.comboBoxPorts.Name = "comboBoxPorts";
            this.comboBoxPorts.Size = new System.Drawing.Size(150, 24);
            this.comboBoxPorts.TabIndex = 14;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(261, 101);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(80, 25);
            this.btnRefresh.TabIndex = 15;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblBaudrate
            // 
            this.lblBaudrate.AutoSize = true;
            this.lblBaudrate.Location = new System.Drawing.Point(30, 139);
            this.lblBaudrate.Name = "lblBaudrate";
            this.lblBaudrate.Size = new System.Drawing.Size(65, 16);
            this.lblBaudrate.TabIndex = 16;
            this.lblBaudrate.Text = "Baudrate:";
            // 
            // comboBoxBaudrate
            // 
            this.comboBoxBaudrate.Location = new System.Drawing.Point(101, 136);
            this.comboBoxBaudrate.Name = "comboBoxBaudrate";
            this.comboBoxBaudrate.Size = new System.Drawing.Size(150, 24);
            this.comboBoxBaudrate.TabIndex = 17;
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.ItemHeight = 16;
            this.listBoxStatus.Items.AddRange(new object[] {
            "Bootloader started..."});
            this.listBoxStatus.Location = new System.Drawing.Point(30, 179);
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.Size = new System.Drawing.Size(440, 148);
            this.listBoxStatus.TabIndex = 18;
            // 
            // btnFlash
            // 
            this.btnFlash.Location = new System.Drawing.Point(490, 179);
            this.btnFlash.Name = "btnFlash";
            this.btnFlash.Size = new System.Drawing.Size(120, 35);
            this.btnFlash.TabIndex = 19;
            this.btnFlash.Text = "Nạp Firmware";
            this.btnFlash.Click += new System.EventHandler(this.btnFlash_Click);
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.Location = new System.Drawing.Point(3, 9);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(199, 29);
            this.label43.TabIndex = 81;
            this.label43.Text = "Install Firmware";
            // 
            // User_Install_Firmware
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
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
            this.Name = "User_Install_Firmware";
            this.Size = new System.Drawing.Size(1076, 644);
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
