using System;
using System.Collections.Generic;
using System.Text;

namespace srv.slots.domain.Entities
{
    public class User
    {
        public string UserId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "client";
    }

}


