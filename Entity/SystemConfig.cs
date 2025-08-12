using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskScheduler.Entity
{
    [Table("SystemConfig")]
    public class SystemConfig
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(64)]
        public string ConfigGroup { get; set; } = string.Empty;
        [MaxLength(32)]
        public string ConfigKey { get; set; } = string.Empty;
        [MaxLength(1024)]
        public string ConfigValue { get; set; } = string.Empty;
        public DateTimeOffset? ExpiryDate { get; set; }
        [MaxLength(16)]
        public string LastUpdatedBy { get; set; } = string.Empty;
        public DateTimeOffset LastUpdatedTime { get; set; }
    }
}
