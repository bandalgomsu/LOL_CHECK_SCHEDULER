using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using lol_check_scheduler.src.infrastructure.firebase;
using Xunit;

namespace test.src.infrastructure.firebase
{
    public class FcmClientTest
    {
        public FcmClientTest()
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("../../../../google-services.json")
            });
        }
        private readonly FcmClient _fcmClient = new();

        [Fact(DisplayName = "SEND_MULTICAST_MESSAGE_SUCCESS")]
        public async void SEND_MULTICAST_MESSAGE_SUCCESS()
        {
            var message = new FcmClientData.FmcMulticastMessage
            {
                Title = "TEST",
                Body = "TEST",
                DeviceTokens = ["TEST_TOKEN"]
            };

            var exception = await Record.ExceptionAsync(() => _fcmClient.SendMulticastMessage(message, true));

            Assert.Null(exception);
        }
    }
}