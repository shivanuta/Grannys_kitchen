using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrannysKitchen.Models.RequestModels
{
    public class FoodItemsRequest
    {
        public FoodItemsRequest()
        {
            this.IsActive = true;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExistingFoodImage { get; set; }
        [Display(Name = "Category Image")]
        public IFormFile FoodImage { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
