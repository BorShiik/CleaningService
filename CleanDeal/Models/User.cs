﻿using Microsoft.AspNetCore.Identity;

namespace CleanDeal.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
