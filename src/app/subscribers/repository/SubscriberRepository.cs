using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lol_check_scheduler.src.app.subscribers.repository.interfaces;

namespace lol_check_scheduler.src.app.subscribers.repository
{
    public class SubscriberRepository(DatabaseContext databaseContext) : RepositoryBase<Subscriber>(databaseContext), ISubscriberRepository
    {
    }
}