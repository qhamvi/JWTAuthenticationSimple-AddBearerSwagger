﻿using System;

namespace JWTAuthentication.Models
{
    public class UserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public DateTime DayJoing { get; set; } 
    }
}
