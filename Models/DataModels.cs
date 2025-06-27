using System;

namespace PasswordVault.Models
{
    public class PasswordHistory
    {
        public int Id { get; set; }
        public int VaultEntryId { get; set; }
        public required string EncryptedPassword { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? ChangeReason { get; set; }
    }

    public class FileAttachment
    {
        public int Id { get; set; }
        public int VaultEntryId { get; set; }
        public required string FileName { get; set; }
        public required string EncryptedContent { get; set; }
        public long FileSize { get; set; }
        public string? ContentType { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class AuditLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public required string Action { get; set; }
        public string? Details { get; set; }
        public int? RelatedEntryId { get; set; }
    }

    public enum EntryType
    {
        Login,
        SecureNote,
        CreditCard,
        Identity
    }
}
