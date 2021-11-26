using RecruitmentApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Models
{
    public class CreateResumeDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string GithubUrl { get; set; }
        public string Description { get; set; }
        public int SeniorityId { get; set; }
        public List<int> SkillsId { get; set; }
    }
}
