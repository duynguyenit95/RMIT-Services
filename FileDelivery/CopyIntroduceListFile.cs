using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Helper;
using TaskScheduler.Interface;

namespace TaskScheduler.FileDelivery
{
    public class CopyIntroduceListFile : TaskFuncBase
    {
        public CopyIntroduceListFile(FileMover fileMover) : base(fileMover) { }
        public override string Name => "CopyIntroduceListFile";

        public override async Task Execute(string? wecomRobotId)
        {
            string folderPath = "\\\\172.19.18.68\\Departments2\\HRA\\I-share\\NHÓM TUYỂN DỤNG-招聘组\\9. BÁO CÁO TỔNG HỢP  - 汇总报告";//\\Năm 2025 - 2025年\\Báo cáo tuyển dụng tháng 6 - 6月份\\7. Tổng hợp danh sách giới thiệu - 推荐明细.xlsx";
            string destinationPath = "\\\\172.19.18.78\\ShareFolder\\HR_InternalFiles";

            string year = DateTime.Now.ToString("yyyy");
            string month = DateTime.Now.ToString("%M");

            await FileMover.MoveFile(
                "HRInternalFile",
                "IntroduceListFile",
                folderPath,
                destinationPath,
                new string[] { $"Năm {year}", $"Báo cáo tuyển dụng tháng {month}" },
                "Tổng hợp danh sách giới thiệu",
                ".xlsx",
                wecomRobotId);
        }
    }
}
