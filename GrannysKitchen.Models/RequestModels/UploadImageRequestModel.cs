using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace GrannysKitchen.Models.RequestModels
{
    public class UploadImageRequestModel
    {
        [Display(Name = "Category Image")]
        public IFormFile CategoryImage { get; set; }
    }
}


