using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler.Interface
{
    public interface ITaskFunc
    {
        string Name { get; }
        Task Execute(string? wecomRobotId);
    }
}
