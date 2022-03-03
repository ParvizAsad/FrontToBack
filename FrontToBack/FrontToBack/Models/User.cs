﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FrontToBack.Models
{
    public class User :IdentityUser
    {
        [Required]
        public string FullName { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
