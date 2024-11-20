using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.infrastructure.riotclient.interfaces
{
    public interface IRiotClient
    {
        Task<RiotClientData.GetPuuidResponse> GetPuuid(string gameName, string tagLine);
        Task<RiotClientData.CurrentGameInfo> GetCurrentGameInfo(string puuid);
    }
}