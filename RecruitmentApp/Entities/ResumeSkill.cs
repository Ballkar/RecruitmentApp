using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Entities
{
    public class ResumeSkill
    {
        public int ResumeId { get; set; }
        public Resume Resume { get; set; }
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}
