﻿using Emerce_Model;

namespace Emerce_Service.User
{
    public interface IUserService
    {
        public General<Emerce_Model.User.UserCreateModel> Insert( Emerce_Model.User.UserCreateModel newUser );
        public General<Emerce_Model.User.UserLoginModel> Login( Emerce_Model.User.UserLoginModel user );
        public General<Emerce_Model.User.UserViewModel> Get();
    }
}
