using Microsoft.EntityFrameworkCore;
using PHE.API.Models;

namespace PHE.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
            
        }

        public DbSet<Applicant> Applicants { get; set; }

        public DbSet<ApplicantsLog> ApplicantsLogs { get; set; }

        public DbSet<JEApplication> JEApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================================
            // APPLICANTS
            // =====================================================

            modelBuilder.Entity<Applicant>()
                .ToTable("Applicants");



            modelBuilder.Entity<Applicant>()
                .HasKey(x => x.Id);



            modelBuilder.Entity<Applicant>()
                .Property(x => x.Latitude)
                .HasPrecision(18, 6);


            modelBuilder.Entity<Applicant>()
                .Property(x => x.Longitude)
                .HasPrecision(18, 6);


            modelBuilder.Entity<Applicant>()
                .Property(x => x.TotalEstimateAmount)
                .HasPrecision(18, 2);


            modelBuilder.Entity<Applicant>()
                .Property(x => x.ShowAmountAsPerNoOfPlots)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Applicant>()
                .Property(x => x.AmountForPlots)
                .HasPrecision(18, 2);


            // =====================================================
            // APPLICANTS LOG
            // =====================================================

            modelBuilder.Entity<ApplicantsLog>()
    .ToTable("ApplicantsLog");

            modelBuilder.Entity<ApplicantsLog>()
                .HasKey(x => x.LogId);

            modelBuilder.Entity<ApplicantsLog>()
                .Property(x => x.Latitude)
                .HasPrecision(18, 6);

            modelBuilder.Entity<ApplicantsLog>()
                .Property(x => x.Longitude)
                .HasPrecision(18, 6);

            modelBuilder.Entity<ApplicantsLog>()
                .Property(x => x.TotalEstimateAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ApplicantsLog>()
                .Property(x => x.ShowAmountAsPerNoOfPlots)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ApplicantsLog>()
                .Property(x => x.AmountForPlots)
                .HasPrecision(18, 2);
        }
    }
}