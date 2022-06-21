using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GrannysKitchen.Models.ResponseModels
{
    public class FoodItemsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExistingFoodImage { get; set; }
        public int CategoryId { get; set; }
        public int TotalStock { get; set; }
        public Nullable<int> AvailableStock { get; set; }
        public decimal ActualPrice { get; set; }
        public int DiscountPercentage { get; set; }
        public Nullable<decimal> DeliveryCharges { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}


