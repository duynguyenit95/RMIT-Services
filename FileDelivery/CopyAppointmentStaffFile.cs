using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Helper;
using TaskScheduler.Interface;

namespace TaskScheduler.FileDelivery
{
    public class CopyAppointmentStaffFile : TaskFuncBase
    {
        public CopyAppointmentStaffFile(FileMover fileMover) : base(fileMover) { }
        public override string Name => "CopyAppointmentStaffFile";
        public override async Task Execute(string? wecomRobotId)
        {
            string folderPath = "\\\\172.19.18.68\\Departments2\\HRA\\I-share\\NHÓM TUYỂN DỤNG-招聘组";
            string destinationPath = "\\\\172.19.18.78\\ShareFolder\\HR_InternalFiles";
            string year = DateTime.Now.ToString("yyyy");

            
            await FileMover.MoveFile(
                "HRInternalFile",
                "AppointmentStaff",
                folderPath,
                destinationPath,
                new string[] { "Tuyển dụng nhân viên", "tuyển dụng hàng ngày", year },
                $"Lịch hẹn",
                ".xlsx",
                wecomRobotId);
        }
    }
}
