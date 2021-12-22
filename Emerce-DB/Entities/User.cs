using System;
using System.Collections.Generic;

#nullable disable

namespace Emerce_DB.Entities
{
    public partial class User
    {
        public User()
        {
            Category = new HashSet<Category>();
            Product = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsWelcomeMailSent { get; set; }
        public DateTime Idatetime { get; set; }
        public DateTime? Udatetime { get; set; }
        public int Iuser { get; set; }
        public int? Uuser { get; set; }

        public virtual ICollection<Category> Category { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
