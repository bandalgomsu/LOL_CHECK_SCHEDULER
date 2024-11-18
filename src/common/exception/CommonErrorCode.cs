using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mysqlx;

namespace lol_check_scheduler.src.common.exception
{
    public class CommonErrorCode : IErrorCode
    {
        public static readonly CommonErrorCode INVALID_INPUT_VALUE = new CommonErrorCode("C01", "Invalid Input Value.", (int)HttpStatusCode.BadRequest);
        public static readonly CommonErrorCode METHOD_NOT_ALLOWED = new CommonErrorCode("C02", "Invalid Method Type.", (int)HttpStatusCode.MethodNotAllowed);
        public static readonly CommonErrorCode ENTITY_NOT_FOUND = new CommonErrorCode("C03", "Entity Not Found.", (int)HttpStatusCode.BadRequest);
        public static readonly CommonErrorCode INTERNAL_SERVER_ERROR = new CommonErrorCode("C04", "Internal Server Error.", (int)HttpStatusCode.InternalServerError);

        public string Code { get; }
        public string Message { get; }
        public int Status { get; }

        // Private constructor to enforce the creation of predefined error codes
        private CommonErrorCode(string code, string message, int status)
        {
            Code = code;
            Message = message;
            Status = status;
        }
    }
}