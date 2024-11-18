using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.infrastructure.database.model
{
    public class Device
    {
        [Key]
        [Column("id")]
        public int? Id { get; set; }

        [Column("device_token")]
        [Required]
        public required string DeviceToken { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("user_id")]
        [Required]
        public required int UserId { get; set; }
    }
}