using Microsoft.EntityFrameworkCore;
using NotesManager.Client.Data.Models;

namespace NotesManager.Client.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<TextFile> TextFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(builder =>
            {
                builder.ToTable("Categories");

                builder.HasKey(c => c.Id);

                builder.Property(c => c.Name).IsRequired().HasMaxLength(128);
            });

            modelBuilder.Entity<TextFile>(builder =>
            {
                builder.ToTable("TextFiles");

                builder.HasKey(t => t.Id);

                builder.Property(t => t.FileName).IsRequired().HasMaxLength(128);

                builder.Property(t => t.Content);

                builder.HasOne(t => t.Category)
                .WithMany(c => c.TextFiles)
                .HasForeignKey(t => t.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
