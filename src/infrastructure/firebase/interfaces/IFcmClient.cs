using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.infrastructure.firebase.interfaces
{
    public interface IFcmClient
    {
        Task SendMulticastMessage(FcmClientData.FmcMulticastMessage message, bool dryRun = false);
    }
}