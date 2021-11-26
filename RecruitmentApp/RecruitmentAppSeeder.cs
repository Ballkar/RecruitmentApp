using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecruitmentApp.Entities;

namespace RecruitmentApp
{
    public class RecruitmentAppSeeder
    {
        private readonly RecruitmentAppDbContext _dbContext;
        private readonly IPasswordHasher<User> _passwordHasher;

        public RecruitmentAppSeeder(RecruitmentAppDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
        }
        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                var pendingMigrations = _dbContext.Database.GetPendingMigrations();
                if (pendingMigrations != null && pendingMigrations.Any())
                {
                    _dbContext.Database.Migrate();
                }

                if (!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.Skills.Any())
                {
                    var roles = GetSkills();
                    _dbContext.Skills.AddRange(roles);
                    _dbContext.SaveChanges();
                }
                if (!_dbContext.Seniority.Any())
                {
                    var seniority = GetSeniority();
                    _dbContext.Seniority.AddRange(seniority);
                    _dbContext.SaveChanges();
                }
                // add first sample user
                var basicUserEmail = "user@sample.pl";

                if (!_dbContext.Users.Any(r => r.Email == basicUserEmail))
                {
                    var user = GetSomeUser(basicUserEmail);
                    _dbContext.Users.AddRange(user);
                    _dbContext.SaveChanges();
                }

                var basicUserId = _dbContext.Users.Where(r => r.Email == basicUserEmail).Select(d => d.Id).First();

                if (!_dbContext.Experiences.Any())
                {
                    var experience = GetSampleExperience(basicUserId);
                    _dbContext.Experiences.AddRange(experience);
                    _dbContext.SaveChanges();
                }

                if (!_dbContext.Resumes.Any(r => r.UserId == basicUserId))
                {
                    var resume = GetSampleResume(basicUserId);
                    _dbContext.Resumes.AddRange(resume);
                    _dbContext.SaveChanges();
                }
            }
        }
        private Experience GetSampleExperience(int basicUserId)
        {
            Experience usersExperiences = new Experience
            {
                Name = "Typical-company",
                Description = "My first job",
                StartDate = new DateTime(2019, 10, 12, 8, 42, 0),
                EndDate = new DateTime(2021, 10, 12, 8, 42, 0),
                UserId = basicUserId
            };
            return usersExperiences;
        }

        private List<Seniority> GetSeniority()
        {
            var seniority = new List<Seniority>()
            {
                new Seniority()
                {
                    Name = "Junior"
                },
                new Seniority()
                {
                    Name = "Mid-level"
                },
                new Seniority()
                {
                    Name = "Senior-level"
                },
            };

            return seniority;
        }




        private List<Skill> GetSampleSkills()
        {
            List<Skill> usersSampleSkills = new List<Skill>();
            for (int i = 1; i < 6; i++)
            {
                var skill = _dbContext
                .Skills
                .FirstOrDefault(r => r.id == i);

                usersSampleSkills.Add(skill);
            }
            return usersSampleSkills;
        }

        private Resume GetSampleResume(int userId)
        {
            DateTime basicUserDateOfBirth = new DateTime(1996, 10, 12, 8, 42, 0);
            var sampleSkills = GetSampleSkills();
            Resume resume = new Resume
            {
                Name = "Henryk",
                Surname = "Iksinski",
                DateOfBirth = basicUserDateOfBirth,
                City = "Kraków",
                GithubUrl = "https://api.github.com/users/bezelix",
                Description = "First basic resume for api test's",
                SeniorityId = 1,
                UserId = userId,
                Skills = new List<ResumeSkill>(),
                Experiences = new List<ExperienceResume>()
                
            };

            resume.Skills.Add(new ResumeSkill { ResumeId=1, SkillId = 2 });
            resume.Experiences.Add(new ExperienceResume { ResumeId=1, ExperienceId = 1 });
           
            return resume;
        }

        private User GetSomeUser(string email)
        {
            User user = new User()
            {
                Email = email,
                PasswordHash = "dupadupa1",
                RoleId = 1
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);
            return user;
        }

        private Skill GetSkillById(int i)
        {
            Skill skill = new Skill();
            skill = _dbContext
                .Skills
                .FirstOrDefault(r => r.id == i);
            return skill;
        }
        private Experience GetExperienceById(int i)
        {
            Experience skill = new Experience();
            skill = _dbContext
                .Experiences
                .FirstOrDefault(r => r.id == i);
            return skill;
        }


        private IEnumerable<Skill> GetSkills()
        {
            var roles = new List<Skill>()
            {
                new Skill()
                {
                    Name = "C"
                },
                new Skill()
                {
                    Name = "C++"
                },
                new Skill()
                {
                    Name = "C#"
                },
                new Skill()
                {
                    Name = "JavaScript"
                },
                new Skill()
                {
                    Name = "Java"
                },
                new Skill()
                {
                    Name = "Html"
                },
                new Skill()
                {
                    Name = "Css"
                },
                new Skill()
                {
                    Name = "Python"
                },
                new Skill()
                {
                    Name = "SQL"
                },
                new Skill()
                {
                    Name = "Entity Framework"
                },
            };
            return roles;
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = "User",
                    IsPublic = true
                },
                new Role()
                {
                    Name = "Recruiter",
                    IsPublic = true
                },
                new Role()
                {
                    Name = "Admin",
                    IsPublic = false
                },
            };

            return roles;
        }
    }
}
