namespace GetReady.Data
{
    using GetReady.Data.Models.QuestionModels;
    using Microsoft.EntityFrameworkCore;
    using Models.UserModels;

    public class GetReadyDbContext : DbContext
    {
        public GetReadyDbContext(DbContextOptions<GetReadyDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<PersonalQuestionPackage> PersonalQuestionPackages { get; set; }

        public DbSet<GlobalQuestionPackage> GlobalQuestionPackages { get; set; }

        public DbSet<QuestionSheet> QuestionSheets { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(u =>
            {
                u.HasIndex(x => x.Username)
                .IsUnique(true);
            });

            base.OnModelCreating(builder);
        }
    }
}
