using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using lol_check_scheduler.src.common.exception;
using lol_check_scheduler.src.infrastructure.firebase.interfaces;

namespace lol_check_scheduler.src.infrastructure.firebase
{
    public class FcmClient : IFcmClient
    {
        public async Task SendMulticastMessage(FcmClientData.FmcMulticastMessage message, bool dryRun = false)
        {
            var multicastMessage = new MulticastMessage
            {
                Tokens = message.DeviceTokens.ToList(),
                Notification = new Notification
                {
                    Title = message.Title,
                    Body = message.Body,
                }
            };

            await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(multicastMessage, dryRun);
        }
    }
}