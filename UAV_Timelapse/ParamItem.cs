using System;
using System.ComponentModel;
using static MAVLink;

namespace UAV_Timelapse
{
    public class ParamItem : INotifyPropertyChanged
    {
        private string _name;
        private float _value;
        private float _default;
        private string _units;
        private string _options;
        private string _desc;
        private bool _fav;
        private bool _modified;
        private MAV_PARAM_TYPE _mavType;
        private int _index;
        private int _count;
        private string _rangeText;

        private float? _min, _max, _step;

        public float? Min
        {
            get => _min;
            set
            {
                if (_min == value) return;
                _min = value;
                OnPropertyChanged(nameof(Min));
            }
        }

        public float? Max
        {
            get => _max;
            set
            {
                if (_max == value) return;
                _max = value;
                OnPropertyChanged(nameof(Max));
            }
        }

        public float? Step
        {
            get => _step;
            set
            {
                if (_step == value) return;
                _step = value;
                OnPropertyChanged(nameof(Step));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Group));
            }
        }

        /// <summary>Giá trị hiện tại của param (FC gửi về / người dùng sửa)</summary>
        public float Value
        {
            get => _value;
            set
            {
                if (Math.Abs(_value - value) <= 1e-6f) return;

                _value = value;
                OnPropertyChanged(nameof(Value));
                Modified = true;
                OnPropertyChanged(nameof(NonDefault));
            }
        }
        public void SetFromFc(float v)
        {
            if (Math.Abs(_value - v) <= 1e-6f) return;

            _value = v;
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(NonDefault));
            // KHÔNG đụng Modified
        }

        /// <summary>Giá trị default (lấy từ metadata nếu có, nếu không thì = lần đọc đầu tiên)</summary>
        public float Default
        {
            get => _default;
            set
            {
                if (Math.Abs(_default - value) <= 1e-6f) return;

                _default = value;
                OnPropertyChanged(nameof(Default));
                OnPropertyChanged(nameof(NonDefault));
            }
        }

        /// <summary>Đơn vị (m, cm, deg/s, Hz, …)</summary>
        public string Units
        {
            get => _units;
            set
            {
                if (_units == value) return;
                _units = value;
                OnPropertyChanged(nameof(Units));
            }
        }

        /// <summary>Chuỗi options đọc từ metadata (0:Disabled,1:Enabled,…)</summary>
        public string Options
        {
            get => _options;
            set
            {
                if (_options == value) return;
                _options = value;
                OnPropertyChanged(nameof(Options));
                OnPropertyChanged(nameof(DisplayOptions));
            }
        }
        public string RangeText
        {
            get => _rangeText;
            set
            {
                if (_rangeText == value) return;
                _rangeText = value;
                OnPropertyChanged(nameof(RangeText));
                OnPropertyChanged(nameof(DisplayOptions));
            }
        }

        // Cái sẽ bind vào cột Options
        public string DisplayOptions => string.IsNullOrEmpty(Options) ? RangeText : Options;


        /// <summary>Mô tả dài của tham số</summary>
        public string Desc
        {
            get => _desc;
            set
            {
                if (_desc == value) return;
                _desc = value;
                OnPropertyChanged(nameof(Desc));
            }
        }

        public bool Fav
        {
            get => _fav;
            set
            {
                if (_fav == value) return;
                _fav = value;
                OnPropertyChanged(nameof(Fav));
            }
        }

        /// <summary>Đánh dấu là đã bị sửa so với lần đọc gần nhất</summary>
        public bool Modified
        {
            get => _modified;
            set
            {
                if (_modified == value) return;
                _modified = value;
                OnPropertyChanged(nameof(Modified));
            }
        }

        /// <summary>True nếu Value khác Default (dùng filter "None Default")</summary>
        public bool NonDefault => Math.Abs(Value - Default) > 1e-6f;

        /// <summary>Kiểu tham số MAVLink (FLOAT, INT8, INT16, …)</summary>
        public MAV_PARAM_TYPE MavType
        {
            get => _mavType;
            set => _mavType = value;
        }

        /// <summary>Index param (param_index FC gửi về)</summary>
        public int Index
        {
            get => _index;
            set => _index = value;
        }

        /// <summary>Tổng số param (param_count FC gửi về)</summary>
        public int Count
        {
            get => _count;
            set => _count = value;
        }

        /// <summary>Nhóm để hiển thị TreeView: ACRO_*, AHRS_*, …</summary>
        public string Group
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return "Other";
                int idx = Name.IndexOf('_');
                return idx > 0 ? Name.Substring(0, idx) : "Other";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
