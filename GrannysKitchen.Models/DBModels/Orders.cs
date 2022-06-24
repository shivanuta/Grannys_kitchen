using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrannysKitchen.Models.DBModels
{
    public class Orders
    {
        public Orders()
        {
            this.IsDelivered = false;
            this.IsCancel = false;
        }
        [Key]
        public int Id { get; set; }
        public string OrderReferenceNo { get; set; }
        [ForeignKey("FoodItems")]
        public int FoodItemId { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public int TotalQuantity { get; set; }
        public double PriceAfterDiscount { get; set; }
        public bool IsDelivered { get; set; }
        public bool IsCancel { get; set; }

        public Nullable<int> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
