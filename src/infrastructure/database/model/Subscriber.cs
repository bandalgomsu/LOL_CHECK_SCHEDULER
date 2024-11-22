using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.infrastructure.database.model
{
    [Table("Summoner_Subscriber")]
    public class Subscriber : BaseEntity
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("summoner_id")]
        public long SummonerId { get; set; }

        [Required]
        [Column("subscriber_id")]
        public long SubscriberId { get; set; }
    }
}