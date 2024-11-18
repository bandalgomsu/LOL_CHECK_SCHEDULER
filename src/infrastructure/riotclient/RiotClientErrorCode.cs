using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using lol_check_scheduler.src.common.exception;

namespace lol_check_scheduler.src.infrastructure.riotclient
{
    public class RiotClientErrorCode : IErrorCode
    {
        public static readonly RiotClientErrorCode RIOT_CLIENT_SUMMONER_NOT_FOUND = new RiotClientErrorCode("RC01", "RIOT_CLIENT_SUMMONER_NOT_FOUND", (int)HttpStatusCode.BadRequest);

        public string Code { get; set; }

        public string Message { get; set; }

        public int Status { get; set; }

        private RiotClientErrorCode(string code, string message, int status)
        {
            Code = code;
            Message = message;
            Status = status;
        }
    }
}