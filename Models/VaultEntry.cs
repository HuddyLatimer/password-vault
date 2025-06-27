using System;
using System.Collections.Generic;

namespace PasswordVault.Models
{
    public class VaultEntry
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Username { get; set; }
        public string? EncryptedPassword { get; set; }
        public string? Website { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public EntryType Type { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<FileAttachment>? Attachments { get; set; }
        public ICollection<PasswordHistory>? PasswordHistory { get; set; }
    }
} 