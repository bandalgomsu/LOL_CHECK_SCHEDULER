using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using dotenv.net;
using lol_check_scheduler.src.common.exception;

namespace lol_check_scheduler.src.infrastructure.riotclient
{
    public class RiotClient
    {
        private const string RIOT_GET_PUUID_URL = "https://asia.api.riotgames.com/riot/account/v1/accounts/by-riot-id/";
        private const string RIOT_CHECK_CURRENT_GAME_INFO_URL = "https://kr.api.riotgames.com/lol/spectator/v5/active-games/by-summoner/";

        private readonly string _riotApiKey;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RiotClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _riotApiKey = _configuration["RiotApiKey"]!;
        }

        public async Task<RiotClientData.GetPuuidResponse> GetPuuid(string gameName, string tagLine)
        {
            var url = $"https://asia.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{gameName}/{tagLine}?api_key={_riotApiKey}";

            var response = await _httpClient.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<RiotClientData.GetPuuidResponse>(jsonResponse) ?? throw new Exception();
            }
            catch (Exception e)
            {
                throw new BusinessException(RiotClientErrorCode.RIOT_CLIENT_SUMMONER_NOT_FOUND);
            }
        }

        public async Task<RiotClientData.CurrentGameInfo> GetCurrentGameInfo(string puuid)
        {
            var url = $"https://kr.api.riotgames.com/lol/spectator/v5/active-games/by-summoner/{puuid}?api_key={_riotApiKey}";

            var response = await _httpClient.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<RiotClientData.CurrentGameInfo>(jsonResponse) ?? throw new Exception();
            }
            catch (Exception)
            {
                return new RiotClientData.CurrentGameInfo
                {
                    IsCurrentPlayingGame = false
                };
            }
        }
    }
}