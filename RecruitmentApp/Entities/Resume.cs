using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Entities
{
    public class Resume
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string GithubUrl { get; set; }
        public string ImageName { get; set; }
        public string Description { get; set; }
        public int SeniorityId { get; set; }
        public virtual Seniority Seniority { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual List<ResumeSkill> Skills { get; set; }
        public virtual List<ExperienceResume> Experiences { get; set; } 


    }
}
