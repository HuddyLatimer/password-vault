using System;

namespace PasswordVault.Models
{
    public class UserSettings
    {
        public int Id { get; set; }
        public required string HashedMasterPassword { get; set; }
        public required string MacAddress { get; set; }
        public DateTime LastLogin { get; set; }
        public bool UseBiometricAuth { get; set; }
        public int AutoLogoutMinutes { get; set; } = 5;
        public string? BiometricKeyHash { get; set; }
    }
}
