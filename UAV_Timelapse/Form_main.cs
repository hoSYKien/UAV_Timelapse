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
    public partial class Form_Main : Form
    {
        public Form_Main()
        {
            InitializeComponent();
        }
        private void addUserControl(UserControl usercontrol)
        {
            usercontrol.AutoSize = false;               // Không để tự co dãn
            //usercontrol.Size = new Size(181, 44);      // Chọn kích thước phù hợp
            // Khoảng cách giữa các control
            //usercontrol.Dock = DockStyle.Fill;
            usercontrol.Margin = new Padding(2);
            panel7.Controls.Add(usercontrol);

            usercontrol.BringToFront();
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
        bool check_btn_Data = true;
        bool check_btn3 = true;
        private void btn_Data_Click(object sender, EventArgs e)
        {
            if (check_btn_Data)
            {
                for(int i = 0; i <150; i++)
                {
                    panel4.Size = new Size(i, 536);
                }
                check_btn_Data = false;
            }
            else
            {
                for (int i = 150; i >= 0; i--)
                {
                    panel4.Size = new Size(i, 536);
                }
                check_btn_Data = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (check_btn3)
            {
                flowLayoutPanel3.Visible = false;
                check_btn3 = false;
            }
            else
            {
                flowLayoutPanel3.Visible = true;
                check_btn3 = true;
            }
        }
    }
}
