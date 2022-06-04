using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GrannysKitchen.Models.DBModels
{
    public class ChefUsers
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public string MobileNo { get; set; }

        [Required, DefaultValue(true)]
        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }

    }
}
