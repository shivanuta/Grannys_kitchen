using GrannysKitchen.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GrannysKitchen.Models.RequestModels
{
    public class ChefMenuRequest
    {
        public ChefMenuRequest()
        {
            this.IsActive = true;
        }
        public int Id { get; set; }
        public string ItemName { get; set; }
        public int CategoryId { get; set; }
        public DateTime MenuDate { get; set; }
        public Categories? Categories { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
