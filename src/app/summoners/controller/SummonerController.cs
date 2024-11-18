using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.summoners.service.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace lol_check_scheduler.src.app.summoners.controller
{
    [ApiController]
    [Route("api/v1/summoners")]
    public class SummonerController : ControllerBase
    {
        private readonly ISummonerService _summonerService;

        public SummonerController(ISummonerService summonerService)
        {
            _summonerService = summonerService;
        }

        [HttpGet]
        public async Task<IEnumerable<Summoner>> Test()
        {
            return await _summonerService.GetSummonersByTopN(49);
        }
    }
}