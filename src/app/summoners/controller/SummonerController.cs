// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using lol_check_scheduler.src.app.summoners.service.interfaces;
// using lol_check_scheduler.src.infrastructure.riotclient;
// using lol_check_scheduler.src.infrastructure.riotclient.interfaces;
// using Microsoft.AspNetCore.Mvc;

// namespace lol_check_scheduler.src.app.summoners.controller
// {
//     [ApiController]
//     [Route("api/v1/summoners")]
//     public class SummonerController(ISummonerService summonerService, IRiotClient riotClient) : ControllerBase
//     {
//         private readonly ISummonerService _summonerService = summonerService;
//         private readonly IRiotClient _riotClient = riotClient;

//         [HttpGet]
//         public async Task<RiotClientData.GetPuuidResponse> Test()
//         {
//             return await _riotClient.GetPuuid("반달곰수", "KR1");
//         }
//     }
// }