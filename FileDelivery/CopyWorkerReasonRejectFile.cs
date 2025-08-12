using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Helper;
using TaskScheduler.Interface;

namespace TaskScheduler.FileDelivery
{
    public class CopyWorkerReasonRejectFile : TaskFuncBase
    {
        public CopyWorkerReasonRejectFile(FileMover fileMover) : base(fileMover) { }
        public override string Name => "CopyWorkerReasonRejectFile";
        public override async Task Execute(string? wecomRobotId)
        {
            string folderPath = "\\\\172.19.18.68\\Departments2\\HRA\\I-share\\NHÓM TUYỂN DỤNG-招聘组";
            string destinationPath = "\\\\172.19.18.78\\ShareFolder\\HR_InternalFiles";

            string year = DateTime.Now.ToString("yyyy");
            string month = DateTime.Now.ToString("MMM");
            string day = DateTime.Now.ToString("dd");

            await FileMover.MoveFile(
                "HRInternalFile",
                "WorkerReasonReject",
                folderPath,
                destinationPath,
                new[] { "Tuyển dụng công nhân", "Danh sách nộp hồ sơ hàng ngày", year, month },
                $"List of employee on {day}",
                ".xlsx",
                wecomRobotId);
        }
    }
}
