using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.infrastructure.riotclient
{
    public class RiotClientData
    {
        public class GetPuuidResponse
        {
            public required string Puuid { get; set; }
            public required string GameName { get; set; }
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