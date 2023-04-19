using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class LoginDto
    {
        [Required]
        [RegularExpression(@"\S+")]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}