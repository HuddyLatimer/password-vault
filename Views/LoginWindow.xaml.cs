using Microsoft.EntityFrameworkCore;
using PasswordVault.Data;
using PasswordVault.Models;
using PasswordVault.Services;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace PasswordVault.Views
{
    public partial class LoginWindow : Window
    {
        private readonly VaultContext _context;
        private readonly ToastService _toastService;

        public LoginWindow()
        {
            InitializeComponent();
            _context = new VaultContext();
            _toastService = new ToastService(this);
            InitializeDatabase();
        }

        private async void InitializeDatabase()
        {
            try
            {
                // Check if database needs migration
                bool hasPendingMigrations = (await _context.Database.GetPendingMigrationsAsync()).Any();
                if (hasPendingMigrations)
                {
                    await _context.Database.MigrateAsync();
                }
                else
                {
                    // Ensure database exists without running migrations
                    await _context.Database.EnsureCreatedAsync();
                }
            }
            catch (Exception ex)
            {
                _toastService.ShowToast($"Error initializing database: {ex.Message}", "Error", ToastType.Error);
            }
        }

        private async void OnLogin(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentMac = SecurityService.GetMacAddress();
                var settings = await _context.Settings.FirstOrDefaultAsync();
                var hashedPassword = SecurityService.HashPassword(PasswordBox.Password);

                if (settings == null)
                {
                    // First time login - create settings
                    settings = new UserSettings
                    {
                        HashedMasterPassword = hashedPassword,
                        MacAddress = currentMac,
                        LastLogin = DateTime.UtcNow
                    };
                    _context.Settings.Add(settings);
                    await _context.SaveChangesAsync();

                    // Create desktop shortcut on first run
                    try
                    {
                        ShortcutService.CreateDesktopShortcut();
                        
                        // Check if error log exists
                        if (File.Exists("shortcut_error.log"))
                        {
                            string error = File.ReadAllText("shortcut_error.log");
                            _toastService.ShowToast(
                                "The application is working, but there was an issue creating the desktop shortcut.\n" +
                                "You can manually create a shortcut to the application if needed.",
                                "Shortcut Creation Warning",
                                ToastType.Warning);
                            File.Delete("shortcut_error.log");
                        }
                    }
                    catch
                    {
                        // If shortcut creation fails, show a message but continue
                        _toastService.ShowToast(
                            "The application is working, but there was an issue creating the desktop shortcut.\n" +
                            "You can manually create a shortcut to the application if needed.",
                            "Shortcut Creation Warning",
                            ToastType.Warning);
                    }
                }
                else
                {
                    // Validate MAC address
                    if (settings.MacAddress != currentMac)
                    {
                        _toastService.ShowToast("This device is not authorized to access the vault.", "Validation Error", ToastType.Warning);
                        return;
                    }

                    // Validate password
                    if (settings.HashedMasterPassword != hashedPassword)
                    {
                        _toastService.ShowToast("Invalid master password.", "Validation Error", ToastType.Warning);
                        return;
                    }

                    settings.LastLogin = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                var mainWindow = new MainWindow(PasswordBox.Password);
                mainWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                _toastService.ShowToast("An error occurred. Please try again.", "Error", ToastType.Error);
            }
        }
    }
} 