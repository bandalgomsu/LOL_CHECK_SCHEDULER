using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;

namespace lol_check_scheduler.src.infrastructure.firebase
{
    public class FcmClient
    {
        public async void SendMulticastMessage(FcmClientData.FmcMulticastMessage message)
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

            await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(multicastMessage);
        }

        public async Task SendMulticastMessage(FcmClientData.FmcMulticastMessage message, bool dryRun)
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