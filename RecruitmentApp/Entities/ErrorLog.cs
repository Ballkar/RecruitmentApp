using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentApp.Entities
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public int User { get; set; }
        public int StatusCode { get; set; }
        public string Request { get; set; }
        public string Method { get; set; }
        public string Headers { get; set; }
        public string PathBase { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
    }
}
