using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace GrannysKitchen.Models.DBModels
{
    public class ResetPasswordTokens
    {
        public ResetPasswordTokens()
        {
            this.IsActive = true;
            this.IsChefUser = false;
            this.CreatedDate = System.DateTime.UtcNow;
        }
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string EmailId { get; set; }
        public string ResetToken { get; set; }
        public DateTime ResetTokenExpires { get; set; }
        public bool IsChefUser { get; set; }
        public bool IsActive { get; set; }
        public Nullable<DateTime> CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
    }
}