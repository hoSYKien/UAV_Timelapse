namespace UAV_Timelapse
{
    partial class Form_Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnData = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.PictureBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBoxBaudrate = new System.Windows.Forms.ComboBox();
            this.comboBoxPorts = new System.Windows.Forms.ComboBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.textBoxTelemetry = new System.Windows.Forms.TextBox();
            this.panelSetup = new System.Windows.Forms.Panel();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.PanelOptional = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRTK_GPS = new System.Windows.Forms.Button();
            this.btnCAN_GPS = new System.Windows.Forms.Button();
            this.btnJoystick = new System.Windows.Forms.Button();
            this.btnComp_Motor = new System.Windows.Forms.Button();
            this.btnRangerFinder = new System.Windows.Forms.Button();
            this.btnOpticalFlow_OSD = new System.Windows.Forms.Button();
            this.btnCamGimbal = new System.Windows.Forms.Button();
            this.btnMotorTest = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOptional = new System.Windows.Forms.Button();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnInstallFirmware = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnData)).BeginInit();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnConnect)).BeginInit();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.panel2.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelSetup.SuspendLayout();
            this.PanelOptional.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.panel7);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel6);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1260, 82);
            this.panel1.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.btnData);
            this.panel5.Controls.Add(this.label1);
            this.panel5.Location = new System.Drawing.Point(12, 2);
            this.panel5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(69, 80);
            this.panel5.TabIndex = 7;
            // 
            // btnData
            // 
            this.btnData.BackColor = System.Drawing.Color.White;
            this.btnData.Image = ((System.Drawing.Image)(resources.GetObject("btnData.Image")));
            this.btnData.Location = new System.Drawing.Point(4, 2);
            this.btnData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnData.Name = "btnData";
            this.btnData.Size = new System.Drawing.Size(61, 53);
            this.btnData.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnData.TabIndex = 0;
            this.btnData.TabStop = false;
            this.btnData.Click += new System.EventHandler(this.btnData_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Data";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.pictureBox2);
            this.panel7.Controls.Add(this.label3);
            this.panel7.Location = new System.Drawing.Point(163, 2);
            this.panel7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(69, 80);
            this.panel7.TabIndex = 9;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.White;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(4, 2);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(61, 53);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Config";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnRefresh);
            this.panel3.Controls.Add(this.panel8);
            this.panel3.Controls.Add(this.comboBox3);
            this.panel3.Controls.Add(this.comboBoxBaudrate);
            this.panel3.Controls.Add(this.comboBoxPorts);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(894, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(364, 80);
            this.panel3.TabIndex = 6;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(9, 52);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 10;
            this.btnRefresh.Text = "Làm Mới";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.label4);
            this.panel8.Controls.Add(this.btnConnect);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel8.Location = new System.Drawing.Point(295, 0);
            this.panel8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(69, 80);
            this.panel8.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Connect";
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.White;
            this.btnConnect.Image = ((System.Drawing.Image)(resources.GetObject("btnConnect.Image")));
            this.btnConnect.Location = new System.Drawing.Point(4, 2);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(61, 55);
            this.btnConnect.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnConnect.TabIndex = 7;
            this.btnConnect.TabStop = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(90, 51);
            this.comboBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(201, 24);
            this.comboBox3.TabIndex = 4;
            // 
            // comboBoxBaudrate
            // 
            this.comboBoxBaudrate.FormattingEnabled = true;
            this.comboBoxBaudrate.Items.AddRange(new object[] {
            "57600",
            "115200",
            "230400",
            "460800"});
            this.comboBoxBaudrate.Location = new System.Drawing.Point(170, 13);
            this.comboBoxBaudrate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxBaudrate.Name = "comboBoxBaudrate";
            this.comboBoxBaudrate.Size = new System.Drawing.Size(121, 24);
            this.comboBoxBaudrate.TabIndex = 3;
            // 
            // comboBoxPorts
            // 
            this.comboBoxPorts.FormattingEnabled = true;
            this.comboBoxPorts.Location = new System.Drawing.Point(42, 13);
            this.comboBoxPorts.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBoxPorts.Name = "comboBoxPorts";
            this.comboBoxPorts.Size = new System.Drawing.Size(121, 24);
            this.comboBoxPorts.TabIndex = 2;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.pictureBox3);
            this.panel6.Controls.Add(this.label2);
            this.panel6.Location = new System.Drawing.Point(87, 2);
            this.panel6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(69, 80);
            this.panel6.TabIndex = 8;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.White;
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(3, 2);
            this.pictureBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(65, 53);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 2;
            this.pictureBox3.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Setup";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panelMain);
            this.panel2.Controls.Add(this.panelSetup);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 82);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1260, 560);
            this.panel2.TabIndex = 1;
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.textBoxTelemetry);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(221, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1039, 560);
            this.panelMain.TabIndex = 2;
            // 
            // textBoxTelemetry
            // 
            this.textBoxTelemetry.Font = new System.Drawing.Font("Consolas", 12F);
            this.textBoxTelemetry.Location = new System.Drawing.Point(18, 26);
            this.textBoxTelemetry.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxTelemetry.Multiline = true;
            this.textBoxTelemetry.Name = "textBoxTelemetry";
            this.textBoxTelemetry.ReadOnly = true;
            this.textBoxTelemetry.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxTelemetry.Size = new System.Drawing.Size(1011, 508);
            this.textBoxTelemetry.TabIndex = 6;
            this.textBoxTelemetry.Text = "GroundSpeed:\r\nYaw:\r\nAltitude:\r\nBattery:\r\nGPS:\r\nMode:";
            // 
            // panelSetup
            // 
            this.panelSetup.AutoScroll = true;
            this.panelSetup.AutoScrollMargin = new System.Drawing.Size(20, 30);
            this.panelSetup.AutoScrollMinSize = new System.Drawing.Size(20, 20);
            this.panelSetup.Controls.Add(this.flowLayoutPanel4);
            this.panelSetup.Controls.Add(this.PanelOptional);
            this.panelSetup.Controls.Add(this.flowLayoutPanel1);
            this.panelSetup.Controls.Add(this.flowLayoutPanel3);
            this.panelSetup.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSetup.Location = new System.Drawing.Point(0, 0);
            this.panelSetup.Name = "panelSetup";
            this.panelSetup.Size = new System.Drawing.Size(221, 560);
            this.panelSetup.TabIndex = 0;
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(0, 560);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(200, 100);
            this.flowLayoutPanel4.TabIndex = 0;
            // 
            // PanelOptional
            // 
            this.PanelOptional.Controls.Add(this.btnRTK_GPS);
            this.PanelOptional.Controls.Add(this.btnCAN_GPS);
            this.PanelOptional.Controls.Add(this.btnJoystick);
            this.PanelOptional.Controls.Add(this.btnComp_Motor);
            this.PanelOptional.Controls.Add(this.btnRangerFinder);
            this.PanelOptional.Controls.Add(this.btnOpticalFlow_OSD);
            this.PanelOptional.Controls.Add(this.btnCamGimbal);
            this.PanelOptional.Controls.Add(this.btnMotorTest);
            this.PanelOptional.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelOptional.Location = new System.Drawing.Point(0, 100);
            this.PanelOptional.Name = "PanelOptional";
            this.PanelOptional.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.PanelOptional.Size = new System.Drawing.Size(200, 460);
            this.PanelOptional.TabIndex = 0;
            // 
            // btnRTK_GPS
            // 
            this.btnRTK_GPS.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnRTK_GPS.Location = new System.Drawing.Point(12, 3);
            this.btnRTK_GPS.Name = "btnRTK_GPS";
            this.btnRTK_GPS.Size = new System.Drawing.Size(185, 48);
            this.btnRTK_GPS.TabIndex = 1;
            this.btnRTK_GPS.Text = "RTK/GPS Inject";
            this.btnRTK_GPS.UseVisualStyleBackColor = false;
            this.btnRTK_GPS.Click += new System.EventHandler(this.btnRTK_GPS_Click);
            // 
            // btnCAN_GPS
            // 
            this.btnCAN_GPS.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnCAN_GPS.Location = new System.Drawing.Point(12, 57);
            this.btnCAN_GPS.Name = "btnCAN_GPS";
            this.btnCAN_GPS.Size = new System.Drawing.Size(185, 48);
            this.btnCAN_GPS.TabIndex = 2;
            this.btnCAN_GPS.Text = "CAN GPS Order";
            this.btnCAN_GPS.UseVisualStyleBackColor = false;
            this.btnCAN_GPS.Click += new System.EventHandler(this.btnCAN_GPS_Click);
            // 
            // btnJoystick
            // 
            this.btnJoystick.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnJoystick.Location = new System.Drawing.Point(12, 111);
            this.btnJoystick.Name = "btnJoystick";
            this.btnJoystick.Size = new System.Drawing.Size(185, 48);
            this.btnJoystick.TabIndex = 3;
            this.btnJoystick.Text = "Joystick";
            this.btnJoystick.UseVisualStyleBackColor = false;
            this.btnJoystick.Click += new System.EventHandler(this.btnJoystick_Click);
            // 
            // btnComp_Motor
            // 
            this.btnComp_Motor.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnComp_Motor.Location = new System.Drawing.Point(12, 165);
            this.btnComp_Motor.Name = "btnComp_Motor";
            this.btnComp_Motor.Size = new System.Drawing.Size(185, 48);
            this.btnComp_Motor.TabIndex = 4;
            this.btnComp_Motor.Text = "Compass/Motor Calid";
            this.btnComp_Motor.UseVisualStyleBackColor = false;
            this.btnComp_Motor.Click += new System.EventHandler(this.btnComp_Motor_Click);
            // 
            // btnRangerFinder
            // 
            this.btnRangerFinder.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnRangerFinder.Location = new System.Drawing.Point(12, 219);
            this.btnRangerFinder.Name = "btnRangerFinder";
            this.btnRangerFinder.Size = new System.Drawing.Size(185, 48);
            this.btnRangerFinder.TabIndex = 5;
            this.btnRangerFinder.Text = "Ranger Finder";
            this.btnRangerFinder.UseVisualStyleBackColor = false;
            this.btnRangerFinder.Click += new System.EventHandler(this.btnRangerFinder_Click);
            // 
            // btnOpticalFlow_OSD
            // 
            this.btnOpticalFlow_OSD.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnOpticalFlow_OSD.Location = new System.Drawing.Point(12, 273);
            this.btnOpticalFlow_OSD.Name = "btnOpticalFlow_OSD";
            this.btnOpticalFlow_OSD.Size = new System.Drawing.Size(185, 48);
            this.btnOpticalFlow_OSD.TabIndex = 6;
            this.btnOpticalFlow_OSD.Text = "Optical Flow/OSD";
            this.btnOpticalFlow_OSD.UseVisualStyleBackColor = false;
            this.btnOpticalFlow_OSD.Click += new System.EventHandler(this.btnOpticalFlow_OSD_Click);
            // 
            // btnCamGimbal
            // 
            this.btnCamGimbal.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnCamGimbal.Location = new System.Drawing.Point(12, 327);
            this.btnCamGimbal.Name = "btnCamGimbal";
            this.btnCamGimbal.Size = new System.Drawing.Size(185, 59);
            this.btnCamGimbal.TabIndex = 7;
            this.btnCamGimbal.Text = "Camera Gimbal";
            this.btnCamGimbal.UseVisualStyleBackColor = false;
            this.btnCamGimbal.Click += new System.EventHandler(this.btnCamGimbal_Click);
            // 
            // btnMotorTest
            // 
            this.btnMotorTest.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnMotorTest.Location = new System.Drawing.Point(12, 392);
            this.btnMotorTest.Name = "btnMotorTest";
            this.btnMotorTest.Size = new System.Drawing.Size(185, 59);
            this.btnMotorTest.TabIndex = 9;
            this.btnMotorTest.Text = "Motor Test";
            this.btnMotorTest.UseVisualStyleBackColor = false;
            this.btnMotorTest.Click += new System.EventHandler(this.btnMotorTest_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnOptional);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 49);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(200, 51);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnOptional
            // 
            this.btnOptional.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnOptional.Location = new System.Drawing.Point(3, 3);
            this.btnOptional.Name = "btnOptional";
            this.btnOptional.Size = new System.Drawing.Size(218, 42);
            this.btnOptional.TabIndex = 0;
            this.btnOptional.Text = "Optional Hardware";
            this.btnOptional.UseVisualStyleBackColor = false;
            this.btnOptional.Click += new System.EventHandler(this.btnOptional_Click);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.btnInstallFirmware);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(200, 49);
            this.flowLayoutPanel3.TabIndex = 1;
            // 
            // btnInstallFirmware
            // 
            this.btnInstallFirmware.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnInstallFirmware.Location = new System.Drawing.Point(3, 3);
            this.btnInstallFirmware.Name = "btnInstallFirmware";
            this.btnInstallFirmware.Size = new System.Drawing.Size(218, 42);
            this.btnInstallFirmware.TabIndex = 0;
            this.btnInstallFirmware.Text = "Install Firmware";
            this.btnInstallFirmware.UseVisualStyleBackColor = false;
            this.btnInstallFirmware.Click += new System.EventHandler(this.btnInstallFirmware_Click);
            // 
            // Form_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1260, 642);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form_Main";
            this.Text = "Form_Main";
            this.panel1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnData)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnConnect)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.panelSetup.ResumeLayout(false);
            this.PanelOptional.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox btnData;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox btnConnect;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBoxBaudrate;
        private System.Windows.Forms.ComboBox comboBoxPorts;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Button btnInstallFirmware;
        private System.Windows.Forms.Panel panelSetup;
        private System.Windows.Forms.FlowLayoutPanel PanelOptional;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button btnRTK_GPS;
        private System.Windows.Forms.Button btnCAN_GPS;
        private System.Windows.Forms.Button btnJoystick;
        private System.Windows.Forms.Button btnComp_Motor;
        private System.Windows.Forms.Button btnRangerFinder;
        private System.Windows.Forms.Button btnOptional;
        private System.Windows.Forms.Button btnOpticalFlow_OSD;
        private System.Windows.Forms.Button btnCamGimbal;
        private System.Windows.Forms.Button btnMotorTest;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxTelemetry;
    }
}

