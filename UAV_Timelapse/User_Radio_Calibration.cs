using System;
using System.Linq;
using System.Windows.Forms;

namespace UAV_Timelapse
{
    public partial class User_Radio_Calibration : UserControl
    {
        private readonly Timer uiTimer = new Timer { Interval = 33 };
        public User_Radio_Calibration()
        {
            InitializeComponent();

            uiTimer.Tick += UiTimer_Tick;
            uiTimer.Start();
        }
        private static int ClampUs(int v)
        {
            if (v == 0) return 0;     // kênh không dùng
            if (v < 1000) return 1000;
            if (v > 2000) return 2000;
            return v;
        }

        private void UiTimer_Tick(object sender, EventArgs e)
        {
            int[] rc = new int[]
            {
                TransmissionFrame.Rc_Ch1,  TransmissionFrame.Rc_Ch2,  TransmissionFrame.Rc_Ch3,  TransmissionFrame.Rc_Ch4,
                TransmissionFrame.Rc_Ch5,  TransmissionFrame.Rc_Ch6,  TransmissionFrame.Rc_Ch7,  TransmissionFrame.Rc_Ch8,
                TransmissionFrame.Rc_Ch9,  TransmissionFrame.Rc_Ch10, TransmissionFrame.Rc_Ch11, TransmissionFrame.Rc_Ch12,
                TransmissionFrame.Rc_Ch13, TransmissionFrame.Rc_Ch14, TransmissionFrame.Rc_Ch15, TransmissionFrame.Rc_Ch16
            }.Select(ClampUs).ToArray();

            RcBar[] bars = { rcBar1,rcBar2,rcBar3,rcBar4,rcBar5,rcBar6,rcBar7,rcBar8,
                     rcBar9,rcBar10,rcBar11,rcBar12,rcBar13,rcBar14,rcBar15,rcBar16 };

            Label[] labels = { lblRc1,lblRc2,lblRc3,lblRc4,lblRc5,lblRc6,lblRc7,lblRc8,
                       lblRc9,lblRc10,lblRc11,lblRc12,lblRc13,lblRc14,lblRc15,lblRc16 };

            for (int i = 0; i < 16; i++)
            {
                int v = rc[i];
                if (v == 0)
                {
                    bars[i].Value = bars[i].Minimum;  // hoặc disable nếu thích
                    labels[i].Text = $"Radio {i + 1} 0";
                }
                else
                {
                    bars[i].Value = v;                 // VẼ NGAY, KHÔNG TRƯỢT
                    labels[i].Text = $"Radio {i + 1} {v}";
                }
            }
        }

        private void btnCalibRadio_Click(object sender, EventArgs e)
        {

        }
    }
}
