using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryPlatform.Service.Exceptions
{
    public class BusinessRuleException: AppException
        
    {
      public  BusinessRuleException(string message) : base(422,message) { }
    }
}
