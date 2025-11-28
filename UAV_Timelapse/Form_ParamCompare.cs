using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UAV_Timelapse
{
    public partial class Form_ParamCompare : Form
    {
        private BindingList<ParamDiffItem> _diffs;

        public BindingList<ParamDiffItem> Diffs => _diffs;
        public Form_ParamCompare(BindingList<ParamDiffItem> diffs)
        {
            InitializeComponent();
            _diffs = diffs ?? new BindingList<ParamDiffItem>();

            gridDiff.AutoGenerateColumns = false;
            gridDiff.Columns.Clear();

            gridDiff.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = "Tham số",
                ReadOnly = true
            });

            gridDiff.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CurrentValue",
                HeaderText = "Giá trị FC",
                ReadOnly = true
            });

            gridDiff.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FileValue",
                HeaderText = "Giá trị file",
                ReadOnly = true
            });

            gridDiff.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DiffType",
                HeaderText = "Loại",
                ReadOnly = true
            });

            gridDiff.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "ApplyFromFile",
                HeaderText = "Áp dụng từ file?",
                ReadOnly = false
            });

            gridDiff.DataSource = _diffs;

            // tô màu dòng cho dễ nhìn
            gridDiff.RowPrePaint += (s, e) =>
            {
                var row = gridDiff.Rows[e.RowIndex];
                if (row.DataBoundItem is ParamDiffItem d)
                {
                    switch (d.DiffType)
                    {
                        case ParamDiffType.Different:
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.MistyRose;
                            break;
                        case ParamDiffType.OnlyInFc:
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                            break;
                        case ParamDiffType.OnlyInFile:
                            row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCyan;
                            break;
                    }
                }
            };
        }


        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
