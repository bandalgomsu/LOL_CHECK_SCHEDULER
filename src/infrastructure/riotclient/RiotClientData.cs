using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.infrastructure.riotclient
{
    public class RiotClientData
    {
        public class GetPuuidResponse
        {
            [JsonPropertyName("puuid")]
            public required string Puuid { get; set; }
            [JsonPropertyName("gameName")]
            public required string GameName { get; set; }
            [JsonPropertyName("tagLine")]
            public required string TagLine { get; set; }
        }

        public class CurrentGameInfo
        {
            [JsonPropertyName("gameId")]
            public long GameId { get; set; } = 0;

            [JsonPropertyName("gameType")]
            public string? GameType { get; set; }

            [JsonPropertyName("gameMode")]
            public string? GameMode { get; set; }

            public bool IsCurrentPlayingGame { get; set; } = true;
        }

        public class LeagueListDTO
        {
            [JsonPropertyName("entries")]
            public required IEnumerable<LeagueItemDTO> Entries { get; set; }
        }

        public class LeagueItemDTO
        {
            [JsonPropertyName("summonerId")]
            public required string SummonerId { get; set; }
        }
    }
}