using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.common.exception
{
    public class ErrorResponse
    {
        public required string Message { get; set; }
        public required int Status { get; set; }
        public required string Code { get; set; }
    }
}