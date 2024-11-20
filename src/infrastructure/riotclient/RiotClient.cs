using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public RiotClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _riotApiKey = _configuration["RiotApiKey"]!;

            _httpClient = _httpClientFactory.CreateClient("RIOT_CLIENT");
        }

        public async Task<RiotClientData.GetPuuidResponse> GetPuuid(string gameName, string tagLine)
        {
            var url = $"https://asia.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{gameName}/{tagLine}?api_key={_riotApiKey}";

            var response = await _httpClient.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<RiotClientData.GetPuuidResponse>() ?? throw new Exception();
            }
            catch (Exception e)
            {
                throw new BusinessException(RiotClientErrorCode.RIOT_CLIENT_SUMMONER_NOT_FOUND, e.Message);
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