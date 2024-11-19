using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.subscribers.repository.interfaces;
using lol_check_scheduler.src.app.subscribers.service;
using Moq;
using Xunit;

namespace test.src.app.subscribers
{
    public class SubscriberServiceTest
    {
        private readonly Mock<ISubscriberRepository> _subscriberRepository = new Mock<ISubscriberRepository>();
        private readonly SubscriberService _subscriberService;

        public SubscriberServiceTest()
        {
            _subscriberService = new SubscriberService(_subscriberRepository.Object);
        }


        [Fact(DisplayName = "GET_SUBSCRIBERS_BY_SUMMONER_ID_SUCCESS")]
        public async Task GET_SUBSCRIBERS_BY_SUMMONER_ID_SUCCESS()
        {
            var summonerId = 1L;
            var subscriberId = 2L;
            _subscriberRepository.Setup(repo => repo.FindAllByCondition(subscriber => subscriber.SummonerId == summonerId))
                .ReturnsAsync([new Subscriber { SummonerId = summonerId, SubscriberId = subscriberId }]);

            var response = await _subscriberService.GetSubscribersBySummonerId(summonerId);

            Assert.Single(response);
            Assert.Equal(summonerId, response.First().SummonerId);
            Assert.Equal(subscriberId, response.First().SubscriberId);
        }
    }
}