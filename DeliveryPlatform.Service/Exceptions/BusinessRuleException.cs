using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryPlatform.Service.Exceptions
{
    public class BusinessRuleException: Exception
        
    {
      public  BusinessRuleException(string message) : base(message) { }
    }
}
