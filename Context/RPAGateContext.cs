using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskScheduler.Entity;

namespace TaskScheduler.Context
{
    public class RPAGateContext : DbContext
    {
        public RPAGateContext(DbContextOptions<RPAGateContext> options) : base(options)
        {
        }

        public DbSet<Scheduler> Schedulers { get; set; } = null!;
        public DbSet<RpaTask> Tasks { get; set; } = null!;
        public DbSet<SystemConfig> SystemConfigs { get; set; } = null!;
    }

}
