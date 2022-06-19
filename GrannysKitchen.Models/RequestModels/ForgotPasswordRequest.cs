using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GrannysKitchen.Models.RequestModels
{
    public class ForgotPasswordRequest
    {
        public ForgotPasswordRequest()
        {
            this.IsChefUser = false;
        }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public bool IsChefUser { get; set; }
    }
}

