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
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.rtxtDataRespond = new System.Windows.Forms.RichTextBox();
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
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(112, 187);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(102, 43);
            this.button2.TabIndex = 26;
            this.button2.Text = "Hiệu chuẩn cân bằng";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(112, 294);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(102, 43);
            this.button3.TabIndex = 27;
            this.button3.Text = "Hiệu chuẩn gia tốc đơn giản";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(146, 125);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(24, 13);
            this.lblStatus.TabIndex = 28;
            this.lblStatus.Text = "test";
            this.lblStatus.Visible = false;
            // 
            // rtxtDataRespond
            // 
            this.rtxtDataRespond.Location = new System.Drawing.Point(348, 26);
            this.rtxtDataRespond.Name = "rtxtDataRespond";
            this.rtxtDataRespond.Size = new System.Drawing.Size(397, 120);
            this.rtxtDataRespond.TabIndex = 29;
            this.rtxtDataRespond.Text = "";
            this.rtxtDataRespond.Visible = false;
            // 
            // User_Accel_Calibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtxtDataRespond);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
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
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.RichTextBox rtxtDataRespond;
    }
}
