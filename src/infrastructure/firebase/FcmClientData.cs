using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.infrastructure.firebase
{
    public class FcmClientData
    {
        public class FmcMulticastMessage
        {
            public required string Title { get; set; }
            public required string Body { get; set; }
            public required IEnumerable<string> DeviceTokens { get; set; }

            public static FmcMulticastMessage CreateCheckedPlayingGameMessage(IEnumerable<string> deviceTokens, Summoner summoner)
            {
                return new FmcMulticastMessage
                {
                    Title = $"{summoner.GameName}/{summoner.TagLine}님이 게임을 시작 했습니다.",
                    Body = $"{summoner.GameName}/{summoner.TagLine}님이 게임을 시작 했습니다.",
                    DeviceTokens = deviceTokens,
                };
            }
        }
    }
}