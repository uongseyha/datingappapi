﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppAPI.DTO
{
    public class UserRegisterRequest
    {
        [Required]
        public string userName { get; set; }
        [Required]
        public string password { get; set; }
    }
}
