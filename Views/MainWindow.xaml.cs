using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using PasswordVault.Data;
using PasswordVault.Models;
using PasswordVault.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PasswordVault.Views
{
    public partial class MainWindow : Window
    {
        private readonly VaultContext _context;
        private readonly SecurityService _security;
        private readonly ImportExportService _importExport;
        private readonly ToastService _toastService;
        private VaultEntry? _currentEntry;
        private bool _isEditing;

        public MainWindow(string masterPassword)
        {
            InitializeComponent();
            _context = new VaultContext();
            _security = new SecurityService(masterPassword);
            _importExport = new ImportExportService(_context, _security);
            _toastService = new ToastService(this);
            LoadEntries();
        }

        private async void LoadEntries()
        {
            var entries = await _context.Entries.OrderBy(e => e.Title).ToListAsync();
            EntriesList.ItemsSource = entries;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchBox.Text.ToLower();
            var entries = _context.Entries
                .Where(entry => entry.Title.ToLower().Contains(searchText) ||
                               entry.Username.ToLower().Contains(searchText))
                .OrderBy(e => e.Title)
                .ToList();
            EntriesList.ItemsSource = entries;
        }

        private void OnAddNewEntry(object sender, RoutedEventArgs e)
        {
            _currentEntry = new VaultEntry
            {
                Title = "",
                Username = "",
                EncryptedPassword = "",
                Website = "",
                Notes = "",
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
            _isEditing = false;

            ClearEntryFields();
            EntryDetails.Visibility = Visibility.Visible;
            WelcomeScreen.Visibility = Visibility.Collapsed;
            TitleBox.Focus();
        }

        private void OnEntrySelected(object sender, SelectionChangedEventArgs e)
        {
            if (EntriesList.SelectedItem is VaultEntry entry)
            {
                _currentEntry = entry;
                _isEditing = false;

                TitleBox.Text = entry.Title;
                UsernameBox.Text = entry.Username;
                PasswordBox.Password = _security.Decrypt(entry.EncryptedPassword);
                WebsiteBox.Text = entry.Website;
                NotesBox.Text = entry.Notes;

                EntryDetails.Visibility = Visibility.Visible;
                WelcomeScreen.Visibility = Visibility.Collapsed;
            }
        }

        private async void OnSaveChanges(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentEntry == null) return;

                if (string.IsNullOrWhiteSpace(TitleBox.Text))
                {
                    _toastService.ShowToast("Title is required.", "Validation Error", ToastType.Warning);
                    return;
                }

                _currentEntry.Title = TitleBox.Text;
                _currentEntry.Username = UsernameBox.Text;
                _currentEntry.EncryptedPassword = _security.Encrypt(PasswordBox.Password);
                _currentEntry.Website = WebsiteBox.Text;
                _currentEntry.Notes = NotesBox.Text;
                _currentEntry.ModifiedAt = DateTime.UtcNow;

                if (!_isEditing)
                {
                    _context.Entries.Add(_currentEntry);
                }

                await _context.SaveChangesAsync();
                LoadEntries();
                _toastService.ShowToast("Entry saved successfully!", "Success", ToastType.Success);
            }
            catch (Exception ex)
            {
                _toastService.ShowToast($"Error saving entry: {ex.Message}", "Error", ToastType.Error);
            }
        }

        private async void OnDeleteEntry(object sender, RoutedEventArgs e)
        {
            if (_currentEntry != null)
            {
                _toastService.ShowInteractiveToast(
                    "Are you sure you want to delete this entry?",
                    "Confirm Delete",
                    ToastType.Question,
                    async (result) =>
                    {
                        if (result == ToastResult.Yes)
                        {
                            _context.Entries.Remove(_currentEntry);
                            await _context.SaveChangesAsync();
                            LoadEntries();

                            EntryDetails.Visibility = Visibility.Collapsed;
                            WelcomeScreen.Visibility = Visibility.Visible;

                            _toastService.ShowToast("Entry deleted successfully!", "Success", ToastType.Success);
                        }
                    });
            }
        }

        private void OnEditEntry(object sender, RoutedEventArgs e)
        {
            _isEditing = true;
            TitleBox.Focus();
        }

        private void OnCopyPassword(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PasswordBox.Password))
            {
                Clipboard.SetText(PasswordBox.Password);
                _toastService.ShowToast("Password copied to clipboard!", "Success", ToastType.Success);
            }
        }

        private void OnGeneratePassword(object sender, RoutedEventArgs e)
        {
            var password = PasswordGeneratorService.GeneratePassword();
            PasswordBox.Password = password;
            _toastService.ShowToast("New password generated!", "Success", ToastType.Success);
        }

        private void OnImport(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.ContextMenu != null)
            {
                button.ContextMenu.IsOpen = true;
            }
        }

        private void OnExport(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.ContextMenu != null)
            {
                button.ContextMenu.IsOpen = true;
            }
        }

        private async void OnImportJson(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    Title = "Import passwords from JSON"
                };

                if (dialog.ShowDialog() == true)
                {
                    _toastService.ShowInteractiveToast(
                        "This will add all entries from the file to your vault. Continue?",
                        "Confirm Import",
                        ToastType.Question,
                        async (result) =>
                        {
                            if (result == ToastResult.Yes)
                            {
                                await _importExport.ImportFromJson(dialog.FileName);
                                LoadEntries();
                                _toastService.ShowToast("Passwords imported successfully!", "Success", ToastType.Success);
                            }
                        });
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowToast($"Error importing passwords: {ex.Message}", "Error", ToastType.Error);
            }
        }

        private async void OnImportCsv(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    Title = "Import passwords from CSV"
                };

                if (dialog.ShowDialog() == true)
                {
                    _toastService.ShowInteractiveToast(
                        "This will add all entries from the file to your vault. Continue?",
                        "Confirm Import",
                        ToastType.Question,
                        async (result) =>
                        {
                            if (result == ToastResult.Yes)
                            {
                                await _importExport.ImportFromCsv(dialog.FileName);
                                LoadEntries();
                                _toastService.ShowToast("Passwords imported successfully!", "Success", ToastType.Success);
                            }
                        });
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowToast($"Error importing passwords: {ex.Message}", "Error", ToastType.Error);
            }
        }

        private async void OnExportJson(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    Title = "Export passwords to JSON",
                    FileName = $"password-vault-export-{DateTime.Now:yyyy-MM-dd}.json"
                };

                if (dialog.ShowDialog() == true)
                {
                    _toastService.ShowInteractiveToast(
                        "The exported file will contain decrypted passwords. Make sure to store it securely and delete it after use. Continue?",
                        "Security Warning",
                        ToastType.Warning,
                        async (result) =>
                        {
                            if (result == ToastResult.Yes)
                            {
                                await _importExport.ExportToJson(dialog.FileName);
                                _toastService.ShowToast(
                                    "Passwords exported successfully!\nPlease store the file securely and delete it after use.",
                                    "Success",
                                    ToastType.Success);
                            }
                        });
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowToast($"Error exporting passwords: {ex.Message}", "Error", ToastType.Error);
            }
        }

        private async void OnExportCsv(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    Title = "Export passwords to CSV",
                    FileName = $"password-vault-export-{DateTime.Now:yyyy-MM-dd}.csv"
                };

                if (dialog.ShowDialog() == true)
                {
                    _toastService.ShowInteractiveToast(
                        "The exported file will contain decrypted passwords. Make sure to store it securely and delete it after use. Continue?",
                        "Security Warning",
                        ToastType.Warning,
                        async (result) =>
                        {
                            if (result == ToastResult.Yes)
                            {
                                await _importExport.ExportToCsv(dialog.FileName);
                                _toastService.ShowToast(
                                    "Passwords exported successfully!\nPlease store the file securely and delete it after use.",
                                    "Success",
                                    ToastType.Success);
                            }
                        });
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowToast($"Error exporting passwords: {ex.Message}", "Error", ToastType.Error);
            }
        }

        private void ClearEntryFields()
        {
            TitleBox.Text = string.Empty;
            UsernameBox.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            WebsiteBox.Text = string.Empty;
            NotesBox.Text = string.Empty;
        }
    }
} 