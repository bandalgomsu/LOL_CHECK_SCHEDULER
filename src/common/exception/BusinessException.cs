using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lol_check_scheduler.src.common.exception
{
    public class BusinessException : ApplicationException
    {
        public IErrorCode ErrorCode { get; }

        public BusinessException(IErrorCode errorCode) : base(errorCode.Message)
        {
            ErrorCode = errorCode;
        }

        public BusinessException(IErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}