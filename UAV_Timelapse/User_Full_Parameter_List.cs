using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static MAVLink;

namespace UAV_Timelapse
{
    public partial class User_Full_Parameter_List : UserControl
    {
        private readonly Form_Main _main;

        private BindingList<ParamItem> _allParams = new BindingList<ParamItem>();
        private BindingSource _view = new BindingSource();

        // === editor phủ lên cell Value ===
        private ComboBox _editorCombo;
        private NumericUpDown _editorNumeric;

        private struct CellRef
        {
            public int Row, Col;
            public CellRef(int r, int c) { Row = r; Col = c; }
        }

        private class OptionItem
        {
            public float Value { get; set; }
            public string Text { get; set; }
            public override string ToString() => Text;  // dùng luôn Text để hiển thị
        }

        public User_Full_Parameter_List(Form_Main main)
        {
            InitializeComponent();

            _main = main;

            // cấu hình grid
            gridParams.AutoGenerateColumns = false;
            gridParams.Columns.Clear();

            gridParams.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = "Tham số",
                ReadOnly = true
            });
            gridParams.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Value",
                HeaderText = "Giá trị",
                ReadOnly = true
            });
            gridParams.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Default",
                HeaderText = "Mặc định",
                ReadOnly = true
            });
            gridParams.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Units",
                HeaderText = "Đơn vị",
                ReadOnly = true
            });
            gridParams.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Options",
                HeaderText = "Tùy chọn",
                ReadOnly = false
            });
            var colDesc = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Desc",
                HeaderText = "Mô tả",
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };

            // Cho phép xuống dòng trong ô Desc
            colDesc.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            // (tuỳ chọn) căn trái trên cho dễ đọc
            colDesc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;

            colDesc.DefaultCellStyle.Font = new Font(
                gridParams.Font.FontFamily,
                8.0f,                      // cỡ chữ mong muốn
                gridParams.Font.Style
);
            gridParams.Columns.Add(colDesc);
            // Tự tăng chiều cao dòng theo nội dung (để thấy hết mô tả)
            gridParams.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            // Dòng mẫu ban đầu, để không bị quá thấp
            gridParams.RowTemplate.Height = 40;

            _view.DataSource = _allParams;
            gridParams.DataSource = _view;

            // TreeView gốc
            tvFullParam.Nodes.Clear();
            tvFullParam.Nodes.Add("ALL", "Tất cả");
            tvFullParam.Sorted = true;

            // === TẠO COMBO & NUMERIC EDITOR ===
            _editorCombo = new ComboBox
            {
                Visible = false,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _editorCombo.SelectedIndexChanged += EditorCombo_SelectedIndexChanged;
            gridParams.Controls.Add(_editorCombo);

            _editorNumeric = new NumericUpDown
            {
                Visible = false,
                DecimalPlaces = 3,
                Minimum = -100000,
                Maximum = 100000
            };
            _editorNumeric.Leave += EditorNumeric_Leave;
            _editorNumeric.KeyDown += EditorNumeric_KeyDown;
            gridParams.Controls.Add(_editorNumeric);

            // đăng ký event của grid
            gridParams.CellBeginEdit += gridParams_CellBeginEdit;
            gridParams.CellEndEdit += (s, e) => HideEditors();
            gridParams.Scroll += (s, e) => HideEditors();
            gridParams.CurrentCellChanged += (s, e) => HideEditors();

            gridParams.CellFormatting += gridParams_CellFormatting;

            // đăng ký nhận PARAM_VALUE từ Form_Main
            _main.OnParamValue += HandleParamValue;
        }

        private void HandleParamValue(mavlink_param_value_t p)
        {
            string name = Encoding.ASCII
                          .GetString(p.param_id)
                          .TrimEnd('\0');

            float value = p.param_value;
            var type = (MAV_PARAM_TYPE)p.param_type;
            int index = p.param_index;
            int count = p.param_count;

            var existing = _allParams.FirstOrDefault(x => x.Name == name);
            if (existing == null)
            {
                // lấy metadata nếu có
                var meta = ParamMetaStore.Get(name);

                var item = new ParamItem
                {
                    Name = name,
                    Value = value,
                    // nếu trong pdef có Default thì dùng, không có thì = lần đọc đầu
                    Default = meta?.Default ?? value,
                    Units = meta?.Units ?? "",
                    Options = meta?.Values ?? "",
                    Desc = meta?.Description ?? "",
                    MavType = type,
                    Index = index,
                    Count = count,

                    Min = meta?.Min,
                    Max = meta?.Max,
                    Step = meta?.Increment,

                    RangeText = meta?.RangeText ?? ""
                };

                _allParams.Add(item);
                AddGroupNodeIfNeeded(item.Group);
            }
            else
            {
                existing.Value = value;
                existing.Index = index;
                existing.Count = count;

                // nếu lúc đầu chưa có metadata thì có thể update thêm (tuỳ thích)
                var meta = ParamMetaStore.Get(name);
                if (meta != null)
                {
                    if (string.IsNullOrEmpty(existing.Units)) existing.Units = meta.Units;
                    if (string.IsNullOrEmpty(existing.Options)) existing.Options = meta.Values;
                    if (string.IsNullOrEmpty(existing.Desc)) existing.Desc = meta.Description;
                    if (string.IsNullOrEmpty(existing.RangeText) && !string.IsNullOrEmpty(meta.RangeText))
                        existing.RangeText = meta.RangeText;
                    if (!existing.Min.HasValue && meta.Min.HasValue) existing.Min = meta.Min;
                    if (!existing.Max.HasValue && meta.Max.HasValue) existing.Max = meta.Max;
                    if (!existing.Step.HasValue && meta.Increment.HasValue) existing.Step = meta.Increment;
                    if (Math.Abs(existing.Default - existing.Value) < 1e-6f && meta.Default.HasValue)
                        existing.Default = meta.Default.Value;
                }
            }

            // có thể update progress ở đây
        }


        private void AddGroupNodeIfNeeded(string group)
        {
            var root = tvFullParam.Nodes["ALL"];
            if (root == null) return;

            bool exists = root.Nodes.Cast<TreeNode>()
                            .Any(n => n.Name == group);
            if (!exists)
            {
                root.Nodes.Add(group, group);
            }
        }

        private void tvFullParam_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string group = e.Node.Name;

            if (group == "ALL")
            {
                var sortedAll = new BindingList<ParamItem>(
                    _allParams.OrderBy(p => p.Name).ToList()
                );
                _view.DataSource = sortedAll;
            }
            else
            {
                var filtered = new BindingList<ParamItem>(
                    _allParams
                        .Where(p => p.Group == group)
                        .OrderBy(p => p.Name)            // sắp xếp theo tên
                        .ToList()
                );
                _view.DataSource = filtered;
            }
        }
        private void HideEditors()
        {
            _editorCombo.Visible = false;
            _editorNumeric.Visible = false;
        }
        private void gridParams_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // chỉ xử lý khi click vào cột Options
            var col = gridParams.Columns[e.ColumnIndex];
            if (col.DataPropertyName != "Options")
                return;

            var row = gridParams.Rows[e.RowIndex];
            var item = row.DataBoundItem as ParamItem;
            if (item == null)
                return;

            // hủy editor mặc định của DataGridView, mình tự vẽ
            e.Cancel = true;
            HideEditors();

            Rectangle rect = gridParams.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

            if (!string.IsNullOrWhiteSpace(item.Options))
            {
                // ----- có Options → dùng ComboBox -----
                var opts = ParseOptions(item.Options);
                if (opts.Count == 0)
                    return;

                _editorCombo.Items.Clear();
                foreach (var op in opts)
                    _editorCombo.Items.Add(op);

                // chọn mục trùng với Value hiện tại
                OptionItem selected = null;
                foreach (var op in opts)
                {
                    if (Math.Abs(op.Value - item.Value) < 1e-4f)
                    {
                        selected = op;
                        break;
                    }
                }
                _editorCombo.SelectedItem = selected ?? opts[0];

                _editorCombo.Tag = new CellRef(e.RowIndex, e.ColumnIndex);
                _editorCombo.Bounds = rect;
                _editorCombo.Visible = true;
                _editorCombo.BringToFront();
                _editorCombo.Focus();
            }
            else
            {
                // ----- không có Options → NumericUpDown -----
                decimal min = -100000m;
                decimal max = 100000m;
                decimal step = 0.001m;     // default
                int decimals = 3;

                if (item.Min.HasValue) min = (decimal)item.Min.Value;
                if (item.Max.HasValue) max = (decimal)item.Max.Value;
                if (item.Step.HasValue && item.Step.Value > 0)
                {
                    step = (decimal)item.Step.Value;
                    decimals = GetDecimalPlaces(step);
                }

                _editorNumeric.Minimum = min;
                _editorNumeric.Maximum = max;
                _editorNumeric.Increment = step;
                _editorNumeric.DecimalPlaces = decimals;

                _editorNumeric.Tag = new CellRef(e.RowIndex, e.ColumnIndex);

                decimal val = (decimal)item.Value;
                if (val < min) val = min;
                if (val > max) val = max;

                _editorNumeric.Value = val;
                _editorNumeric.Bounds = rect;
                _editorNumeric.Visible = true;
                _editorNumeric.BringToFront();
                _editorNumeric.Focus();
                _editorNumeric.Select(0, _editorNumeric.Text.Length);
            }
        }
        private List<OptionItem> ParseOptions(string options)
        {
            var list = new List<OptionItem>();
            if (string.IsNullOrWhiteSpace(options)) return list;

            // chấp nhận cả dấu phẩy và dấu cách làm separator
            var parts = options.Split(new[] { ',', ' ' },
                                      StringSplitOptions.RemoveEmptyEntries);

            foreach (var p in parts)
            {
                var kv = p.Split(':');
                if (kv.Length < 2) continue;

                if (float.TryParse(kv[0], NumberStyles.Any,
                                   CultureInfo.InvariantCulture, out float val))
                {
                    string text = string.Join(":", kv.Skip(1)); // đề phòng label có dấu ':'
                    list.Add(new OptionItem { Value = val, Text = text });
                }
            }

            return list;
        }
        private void EditorCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_editorCombo.Visible) return;
            if (!(_editorCombo.SelectedItem is OptionItem opt)) return;
            if (!(_editorCombo.Tag is CellRef cr)) return;

            var row = gridParams.Rows[cr.Row];
            var item = row.DataBoundItem as ParamItem;
            if (item == null) return;

            // cập nhật Value (ParamItem + cell)
            item.Value = opt.Value;
            //gridParams[cr.Col, cr.Row].Value = item.Value;

            // đánh dấu đã sửa (ParamItem.Modified đã tự set trong setter Value)
            HideEditors();
        }
        private void EditorNumeric_Leave(object sender, EventArgs e)
        {
            CommitNumeric();
        }

        private void EditorNumeric_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                CommitNumeric();
                gridParams.Focus();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                HideEditors();
            }
        }

        private void CommitNumeric()
        {
            if (!_editorNumeric.Visible) return;
            if (!(_editorNumeric.Tag is CellRef cr)) { HideEditors(); return; }

            var row = gridParams.Rows[cr.Row];
            var item = row.DataBoundItem as ParamItem;
            if (item == null) { HideEditors(); return; }

            item.Value = (float)_editorNumeric.Value;
            //gridParams[cr.Col, cr.Row].Value = item.Value;

            HideEditors();
        }
        private int GetDecimalPlaces(decimal step)
        {
            step = Math.Abs(step);
            if (step == 0) return 3;

            int decimals = 0;
            while (decimals < 6 && step != Math.Truncate(step))
            {
                step *= 10;
                decimals++;
            }
            if (decimals == 0) decimals = 1;   // cho đẹp, không bị 0.0
            return decimals;
        }
        private void gridParams_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var col = gridParams.Columns[e.ColumnIndex];
            if (col.DataPropertyName != "Options") return;
            if (e.RowIndex < 0) return;

            var item = gridParams.Rows[e.RowIndex].DataBoundItem as ParamItem;
            if (item == null) return;

            // Nếu có Options rời rạc → hiển thị options
            // Nếu không có → hiển thị RangeText (ví dụ "0 1")
            e.Value = string.IsNullOrEmpty(item.Options)
                      ? item.RangeText
                      : item.Options;

            e.FormattingApplied = true;
        }

        private void gridParams_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
