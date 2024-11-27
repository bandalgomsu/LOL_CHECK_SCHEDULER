using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using lol_check_scheduler.src.common.exception;

namespace lol_check_scheduler.src.app.summoners.exception
{
    public class SummonerErrorCode : IErrorCode
    {
        public static readonly SummonerErrorCode SUMMONER_NOT_FOUND = new SummonerErrorCode("S01", "SUMMONER_NOT_FOUND", (int)HttpStatusCode.BadRequest);

        public string Code { get; set; }

        public string Message { get; set; }

        public int Status { get; set; }

        private SummonerErrorCode(string code, string message, int status)
        {
            Code = code;
            Message = message;
            Status = status;
        }
    }
}