using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter.Xml;
using dotenv.net;
using lol_check_scheduler.src.common.exception;
using lol_check_scheduler.src.infrastructure.riotclient.interfaces;

namespace lol_check_scheduler.src.infrastructure.riotclient
{
    public class RiotClient : IRiotClient
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
                if (e is HttpRequestException && response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new BusinessException(RiotClientErrorCode.RIOT_CLIENT_SUMMONER_NOT_FOUND, e.Message);
                }
                else if (e is HttpRequestException)
                {
                    throw new BusinessException(RiotClientErrorCode.RIOT_CLIENT_EXTERNAL_ERROR, e.Message);
                }

                throw new BusinessException(CommonErrorCode.INTERNAL_SERVER_ERROR, e.Message);
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
            catch (Exception e)
            {
                if (e is HttpRequestException && response.StatusCode == HttpStatusCode.NotFound)
                {
                    return new RiotClientData.CurrentGameInfo
                    {
                        IsCurrentPlayingGame = false
                    };
                }
                else if (e is HttpRequestException)
                {
                    throw new BusinessException(RiotClientErrorCode.RIOT_CLIENT_EXTERNAL_ERROR, e.Message);
                }

                throw new BusinessException(CommonErrorCode.INTERNAL_SERVER_ERROR, e.Message);
            }
        }

        public async Task<RiotClientData.LeagueListDTO> GetLeagueListInChallengerLeagues()
        {
            var url = $"https://kr.api.riotgames.com/lol/league/v4/challengerleagues/by-queue/RANKED_SOLO_5x5?api_key={_riotApiKey}";

            var response = await _httpClient.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<RiotClientData.LeagueListDTO>(jsonResponse) ?? throw new Exception();
            }
            catch (Exception e)
            {
                if (e is HttpRequestException)
                {
                    throw new BusinessException(RiotClientErrorCode.RIOT_CLIENT_EXTERNAL_ERROR, e.Message);
                }

                throw new BusinessException(CommonErrorCode.INTERNAL_SERVER_ERROR, e.Message);
            }
        }

        public async Task<RiotClientData.LeagueListDTO> GetLeagueListInGrandMasterLeagues()
        {
            var url = $"https://kr.api.riotgames.com/lol/league/v4/grandmasterleagues/by-queue/RANKED_SOLO_5x5?api_key={_riotApiKey}";

            var response = await _httpClient.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<RiotClientData.LeagueListDTO>(jsonResponse) ?? throw new Exception();
            }
            catch (Exception e)
            {
                if (e is HttpRequestException)
                {
                    throw new BusinessException(RiotClientErrorCode.RIOT_CLIENT_EXTERNAL_ERROR, e.Message);
                }

                throw new BusinessException(CommonErrorCode.INTERNAL_SERVER_ERROR, e.Message);
            }
        }

        public async Task<RiotClientData.LeagueListDTO> GetLeagueListInMasterLeagues()
        {
            var url = $"https://kr.api.riotgames.com/lol/league/v4/masterleagues/by-queue/RANKED_SOLO_5x5?api_key={_riotApiKey}";

            var response = await _httpClient.GetAsync(url);

            try
            {
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<RiotClientData.LeagueListDTO>(jsonResponse) ?? throw new Exception();
            }
            catch (Exception e)
            {
                if (e is HttpRequestException)
                {
                    throw new BusinessException(RiotClientErrorCode.RIOT_CLIENT_EXTERNAL_ERROR, e.Message);
                }

                throw new BusinessException(CommonErrorCode.INTERNAL_SERVER_ERROR, e.Message);
            }
        }
    }
}