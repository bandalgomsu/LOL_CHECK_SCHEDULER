using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.infrastructure.database.model
{
    public class Summoner : BaseEntity
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        public String? Puuid { get; set; }

        [Required]
        [Column("game_name")]
        public String? GameName { get; set; }

        [Required]
        [Column("tag_line")]
        public String? TagLine { get; set; }

        public String? Introduce { get; set; }

        [Column("recent_game_id")]
        public long RecentGameId { get; set; } = 0;
    }
}