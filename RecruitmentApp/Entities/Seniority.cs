using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Entities
{
    public class Seniority
    {
        public int id { get; set; }
        public string Name { get; set; }
        public virtual List<Resume> Resumes { get; set; } = new List<Resume>();

    }
}
