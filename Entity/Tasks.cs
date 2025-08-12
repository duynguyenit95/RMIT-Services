using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler.Entity
{
    [Table("Tasks")]
    public class RpaTask
    {
        [Key]
        public int Id { get; set; }
        public int? SchedulerID { get; set; }
        [MaxLength(64)]
        public string Type { get; set; } = string.Empty;
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(1024)]
        public string? Message { get; set; }
        [MaxLength(512)]
        public string? Tag { get; set; }
        [MaxLength(64)]
        public string? WecomRobotId { get; set; }
        [MaxLength(128)]
        public string? RequestUrl { get; set; }
        [MaxLength(64)]
        public string? FuncName { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime? LastRunAt { get; set; }
        public DateTime? NextRunAt { get; set; }
        public Scheduler? Scheduler { get; set; }
    }
    public class RpaTaskConfigure : IEntityTypeConfiguration<RpaTask>
    {
        public void Configure(EntityTypeBuilder<RpaTask> builder)
        {
            builder.HasOne(x => x.Scheduler).WithMany().HasForeignKey(x => x.SchedulerID);
        }
    }
}
