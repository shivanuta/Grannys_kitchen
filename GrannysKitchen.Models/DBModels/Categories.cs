using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrannysKitchen.Models.DBModels
{
    public class Categories
    {
        public Categories()
        {
            this.IsActive = true;
        }
        [Key]
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }
        public bool IsActive { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}
