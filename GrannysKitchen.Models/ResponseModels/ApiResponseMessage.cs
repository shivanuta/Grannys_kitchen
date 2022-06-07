using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrannysKitchen.Models.ResponseModels
{
    public class ApiResponseMessage
    {
        public ApiResponseMessage()
        {
            this.IsSuccess = false;
            this.ResponseMesssage = new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.Unauthorized };
        }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public string UniqueImageName { get; set; }
        public HttpResponseMessage ResponseMesssage { get; set; }
    }
}
