using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
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

        private int _expectedCount = -1;
        private bool[] _receivedIndex; //đánh dấu param_index nhận được

        private Timer _retryTimer;
        private int _retryRound = 0;
        private const int MAX_RETRY_ROUNDS = 10;

        private string _currentGroup = "ALL";

        public bool IsParamSyncDone
        {
            get
            {
                if (_expectedCount <= 0) return false;
                if (_receivedIndex == null) return false;
                int got = _receivedIndex.Count(b => b);
                return got >= _expectedCount;
            }
        }

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

            btnSaveFile.Enabled = false;
            btnWriteParams.Enabled = false;

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

            // === TIMER RETRY XIN LẠI PARAM THIẾU ===
            _retryTimer = new Timer();
            _retryTimer.Interval = 1000;   // 1s/lần, muốn dày hơn thì giảm xuống 500ms
            _retryTimer.Tick += RetryTimer_Tick;

            // đăng ký nhận PARAM_VALUE từ Form_Main
            _main.OnParamValue += HandleParamValue;
        }
        private void RetryTimer_Tick(object sender, EventArgs e)
        {
            // 1s vừa trôi qua mà KHÔNG có PARAM_VALUE mới => kết thúc một "đợt"
            _retryTimer.Stop();

            if (IsParamSyncDone)
                return;

            if (_retryRound >= MAX_RETRY_ROUNDS)
                return;

            _retryRound++;

            // xin lại các tham số còn thiếu (tối đa 100 index / lần)
            RequestMissingParams();

            // Sau khi gửi, FC sẽ trả PARAM_VALUE. Mỗi lần nhận được,
            // HandleParamValue sẽ tự Start lại timer.
            // Nếu FC không trả gì, 1s nữa timer lại Tick → retry tiếp (tối đa 10 lần).
            _retryTimer.Start();
        }

        private void HandleParamValue(mavlink_param_value_t p)
        {
            if (!IsHandleCreated || IsDisposed) return;

            string name = Encoding.ASCII
                          .GetString(p.param_id)
                          .TrimEnd('\0');

            float value = p.param_value;
            var type = (MAV_PARAM_TYPE)p.param_type;
            int index = p.param_index;
            int count = p.param_count;

            // ====== LẦN ĐẦU NHẬN ĐƯỢC param_count ======
            if (_expectedCount < 0 && count > 0)
            {
                _expectedCount = count;
                _receivedIndex = new bool[count];   // mặc định false

                // reset trạng thái đếm & retry
                _retryRound = 0;
                btnSaveFile.Enabled = false;

                _retryTimer.Stop();
                _retryTimer.Start();   // bắt đầu auto retry cho đến khi đủ
            }

            // Đánh dấu index đã nhận
            if (_receivedIndex != null &&
                index >= 0 && index < _expectedCount)
            {
                _receivedIndex[index] = true;
            }

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

                item.SetFromFc(value);
                _allParams.Add(item);
                AddGroupNodeIfNeeded(item.Group);
            }
            else
            {
                existing.SetFromFc(value);
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
            // (tuỳ chọn) hiển thị progress lên caption / label
            if (_expectedCount > 0)
            {
                int got = _receivedIndex?.Count(b => b) ?? _allParams.Count;
                //this.BeginInvoke(new Action(() =>
                //{
                    lblParamProgress.Text = $"Tham số: {got}/{_expectedCount}"; // nếu bạn có label
                //}));
            }
            if (IsParamSyncDone)
            {
                //this.BeginInvoke(new Action(() =>
                //{
                    _retryTimer.Stop();
                    btnSaveFile.Enabled = true;   // đã đủ, bật nút xuất file
                    UpdateWriteButtonState();
                //}));
            }
            else
            {
                // chưa đủ → reset idle timer.
                // nếu 1s tới không có param mới thì RetryTimer_Tick sẽ xin thêm.
                //this.BeginInvoke(new Action(() =>
                //{
                    btnSaveFile.Enabled = false;  // đang sync, khoá nút Save
                    btnWriteParams.Enabled = false;
                    _retryTimer.Stop();
                    _retryTimer.Start();
                //}));
            }
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
            _currentGroup = e.Node.Name;   // lưu group đang chọn
            ApplyFilter();                 // áp dụng filter (group + search)
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

            //Update tt param
            UpdateWriteButtonState();
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

            //Update tt param
            UpdateWriteButtonState();
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

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            if (!IsParamSyncDone)
            {
                //btnSaveFile.Enabled = false;
                //gửi lại yêu cầu tham số
                RequestMissingParams();
                return;
            }

            if (_allParams == null || _allParams.Count == 0)
            {
                MessageBox.Show("Chưa có tham số nào để lưu.", "Thông báo");
                return;
            }

            using (var dlg = new SaveFileDialog())
            {
                dlg.Filter = "ArduPilot param (*.param)|*.param|All files (*.*)|*.*";
                dlg.FileName = "copter.param";

                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    using (var sw = new StreamWriter(dlg.FileName, false, Encoding.ASCII))
                    {
                        // Ghi từng param, sắp xếp theo tên
                        foreach (var p in _allParams.OrderBy(x => x.Name))
                        {
                            string valStr = p.Value.ToString(CultureInfo.InvariantCulture);
                            sw.WriteLine("{0},{1}", p.Name, valStr);
                        }
                    }

                    MessageBox.Show("Đã lưu tham số ra file:\n" + dlg.FileName,
                                    "Save to file", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu file: " + ex.Message);
                }
            }
        }
        private void RequestMissingParams()
        {
            if (_expectedCount <= 0 || _receivedIndex == null)
            {
                //MessageBox.Show("Chưa biết tổng số tham số (param_count).", "Thông báo");
                return;
            }

            var missing = new List<int>();
            for (int i = 0; i < _expectedCount; i++)
            {
                if (!_receivedIndex[i])
                    missing.Add(i);
            }

            if (missing.Count == 0)
            {
                //MessageBox.Show("Không còn tham số nào bị thiếu.", "Thông báo");
                return;
            }

            // Có thể giới hạn theo batch để khỏi spam FC
            // ví dụ: chỉ gửi lại tối đa 50 cái một lần
            const int MAX_PER_BATCH = 100;
            var batch = missing.Take(MAX_PER_BATCH).ToList();

            foreach (int idx in batch)
            {
                _main.RequestParamByIndex((short)idx);
            }

            //MessageBox.Show($"Đã gửi yêu cầu đọc lại {batch.Count} tham số bị thiếu.\n" +
            //                "Đợi FC trả về rồi hãy bấm lưu file.",
            //                "Request missing params");
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "ArduPilot param (*.param;*.parm)|*.param;*.parm|All files (*.*)|*.*";
                if (dlg.ShowDialog() != DialogResult.OK) return;

                var lines = File.ReadAllLines(dlg.FileName);
                int applied = 0;

                foreach (var raw in lines)
                {
                    var line = raw.Trim();
                    if (string.IsNullOrEmpty(line)) continue;
                    if (line.StartsWith("#") || line.StartsWith(";")) continue;

                    // chấp nhận "NAME,VALUE" hoặc "NAME VALUE"
                    var parts = line.Split(new[] { ',', ' ', '\t' },
                                           StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length < 2) continue;

                    string name = parts[0].Trim();
                    if (!float.TryParse(parts[1], NumberStyles.Any,
                                        CultureInfo.InvariantCulture, out float val))
                        continue;

                    var p = _allParams.FirstOrDefault(x => x.Name == name);
                    if (p == null) continue; // tham số không tồn tại trên FC hiện tại

                    p.Value = val;      // ParamItem setter tự đánh dấu Modified nếu bạn có
                    applied++;
                }

                // refresh grid nếu cần
                gridParams.Refresh();

                //Update tt param
                UpdateWriteButtonState();

                MessageBox.Show($"Đã nạp {applied} tham số từ file.",
                                "Load from file");
            }
        }

        private void btnWriteParams_Click(object sender, EventArgs e)
        {
            // Lọc các tham số đã sửa
            var changed = _allParams.Where(p => p.Modified).ToList();
            if (changed.Count == 0)
            {
                // Không có gì để ghi
                btnWriteParams.Enabled = false;
                return;
            }

            // Gửi PARAM_SET cho từng param đã sửa
            foreach (var p in changed)
            {
                _main.SendParamSet(p);
            }

            // Sau khi gửi xong, clear cờ Modified
            foreach (var p in changed)
            {
                p.Modified = false;
            }

            // refresh hiển thị cột Modified / màu sắc nếu bạn dùng
            gridParams.Refresh();

            // cập nhật lại trạng thái nút
            UpdateWriteButtonState();

            MessageBox.Show($"Đã gửi {changed.Count} tham số lên FC.", "Write Params");
        }
        private void UpdateWriteButtonState()
        {
            // Chỉ bật khi đã sync xong (tuỳ bạn, có thể bỏ điều kiện IsParamSyncDone)
            bool hasModified = _allParams.Any(p => p.Modified);
            btnWriteParams.Enabled = hasModified && IsParamSyncDone;
        }

        private void btnCompareParam_Click(object sender, EventArgs e)
        {
            if (_allParams == null || _allParams.Count == 0)
            {
                MessageBox.Show("Chưa đọc tham số từ FC.", "Compare Params");
                return;
            }

            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "ArduPilot param (*.param;*.parm)|*.param;*.parm|All files (*.*)|*.*";
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                var fileDict = LoadParamFileToDict(dlg.FileName);
                if (fileDict.Count == 0)
                {
                    MessageBox.Show("File không có tham số hợp lệ.", "Compare Params");
                    return;
                }

                var diffs = BuildDiffList(fileDict);

                if (diffs.Count == 0)
                {
                    MessageBox.Show("Tất cả tham số giống nhau.", "Compare Params");
                    return;
                }

                var form = new Form_ParamCompare(diffs);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    // Nếu sau này bạn muốn “áp dụng” các param được tick ApplyFromFile
                    var toApply = form.Diffs
                                      .Where(d => d.ApplyFromFile &&
                                                  d.FileValue.HasValue)
                                      .ToList();

                    if (toApply.Count > 0)
                    {
                        foreach (var d in toApply)
                        {
                            var p = _allParams.FirstOrDefault(x =>
                                x.Name.Equals(d.Name, StringComparison.OrdinalIgnoreCase));
                            if (p != null)
                            {
                                p.Value = d.FileValue.Value;  // đánh dấu Modified luôn
                            }
                        }

                        gridParams.Refresh();
                        UpdateWriteButtonState(); // bật nút Write Params
                        MessageBox.Show($"Đã áp dụng {toApply.Count} tham số từ file.",
                                        "Compare Params");
                    }
                }
            }
        }
        private BindingList<ParamDiffItem> BuildDiffList(Dictionary<string, float> fileParams)
        {
            var diffs = new BindingList<ParamDiffItem>();

            // current = param đang có trong FC
            var currentDict = _allParams.ToDictionary(p => p.Name,
                                                      p => p.Value,
                                                      StringComparer.OrdinalIgnoreCase);

            // 1) Duyệt các param đang có trong FC
            foreach (var kv in currentDict)
            {
                string name = kv.Key;
                float curVal = kv.Value;

                if (fileParams.TryGetValue(name, out float fileVal))
                {
                    // Có cả 2 phía
                    if (Math.Abs(curVal - fileVal) > 1e-6f)
                    {
                        diffs.Add(new ParamDiffItem
                        {
                            Name = name,
                            CurrentValue = curVal,
                            FileValue = fileVal,
                            DiffType = ParamDiffType.Different,
                            ApplyFromFile = true   // mặc định tick để tiện
                        });
                    }

                    // Đã xử lý, bỏ khỏi dict file để tí nữa còn lại chỉ là OnlyInFile
                    fileParams.Remove(name);
                }
                else
                {
                    // Chỉ có trong FC
                    diffs.Add(new ParamDiffItem
                    {
                        Name = name,
                        CurrentValue = curVal,
                        FileValue = null,
                        DiffType = ParamDiffType.OnlyInFc,
                        ApplyFromFile = false
                    });
                }
            }

            // 2) Những param chỉ có trong file (fileParams còn lại)
            foreach (var kv in fileParams)
            {
                diffs.Add(new ParamDiffItem
                {
                    Name = kv.Key,
                    CurrentValue = null,
                    FileValue = kv.Value,
                    DiffType = ParamDiffType.OnlyInFile,
                    ApplyFromFile = false
                });
            }

            return diffs;
        }

        private Dictionary<string, float> LoadParamFileToDict(string filePath)
        {
            var dict = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);

            if (!File.Exists(filePath)) return dict;

            var lines = File.ReadAllLines(filePath);

            foreach (var raw in lines)
            {
                var line = raw.Trim();
                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith("#") || line.StartsWith(";")) continue;

                // chấp nhận "NAME,VALUE" hoặc "NAME VALUE"
                var parts = line.Split(new[] { ',', ' ', '\t' },
                                       StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) continue;

                string name = parts[0].Trim();
                if (!float.TryParse(parts[1],
                                    NumberStyles.Any,
                                    CultureInfo.InvariantCulture,
                                    out float val))
                    continue;

                dict[name] = val;
            }

            return dict;
        }

        private void btnRefreshParam_Click(object sender, EventArgs e)
        {
            _retryTimer.Stop();
            _retryRound = 0;
            _expectedCount = -1;
            _receivedIndex = null;

            //Xoá danh sách tham số hiện có
            _allParams.Clear();
            _view.DataSource = _allParams;

            // reset TreeView (giữ node ALL)
            var root = tvFullParam.Nodes["ALL"];
            if (root != null)
                root.Nodes.Clear();

            lblParamProgress.Text = "Tham số: 0/0";

            // khoá các nút Save / Write cho tới khi sync xong
            btnSaveFile.Enabled = false;
            btnWriteParams.Enabled = false;

            //Gửi yêu cầu đọc lại toàn bộ param từ FC
            _main.RequestAllParams();
        }
        private void ApplyFilter()
        {
            // group hiện tại
            string group = _currentGroup;
            // từ khóa đang gõ
            string keyword = txtSearch.Text.Trim();

            IEnumerable<ParamItem> query = _allParams;

            // lọc theo group (ACRO, AHRS, v.v.)
            if (group != "ALL")
                query = query.Where(p => p.Group == group);

            // lọc theo từ khóa (tên hoặc mô tả)
            if (!string.IsNullOrEmpty(keyword))
            {
                string k = keyword.ToUpperInvariant();
                query = query.Where(p =>
                    (!string.IsNullOrEmpty(p.Name) &&
                     p.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    ||
                    (!string.IsNullOrEmpty(p.Desc) &&
                     p.Desc.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                );
            }

            // đổ lại vào view
            var list = new BindingList<ParamItem>(query.OrderBy(p => p.Name).ToList());
            _view.DataSource = list;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }
    }
}
