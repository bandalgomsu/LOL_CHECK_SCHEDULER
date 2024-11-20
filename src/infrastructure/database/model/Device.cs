using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.infrastructure.database.model
{
    public class Device : BaseEntity
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("device_token")]
        [Required]
        public string? DeviceToken { get; set; }

        [Column("user_id")]
        [Required]
        public long UserId { get; set; }
    }
}