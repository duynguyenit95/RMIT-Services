using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler.Entity
{
    [Table("Scheduler")]
    public class Scheduler
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(64)]
        public string Name { get; set; } = string.Empty;
        public TimeSpan? Time { get; set; }
        public int? IntervalMinutes { get; set; }
        public int? IntervalHours { get; set; }
        [MaxLength(32)]
        public string? WeekDays { get; set; }
        [MaxLength(64)]
        public string? MonthDays { get; set; }
    }
}
