using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.subscribers.repository.interfaces;
using lol_check_scheduler.src.app.subscribers.service.interfaces;

namespace lol_check_scheduler.src.app.subscribers.service
{
    public class SubscriberService(ISubscriberRepository subscriberRepository) : ISubscriberService
    {
        private readonly ISubscriberRepository _subscriberRepository = subscriberRepository;

        public async Task<IEnumerable<Subscriber>> GetSubscribersBySummonerId(int summonerId)
        {
            return await _subscriberRepository.FindAllByCondition(subscriber => subscriber.SummonerId == summonerId);
        }
    }
}