using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using TheCCPConnection.Models;

namespace TheCCPConnection.DAL
{
    public class RequestContext : DbContext
    {
        public RequestContext() : base("RequestContext")
        {
        }

        public DbSet<Advisor> Advisors { get; set; }
        public DbSet<AdvisorDecision> AdvisorDecisions { get; set; }
        public DbSet<AdvisorSchool> AdvisorSchools { get; set; }
        public DbSet<Counselor> Counselors { get; set; }
        public DbSet<CounselorDecision> CounselorDecisions { get; set; }
        public DbSet<CounselorSchool> CounselorSchools { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Decider> Deciders { get; set; }
        public DbSet<MaxCredit> MaxCredits { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<ParentDecision> ParentDecisions { get; set; }
        public DbSet<ParentPIN> ParentPINs { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestWindow> RequestWindows { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCancel> StudentCancels { get; set; }
        public DbSet<StudentSchool> StudentSchools { get; set; }
        public DbSet<Term> Terms { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // establish keys for each join table
            modelBuilder.Entity<AdvisorSchool>()
                .HasKey(a => new { a.AdvisorID, a.SchoolID });

            modelBuilder.Entity<CounselorSchool>()
                .HasKey(c => new { c.CounselorID, c.SchoolID });

            modelBuilder.Entity<StudentSchool>()
                .HasKey(s => new { s.StudentID, s.SchoolID });

            // disable cascade delete to avoid foreign key constraint
            modelBuilder.Entity<StudentCancel>()
                .HasOptional<Student>(c => c.Student)
                .WithMany()
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<AdvisorDecision>()
            //    .HasOptional<Advisor>(c => c.Advisor)
            //    .WithMany()
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<CounselorDecision>()
            //    .HasOptional<Counselor>(c => c.Counselor)
            //    .WithMany()
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<ParentDecision>()
            //    .HasOptional<Parent>(c => c.Parent)
            //    .WithMany()
            //    .WillCascadeOnDelete(false);
        }
    }
}