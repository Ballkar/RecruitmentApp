using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
    }
}
