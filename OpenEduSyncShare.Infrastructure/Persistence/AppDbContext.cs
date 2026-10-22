using Microsoft.EntityFrameworkCore;
using OpenEduSyncShare.Domain.Entities;

namespace OpenEduSyncShare.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Answer> Answers => Set<Answer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.ToTable("Answers");

            entity.HasKey(e => e.QuestionId);

            entity.Property(e => e.QuestionId)
                .HasColumnName("question_id")
                .HasColumnType("text")
                .IsRequired();

            entity.Property(e => e.AnswerData)
                .HasColumnName("answer")
                .HasColumnType("jsonb")
                .IsRequired();
        });
    }
}