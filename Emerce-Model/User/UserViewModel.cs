using System;

namespace Emerce_Model.User
{
    public class UserViewModel : IUserModel

    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime Idatetime { get; set; }
        public DateTime? Udatetime { get; set; }
        public string Iuser { get; set; }
    }
}
