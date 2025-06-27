using Microsoft.EntityFrameworkCore;
using PasswordVault.Models;
using System;
using System.IO;

namespace PasswordVault.Data
{
    public class VaultContext : DbContext
    {
        public DbSet<VaultEntry> Entries { get; set; } = null!;
        public DbSet<UserSettings> Settings { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<FileAttachment> Attachments { get; set; } = null!;
        public DbSet<PasswordHistory> PasswordHistory { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PasswordVault",
                "vault.db");

            var directory = Path.GetDirectoryName(dbPath);
            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }
            
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VaultEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired();
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Entries)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.HasOne(e => e.ParentCategory)
                    .WithMany(c => c.SubCategories)
                    .HasForeignKey(e => e.ParentCategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserSettings>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<FileAttachment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne<VaultEntry>()
                    .WithMany(v => v.Attachments)
                    .HasForeignKey(e => e.VaultEntryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PasswordHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne<VaultEntry>()
                    .WithMany(v => v.PasswordHistory)
                    .HasForeignKey(e => e.VaultEntryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AuditLog>()
                .HasKey(e => e.Id);
        }
    }
} 