using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Entities
{
    public class Skill
    {
        public int id { get; set; }
        public string Name { get; set; }
        public virtual List<ResumeSkill> Resumes { get; set; }

    }
}
