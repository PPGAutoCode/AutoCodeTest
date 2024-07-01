
using System;

namespace ProjectName.ControllersExceptions
{
    public class BusinessException : Exception
    {
        public string ErrorCode { get; }
        
        public BusinessException(string code, string message) : base(message)
        {
            ErrorCode = code;
        }
    }

    public class TechnicalException : Exception
    {
        public string ErrorCode { get; }
        
        public TechnicalException(string code, string message) : base(message)
        {
            ErrorCode = code;
        }
    }
}
