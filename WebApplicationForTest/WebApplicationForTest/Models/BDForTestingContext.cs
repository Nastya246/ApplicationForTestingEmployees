using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WebApplicationForTest.Models
{
    public class BDForTestingContext : DbContext
    {
        public DbSet<AnswerPractice> AnswerPractices { get; set; }
        public DbSet<AnswerTheory> AnswerTheories { get; set; }
        public DbSet<LogUser> LogUsers { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Practice> Practices { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<ResultTest> ResultTests { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Theory> Theories { get; set; }
        public DbSet<Topic> Topic { get; set; }
        public DbSet<Units> Unitss { get; set; }
        public DbSet<UnitsPositions> UnitsPositions { get; set; }
        public DbSet<Users> Userss { get; set; }
    }
}