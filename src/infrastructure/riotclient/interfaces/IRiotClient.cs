namespace lol_check_scheduler.src.infrastructure.riotclient.interfaces
{
    public interface IRiotClient
    {
        Task<RiotClientData.GetSummonerAccountInfoResponse> GetSummonerAccountInfoByGameNameAndTagLine(string gameName, string tagLine);
        Task<RiotClientData.GetSummonerAccountInfoResponse> GetSummonerAccountInfoByPuuid(string puuid);

        Task<RiotClientData.CurrentGameInfo> GetCurrentGameInfo(string puuid);

        Task<RiotClientData.LeagueListDTO> GetLeagueListInChallengerLeagues();
        Task<RiotClientData.LeagueListDTO> GetLeagueListInGrandMasterLeagues();
        Task<RiotClientData.LeagueListDTO> GetLeagueListInMasterLeagues();

        Task<RiotClientData.GetSummonerInfoResponse> GetSummonerInfoBySummonerId(string summonerId);
    }
}