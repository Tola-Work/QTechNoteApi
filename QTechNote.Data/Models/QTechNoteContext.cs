using Microsoft.EntityFrameworkCore;

namespace QTechNote.Data.Models;

public class QTechNoteContext : DbContext
{
    public QTechNoteContext()
    {
    }

    public QTechNoteContext(DbContextOptions<QTechNoteContext> options)
        : base(options)
    {
    }

    public DbSet<AuthToken> AuthTokens { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Note> Notes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthToken>(entity =>
        {
            entity.HasKey(e => e.TokenId);

            entity.Property(e => e.AccessToken)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(e => e.RefreshToken)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(e => e.AccessTokenExpiry)
                .IsRequired();

            entity.Property(e => e.RefreshTokenExpiry)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(false);

            entity.HasOne(d => d.User)
                .WithMany(p => p.AuthTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.HasIndex(e => e.Username)
                .IsUnique();

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Email)
                .HasMaxLength(100);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(false);
        });


        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.NoteId);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Content)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Notes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}
