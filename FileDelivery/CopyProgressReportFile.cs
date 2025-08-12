using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Helper;
using TaskScheduler.Interface;

namespace TaskScheduler.FileDelivery
{
    public class CopyProgressReportFile : TaskFuncBase
    {
        public CopyProgressReportFile(FileMover fileMover) : base(fileMover) { }
        public override string Name => "CopyProgressReportFile";
        public override async Task Execute(string? wecomRobotId)
        {
            string subject = "FileMover - HR Recruitment Progress Report No4"; 
            string folderPath = "\\\\172.19.18.68\\Departments2\\HRA\\I-share\\NHÓM TUYỂN DỤNG-招聘组\\9. BÁO CÁO TỔNG HỢP  - 汇总报告";
            string destinationPath = "\\\\172.19.18.78\\ShareFolder\\HR_InternalFiles";
            string year = DateTime.Now.ToString("yyyy");
            string month = DateTime.Now.ToString("%M");

            await FileMover.MoveFile(
                "HRInternalFile",
                "ProgressReport",
                folderPath,
                destinationPath,
                new string[] { $"Năm {year}", $"Báo cáo tuyển dụng tháng {month}" },
                $"Báo cáo tiến độ",
                ".xlsx",
                wecomRobotId);
        }
    }
}
