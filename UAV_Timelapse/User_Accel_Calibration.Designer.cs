namespace UAV_Timelapse
{
    partial class User_Accel_Calibration
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
                if (_main != null)
                {
                    _main.OnStatustext -= Main_OnStatustext;
                    _main.OnCommandLong -= Main_OnCommandLong;
                }
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnCalibAccel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCalibLevel = new System.Windows.Forms.Button();
            this.btnSimpleAccelCal = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(319, 36);
            this.label1.TabIndex = 17;
            this.label1.Text = "Đặt flight controller ở tư thế cân bằng để thiết lập giá trị Min/Max mặc định của" +
    " gia tốc kế (3 trục)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCalibAccel
            // 
            this.btnCalibAccel.Location = new System.Drawing.Point(112, 79);
            this.btnCalibAccel.Name = "btnCalibAccel";
            this.btnCalibAccel.Size = new System.Drawing.Size(102, 43);
            this.btnCalibAccel.TabIndex = 18;
            this.btnCalibAccel.Text = "Hiệu chuẩn gia tốc kế";
            this.btnCalibAccel.UseVisualStyleBackColor = true;
            this.btnCalibAccel.Click += new System.EventHandler(this.btnCalibAccel_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(319, 36);
            this.label2.TabIndex = 23;
            this.label2.Text = "Đặt flight controller cân bằng để thiết lập giá trị lệch (offset) mặc định của gi" +
    "a tốc kế (1 trục / tinh chỉnh AHRS)";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(4, 255);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(319, 36);
            this.label3.TabIndex = 24;
            this.label3.Text = "Đặt flight controller cân bằng để thiết lập hệ số scale mặc định của gia tốc kế c" +
    "ho bay ngang (1 trục)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(196, 36);
            this.label4.TabIndex = 25;
            this.label4.Text = "Hiệu chuẩn gia tốc kế";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCalibLevel
            // 
            this.btnCalibLevel.Location = new System.Drawing.Point(112, 187);
            this.btnCalibLevel.Name = "btnCalibLevel";
            this.btnCalibLevel.Size = new System.Drawing.Size(102, 43);
            this.btnCalibLevel.TabIndex = 26;
            this.btnCalibLevel.Text = "Hiệu chuẩn cân bằng";
            this.btnCalibLevel.UseVisualStyleBackColor = true;
            this.btnCalibLevel.Click += new System.EventHandler(this.btnCalibLevel_Click);
            // 
            // btnSimpleAccelCal
            // 
            this.btnSimpleAccelCal.Location = new System.Drawing.Point(112, 294);
            this.btnSimpleAccelCal.Name = "btnSimpleAccelCal";
            this.btnSimpleAccelCal.Size = new System.Drawing.Size(102, 43);
            this.btnSimpleAccelCal.TabIndex = 27;
            this.btnSimpleAccelCal.Text = "Hiệu chuẩn gia tốc đơn giản";
            this.btnSimpleAccelCal.UseVisualStyleBackColor = true;
            this.btnSimpleAccelCal.Click += new System.EventHandler(this.btnSimpleAccelCal_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(146, 125);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(24, 13);
            this.lblStatus.TabIndex = 28;
            this.lblStatus.Text = "test";
            // 
            // User_Accel_Calibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnSimpleAccelCal);
            this.Controls.Add(this.btnCalibLevel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCalibAccel);
            this.Controls.Add(this.label1);
            this.Name = "User_Accel_Calibration";
            this.Size = new System.Drawing.Size(761, 549);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCalibAccel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCalibLevel;
        private System.Windows.Forms.Button btnSimpleAccelCal;
        private System.Windows.Forms.Label lblStatus;
    }
}
