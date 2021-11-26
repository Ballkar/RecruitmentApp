using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Entities
{
    public class ExperienceResume
    {
        public int ResumeId { get; set; }
        public Resume Resume { get; set; }
        public int ExperienceId { get; set; }
        public Experience Experience { get; set; }

    }
}
