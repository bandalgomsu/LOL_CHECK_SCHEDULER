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
        [Column("puuid")]
        public string? Puuid { get; set; }

        [Required]
        [Column("game_name")]
        public string? GameName { get; set; }

        [Required]
        [Column("tag_line")]
        public string? TagLine { get; set; }

        [Column("introduce")]
        public string? Introduce { get; set; }

        [Column("recent_game_id")]
        public long? RecentGameId { get; set; } = 0;
    }
}