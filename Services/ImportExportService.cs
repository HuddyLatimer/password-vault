using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using PasswordVault.Models;
using PasswordVault.Data;
using Microsoft.EntityFrameworkCore;

namespace PasswordVault.Services
{
    public class ImportExportService
    {
        private readonly VaultContext _context;
        private readonly SecurityService _securityService;

        public ImportExportService(VaultContext context, SecurityService securityService)
        {
            _context = context;
            _securityService = securityService;
        }

        public class ExportEntry
        {
            public string Title { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Website { get; set; } = string.Empty;
            public string Notes { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime ModifiedAt { get; set; }
        }

        public async Task ExportToJson(string filePath)
        {
            var entries = await _context.Entries.ToListAsync();
            var exportEntries = new List<ExportEntry>();

            foreach (var entry in entries)
            {
                exportEntries.Add(new ExportEntry
                {
                    Title = entry.Title,
                    Username = entry.Username,
                    Password = _securityService.Decrypt(entry.EncryptedPassword),
                    Website = entry.Website,
                    Notes = entry.Notes,
                    CreatedAt = entry.CreatedAt,
                    ModifiedAt = entry.ModifiedAt
                });
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var jsonString = JsonSerializer.Serialize(exportEntries, options);
            await File.WriteAllTextAsync(filePath, jsonString);
        }

        public async Task ImportFromJson(string filePath)
        {
            var jsonString = await File.ReadAllTextAsync(filePath);
            var importedEntries = JsonSerializer.Deserialize<List<ExportEntry>>(jsonString);

            if (importedEntries == null)
                throw new InvalidOperationException("Failed to deserialize import file");

            foreach (var importedEntry in importedEntries)
            {
                var entry = new VaultEntry
                {
                    Title = importedEntry.Title,
                    Username = importedEntry.Username,
                    EncryptedPassword = _securityService.Encrypt(importedEntry.Password),
                    Website = importedEntry.Website,
                    Notes = importedEntry.Notes,
                    CreatedAt = importedEntry.CreatedAt,
                    ModifiedAt = importedEntry.ModifiedAt
                };

                await _context.Entries.AddAsync(entry);
            }

            await _context.SaveChangesAsync();
        }

        public async Task ExportToCsv(string filePath)
        {
            var entries = await _context.Entries.ToListAsync();
            var lines = new List<string>
            {
                "Title,Username,Password,Website,Notes,CreatedAt,ModifiedAt"
            };

            foreach (var entry in entries)
            {
                var line = string.Join(",",
                    EscapeCsvField(entry.Title),
                    EscapeCsvField(entry.Username),
                    EscapeCsvField(_securityService.Decrypt(entry.EncryptedPassword)),
                    EscapeCsvField(entry.Website),
                    EscapeCsvField(entry.Notes),
                    entry.CreatedAt.ToString("o"),
                    entry.ModifiedAt.ToString("o"));
                lines.Add(line);
            }

            await File.WriteAllLinesAsync(filePath, lines);
        }

        public async Task ImportFromCsv(string filePath)
        {
            var lines = await File.ReadAllLinesAsync(filePath);
            
            // Skip header
            for (int i = 1; i < lines.Length; i++)
            {
                var fields = ParseCsvLine(lines[i]);
                if (fields.Count < 7) continue;

                var entry = new VaultEntry
                {
                    Title = fields[0],
                    Username = fields[1],
                    EncryptedPassword = _securityService.Encrypt(fields[2]),
                    Website = fields[3],
                    Notes = fields[4],
                    CreatedAt = DateTime.Parse(fields[5]),
                    ModifiedAt = DateTime.Parse(fields[6])
                };

                await _context.Entries.AddAsync(entry);
            }

            await _context.SaveChangesAsync();
        }

        private string EscapeCsvField(string field)
        {
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }

        private List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var currentField = new System.Text.StringBuilder();
            bool inQuotes = false;
            
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '\"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '\"')
                    {
                        currentField.Append('\"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (line[i] == ',' && !inQuotes)
                {
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(line[i]);
                }
            }
            
            fields.Add(currentField.ToString());
            return fields;
        }
    }
} 