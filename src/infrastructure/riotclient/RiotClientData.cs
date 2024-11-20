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
            public int GameId { get; set; } = 0;
            public string? GameType { get; set; }
            public string? GameMode { get; set; }
            public bool IsCurrentPlayingGame { get; set; } = true;
        }
    }
}