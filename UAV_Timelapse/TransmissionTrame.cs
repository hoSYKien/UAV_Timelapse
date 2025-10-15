using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAV_Timelapse
{
    internal class TransmissionTrame
    {
        int len; //lenth pay load
        bool inc;
        bool cmp;
        int seq; // stt gói tin
        int sys_ID; // ID Hệ thống
        int comp_ID; // ID thiết bị
        int msg_ID;
        string pay_Load;
        int check_sum;
    }
}
