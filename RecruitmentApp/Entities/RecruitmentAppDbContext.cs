using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;


namespace RecruitmentApp.Entities
{
    public class RecruitmentAppDbContext : DbContext
    {
        public IConfiguration Configuration { get; }
        public RecruitmentAppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Resume> Resumes { get; set; }
        public virtual DbSet<Experience> Experiences { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<Seniority> Seniority { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResumeSkill>()
            .HasKey(er => new { er.ResumeId, er.SkillId });
            modelBuilder.Entity<ResumeSkill>()
                .HasOne(rs => rs.Resume)
                .WithMany(rs => rs.Skills)
                .HasForeignKey(rs => rs.ResumeId);
            modelBuilder.Entity<ResumeSkill>()
               .HasOne(rs => rs.Skill)
               .WithMany(rs => rs.Resumes)
               .HasForeignKey(rs => rs.SkillId);

            modelBuilder.Entity<ExperienceResume>()
            .HasKey(er => new { er.ResumeId, er.ExperienceId });
            modelBuilder.Entity<ExperienceResume>()
                .HasOne(er => er.Resume)
                .WithMany(er => er.Experiences)
                .HasForeignKey(er => er.ResumeId);
            modelBuilder.Entity<ExperienceResume>()
               .HasOne(er => er.Experience)
               .WithMany(er => er.Resumes)
               .HasForeignKey(er => er.ExperienceId);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
               .UseMySql(Configuration.GetConnectionString("RecruitmentAppDbConnection"), new MySqlServerVersion(new Version(10, 3, 31)))
               //.UseLazyLoadingProxies()
               .UseLoggerFactory(LoggerFactory.Create(b => b
                   .AddConsole()
                   .AddFilter(level => level >= LogLevel.Information)))
               .EnableSensitiveDataLogging()
               .EnableDetailedErrors();
        }
    }
}
