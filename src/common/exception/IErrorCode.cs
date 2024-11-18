using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.common.exception
{
    public interface IErrorCode
    {
        string Code { get; }
        string Message { get; }
        int Status { get; }
    }
}