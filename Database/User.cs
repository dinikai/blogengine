﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogEngine.Database
{
    internal class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Nickname { get; set; }
        public string Password { get; set; }
    }
}
