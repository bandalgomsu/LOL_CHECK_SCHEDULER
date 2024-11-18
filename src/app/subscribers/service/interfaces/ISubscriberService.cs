using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.app.subscribers.service.interfaces
{
    public interface ISubscriberService
    {
        Task<IEnumerable<Subscriber>> GetSubscribersBySummonerId(int summonerId);
    }
}