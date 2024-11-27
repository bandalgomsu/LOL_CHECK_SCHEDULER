namespace lol_check_scheduler.src.infrastructure.riotclient.interfaces
{
    public interface IRiotClient
    {
        Task<RiotClientData.GetPuuidResponse> GetPuuid(string gameName, string tagLine);
        Task<RiotClientData.CurrentGameInfo> GetCurrentGameInfo(string puuid);

        Task<RiotClientData.LeagueListDTO> GetLeagueListInChallengerLeagues();
        Task<RiotClientData.LeagueListDTO> GetLeagueListInGrandMasterLeagues();
        Task<RiotClientData.LeagueListDTO> GetLeagueListInMasterLeagues();
    }
}