using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.FileDelivery;

namespace TaskScheduler.Interface
{
    public abstract class TaskFuncBase : ITaskFunc
    {
        protected readonly FileMover FileMover;

        protected TaskFuncBase(FileMover fileMover)
        {
            FileMover = fileMover;
        }

        public abstract string Name { get; }

        public abstract Task Execute(string? wecomRobotId);
    }

}
