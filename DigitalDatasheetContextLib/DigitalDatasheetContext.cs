using DigitalDatasheetEntityLib;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace DigitalDatasheetContextLib
{
    public class DigitalDatasheetContext : DbContext
    {
        public DbSet<JobForm> JobForms { get; set; }
        public DbSet<JobData> JobDataTable { get; set; }
        public DbSet<JobRequirements> JobRequirementsTable { get; set; }
        public DbSet<JobRemark> JobRemarksTable { get; set; }
        public DbSet<DocumentRemark> RemarksDocument { get; set; }
        public DbSet<JobNote> JobNotes { get; set; }
        public DbSet<SpecificationRequirements> SpecificationRequirementsTable { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = Path.Combine(@"database_location", "database_name.db");
            optionsBuilder.UseSqlite($"Filename={path}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // use FluentAPI to assign multi-column primary keys
            modelBuilder.Entity<JobForm>().HasKey(requirement => new { requirement.WorkOrderNumber, requirement.TestCondition, requirement.TestPerformedOn });
            modelBuilder.Entity<JobData>().HasKey(data => new { data.WorkOrderNumber, data.TestCondition, data.TestPerformedOn, data.StructureTitle, data.SerialNumber, data.Location });
            modelBuilder.Entity<JobRequirements>().HasKey(requirement => new { requirement.WorkOrderNumber, requirement.TestCondition, requirement.TestPerformedOn });
            modelBuilder.Entity<JobRemark>().HasKey(remark => new { remark.WorkOrderNumber, remark.TestCondition, remark.TestPerformedOn, remark.Remark });
            modelBuilder.Entity<JobNote>().HasKey(note => new { note.WorkOrderNumber, note.TestCondition, note.TestPerformedOn, note.User, note.DateAdded });
        }
    }
}
