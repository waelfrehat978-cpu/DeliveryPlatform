using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryPlatform.Service.Exceptions
{
    public class ValidationException: AppException
    {
        public Dictionary<string, string[]>Errors {  get; }
       public ValidationException(string field, string message) : base(400,message) {

            Errors = new Dictionary<string, string[]> {
                { field, new[]{message} }
            
            };


        }
    }
}
