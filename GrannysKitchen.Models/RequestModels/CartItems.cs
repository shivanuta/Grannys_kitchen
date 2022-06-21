using GrannysKitchen.Models.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GrannysKitchen.Models.RequestModels
{
    public class CartItems
    {
        public FoodItemsResponse FoodItemsResponse { get; set; }
        public int Quantity { get; set; }
    }
}

