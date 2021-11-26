using RecruitmentApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Models
{
    public class ResumeDto
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string GithubUrl { get; set; }
        public GitDataDto GitData { get; set; }
        public string ImageName { get; set; }
        public string Description { get; set; }
        public int SeniorityId { get; set; }
        public SeniorityDto SeniorityDto { get; set; }
        public List<Experience> Experiences { get; set; }
        public List<Skill> Skills { get; set; }
        public int UserId { get; set; }
    }
}
