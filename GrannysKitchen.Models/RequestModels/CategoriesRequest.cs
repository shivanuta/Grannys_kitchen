using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrannysKitchen.Models.RequestModels
{
    public class CategoriesRequest : EditImageRequestModel
    {
        public CategoriesRequest()
        {
            this.IsActive = true;
        }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
