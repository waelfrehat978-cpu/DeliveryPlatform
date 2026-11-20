using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryPlatform.Service.Exceptions
{
    public class NotFoundException: AppException
    {
        public NotFoundException(string message): base(404,message){ }


    }
}
