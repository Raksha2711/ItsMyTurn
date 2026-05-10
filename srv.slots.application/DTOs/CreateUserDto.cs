using System;
using System.Collections.Generic;
using System.Text;

namespace srv.slots.application.DTOs
{
    public class CreateUserDto
    {
        public string FullName { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string Password { get; set; } = string.Empty;
    }
}
