namespace UAV_Timelapse
{
    partial class User_Full_Parameter_List
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
            if (disposing)
            {
                if (_main != null)
                    _main.OnParamValue -= HandleParamValue;

                if (components != null)
                {
                    components.Dispose();
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.tvFullParam = new System.Windows.Forms.TreeView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.gridParams = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblParamProgress = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnWriteParams = new System.Windows.Forms.Button();
            this.btnCompareParam = new System.Windows.Forms.Button();
            this.btnRefreshParam = new System.Windows.Forms.Button();
            this.btnSaveFile = new System.Windows.Forms.Button();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridParams)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tvFullParam);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(145, 549);
            this.panel1.TabIndex = 0;
            // 
            // tvFullParam
            // 
            this.tvFullParam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFullParam.Location = new System.Drawing.Point(0, 0);
            this.tvFullParam.Name = "tvFullParam";
            this.tvFullParam.Size = new System.Drawing.Size(145, 549);
            this.tvFullParam.TabIndex = 0;
            this.tvFullParam.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFullParam_AfterSelect);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gridParams);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(145, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(469, 549);
            this.panel2.TabIndex = 1;
            // 
            // gridParams
            // 
            this.gridParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridParams.Location = new System.Drawing.Point(0, 0);
            this.gridParams.Name = "gridParams";
            this.gridParams.Size = new System.Drawing.Size(469, 549);
            this.gridParams.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblParamProgress);
            this.panel3.Controls.Add(this.checkBox2);
            this.panel3.Controls.Add(this.checkBox1);
            this.panel3.Controls.Add(this.txtSearch);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.button7);
            this.panel3.Controls.Add(this.button6);
            this.panel3.Controls.Add(this.comboBox1);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.btnWriteParams);
            this.panel3.Controls.Add(this.btnCompareParam);
            this.panel3.Controls.Add(this.btnRefreshParam);
            this.panel3.Controls.Add(this.btnSaveFile);
            this.panel3.Controls.Add(this.btnLoadFile);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(614, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(147, 549);
            this.panel3.TabIndex = 2;
            // 
            // lblParamProgress
            // 
            this.lblParamProgress.AutoSize = true;
            this.lblParamProgress.Location = new System.Drawing.Point(1, 437);
            this.lblParamProgress.Name = "lblParamProgress";
            this.lblParamProgress.Size = new System.Drawing.Size(60, 13);
            this.lblParamProgress.TabIndex = 13;
            this.lblParamProgress.Text = "Tham số: 0";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(4, 386);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(98, 17);
            this.checkBox2.TabIndex = 12;
            this.checkBox2.Text = "Khác mặc định";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(4, 363);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(89, 17);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "Đã chỉnh sửa";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(4, 337);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(139, 20);
            this.txtSearch.TabIndex = 10;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 318);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "Tìm kiếm";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(4, 290);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(140, 25);
            this.button7.TabIndex = 8;
            this.button7.Text = "Khôi phục mặc định";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(4, 259);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(140, 25);
            this.button6.TabIndex = 7;
            this.button6.Text = "Tải cấu hình mẫu";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "3DR_Iris+_AC34.param"});
            this.comboBox1.Location = new System.Drawing.Point(3, 232);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(140, 21);
            this.comboBox1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(5, 195);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 34);
            this.label1.TabIndex = 5;
            this.label1.Text = "Tất cả giá trị dùng đơn vị thô, không scale";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnWriteParams
            // 
            this.btnWriteParams.Location = new System.Drawing.Point(4, 105);
            this.btnWriteParams.Name = "btnWriteParams";
            this.btnWriteParams.Size = new System.Drawing.Size(140, 25);
            this.btnWriteParams.TabIndex = 4;
            this.btnWriteParams.Text = "Ghi tham số";
            this.btnWriteParams.UseVisualStyleBackColor = true;
            this.btnWriteParams.Click += new System.EventHandler(this.btnWriteParams_Click);
            // 
            // btnCompareParam
            // 
            this.btnCompareParam.Location = new System.Drawing.Point(4, 167);
            this.btnCompareParam.Name = "btnCompareParam";
            this.btnCompareParam.Size = new System.Drawing.Size(140, 25);
            this.btnCompareParam.TabIndex = 3;
            this.btnCompareParam.Text = "So sánh tham số";
            this.btnCompareParam.UseVisualStyleBackColor = true;
            this.btnCompareParam.Click += new System.EventHandler(this.btnCompareParam_Click);
            // 
            // btnRefreshParam
            // 
            this.btnRefreshParam.Location = new System.Drawing.Point(4, 136);
            this.btnRefreshParam.Name = "btnRefreshParam";
            this.btnRefreshParam.Size = new System.Drawing.Size(140, 25);
            this.btnRefreshParam.TabIndex = 2;
            this.btnRefreshParam.Text = "Làm mới tham số";
            this.btnRefreshParam.UseVisualStyleBackColor = true;
            this.btnRefreshParam.Click += new System.EventHandler(this.btnRefreshParam_Click);
            // 
            // btnSaveFile
            // 
            this.btnSaveFile.Location = new System.Drawing.Point(3, 34);
            this.btnSaveFile.Name = "btnSaveFile";
            this.btnSaveFile.Size = new System.Drawing.Size(140, 25);
            this.btnSaveFile.TabIndex = 1;
            this.btnSaveFile.Text = "Xuất file tham số";
            this.btnSaveFile.UseVisualStyleBackColor = true;
            this.btnSaveFile.Click += new System.EventHandler(this.btnSaveFile_Click);
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.Location = new System.Drawing.Point(3, 3);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(140, 25);
            this.btnLoadFile.TabIndex = 0;
            this.btnLoadFile.Text = "Nạp tham số từ file";
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // User_Full_Parameter_List
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Name = "User_Full_Parameter_List";
            this.Size = new System.Drawing.Size(761, 549);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridParams)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TreeView tvFullParam;
        private System.Windows.Forms.DataGridView gridParams;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnWriteParams;
        private System.Windows.Forms.Button btnCompareParam;
        private System.Windows.Forms.Button btnRefreshParam;
        private System.Windows.Forms.Button btnSaveFile;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblParamProgress;
    }
}
