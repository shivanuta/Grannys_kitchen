using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrannysKitchen.Models.RequestModels
{
    public class OrderSummary
    {
        public List<FoodItemRequest> FoodItemRequest { get; set; }
        public double TotalPrice { get; set; }
        public int TotalQuantity { get; set; }
        public double PriceAfterDiscount { get; set; }

        public Nullable<int> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }

    public class FoodItemRequest
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }
}
