﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTOs.Auth
{

    public class LoginDto
    {
        public string Cedula { get; set; }
        public string Password { get; set; }
    }
}