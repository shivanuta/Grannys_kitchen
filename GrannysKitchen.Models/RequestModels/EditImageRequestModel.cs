using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GrannysKitchen.Models.RequestModels
{
    public class EditImageRequestModel : UploadImageRequestModel
    {
        public int Id { get; set; }
        public string ExistingImage { get; set; }
    }
}