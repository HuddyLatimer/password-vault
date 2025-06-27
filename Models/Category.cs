using System;
using System.Collections.Generic;

namespace PasswordVault.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? IconName { get; set; }
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public List<Category> SubCategories { get; set; } = new();
        public List<VaultEntry> Entries { get; set; } = new();
    }
}
