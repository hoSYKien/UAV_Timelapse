namespace UAV_Timelapse
{
    partial class User_Motor_Test
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
            this.numThrottle = new System.Windows.Forms.NumericUpDown();
            this.numDuration = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblClass = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.btnTestSequenceMotor = new System.Windows.Forms.Button();
            this.btnStopAllMotor = new System.Windows.Forms.Button();
            this.btnTestAllMotor = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.flowMotors = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.numThrottle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Kiểm tra động cơ";
            // 
            // numThrottle
            // 
            this.numThrottle.Location = new System.Drawing.Point(53, 48);
            this.numThrottle.Margin = new System.Windows.Forms.Padding(2);
            this.numThrottle.Name = "numThrottle";
            this.numThrottle.Size = new System.Drawing.Size(67, 20);
            this.numThrottle.TabIndex = 2;
            this.numThrottle.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // numDuration
            // 
            this.numDuration.Location = new System.Drawing.Point(221, 48);
            this.numDuration.Margin = new System.Windows.Forms.Padding(2);
            this.numDuration.Name = "numDuration";
            this.numDuration.Size = new System.Drawing.Size(67, 20);
            this.numDuration.TabIndex = 3;
            this.numDuration.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 50);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Ga (%)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(152, 50);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Thời gian (s)";
            // 
            // lblClass
            // 
            this.lblClass.AutoSize = true;
            this.lblClass.Location = new System.Drawing.Point(11, 85);
            this.lblClass.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(36, 13);
            this.lblClass.TabIndex = 6;
            this.lblClass.Text = "Dạng:";
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(152, 85);
            this.lblType.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(63, 13);
            this.lblType.TabIndex = 7;
            this.lblType.Text = "Loại khung:";
            // 
            // btnTestSequenceMotor
            // 
            this.btnTestSequenceMotor.BackColor = System.Drawing.Color.DarkCyan;
            this.btnTestSequenceMotor.Location = new System.Drawing.Point(155, 190);
            this.btnTestSequenceMotor.Margin = new System.Windows.Forms.Padding(2);
            this.btnTestSequenceMotor.Name = "btnTestSequenceMotor";
            this.btnTestSequenceMotor.Size = new System.Drawing.Size(82, 38);
            this.btnTestSequenceMotor.TabIndex = 14;
            this.btnTestSequenceMotor.Text = "Kiểm tra lần lượt từng động cơ";
            this.btnTestSequenceMotor.UseVisualStyleBackColor = false;
            this.btnTestSequenceMotor.Click += new System.EventHandler(this.btnTestSequenceMotor_Click);
            // 
            // btnStopAllMotor
            // 
            this.btnStopAllMotor.BackColor = System.Drawing.Color.DarkCyan;
            this.btnStopAllMotor.Location = new System.Drawing.Point(155, 148);
            this.btnStopAllMotor.Margin = new System.Windows.Forms.Padding(2);
            this.btnStopAllMotor.Name = "btnStopAllMotor";
            this.btnStopAllMotor.Size = new System.Drawing.Size(82, 38);
            this.btnStopAllMotor.TabIndex = 13;
            this.btnStopAllMotor.Text = "Dừng tất cả động cơ";
            this.btnStopAllMotor.UseVisualStyleBackColor = false;
            this.btnStopAllMotor.Click += new System.EventHandler(this.btnStopAllMotor_Click);
            // 
            // btnTestAllMotor
            // 
            this.btnTestAllMotor.BackColor = System.Drawing.Color.DarkCyan;
            this.btnTestAllMotor.Location = new System.Drawing.Point(155, 106);
            this.btnTestAllMotor.Margin = new System.Windows.Forms.Padding(2);
            this.btnTestAllMotor.Name = "btnTestAllMotor";
            this.btnTestAllMotor.Size = new System.Drawing.Size(82, 38);
            this.btnTestAllMotor.TabIndex = 12;
            this.btnTestAllMotor.Text = "Kiểm tra tất cả động cơ";
            this.btnTestAllMotor.UseVisualStyleBackColor = false;
            this.btnTestAllMotor.Click += new System.EventHandler(this.btnTestAllMotor_Click);
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.DarkCyan;
            this.button5.Location = new System.Drawing.Point(310, 48);
            this.button5.Margin = new System.Windows.Forms.Padding(2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(82, 54);
            this.button5.TabIndex = 15;
            this.button5.Text = "Đặt tốc độ tối thiểu khi vừa arm";
            this.button5.UseVisualStyleBackColor = false;
            // 
            // button9
            // 
            this.button9.BackColor = System.Drawing.Color.DarkCyan;
            this.button9.Location = new System.Drawing.Point(310, 106);
            this.button9.Margin = new System.Windows.Forms.Padding(2);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(82, 51);
            this.button9.TabIndex = 16;
            this.button9.Text = "Đặt tốc độ tối thiểu khi bay";
            this.button9.UseVisualStyleBackColor = false;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(409, 50);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(132, 48);
            this.label6.TabIndex = 17;
            this.label6.Text = "Thiết lập % ga tối thiểu khi đã arm nhưng vẫn còn trên mặt đất";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(409, 114);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(132, 38);
            this.label7.TabIndex = 18;
            this.label7.Text = "Thiết lập % ga tối thiểu khi đang bay";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(315, 159);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(248, 17);
            this.label8.TabIndex = 19;
            this.label8.Text = "Lưu ý: Hãy giữ chặt UAV của bạn";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(315, 180);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(448, 26);
            this.label9.TabIndex = 20;
            this.label9.Text = "Chế độ này dùng để kiểm tra các động cơ có hoạt động bình thường hay không.\nCác đ" +
    "ộng cơ sẽ được test theo thứ tự xoay chiều kim đồng hồ, bắt đầu từ góc trước bên" +
    " phải";
            // 
            // flowMotors
            // 
            this.flowMotors.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowMotors.Location = new System.Drawing.Point(14, 106);
            this.flowMotors.Name = "flowMotors";
            this.flowMotors.Size = new System.Drawing.Size(106, 310);
            this.flowMotors.TabIndex = 21;
            this.flowMotors.WrapContents = false;
            // 
            // User_Motor_Test
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnTestAllMotor);
            this.Controls.Add(this.flowMotors);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.btnTestSequenceMotor);
            this.Controls.Add(this.btnStopAllMotor);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.lblClass);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numDuration);
            this.Controls.Add(this.numThrottle);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "User_Motor_Test";
            this.Size = new System.Drawing.Size(931, 436);
            ((System.ComponentModel.ISupportInitialize)(this.numThrottle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numThrottle;
        private System.Windows.Forms.NumericUpDown numDuration;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblClass;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Button btnTestSequenceMotor;
        private System.Windows.Forms.Button btnStopAllMotor;
        private System.Windows.Forms.Button btnTestAllMotor;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.FlowLayoutPanel flowMotors;
    }
}
