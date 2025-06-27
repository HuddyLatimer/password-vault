# Password Vault

A secure, modern password manager built with WPF and .NET 8.0. Password Vault helps you store and manage your passwords with a clean, Material Design interface.

![Password Vault Screenshot](asset/logo.png)

## Features

- 🔒 Secure password storage with encryption
- 🎨 Modern Material Design interface
- 📱 Password generator
- 📂 Category organization
- 💾 Import/Export functionality
- 🔍 Quick search
- 📎 File attachments support
- 📝 Password history tracking
- 🔐 Auto-logout for security

## System Requirements

- Windows 10 or later
- .NET 8.0 Runtime

## Installation

### Option 1: Download Release
1. Go to the [Releases](../../releases) page
2. Download the latest `PasswordVault-Setup.exe`
3. Run the installer
4. Launch Password Vault from the Start Menu

### Option 2: Build from Source
1. Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Clone the repository
3. Open the solution in Visual Studio 2022
4. Build and run the project

## Usage

1. First Launch:
   - Create a master password
   - This password will be used to encrypt all your data

2. Managing Entries:
   - Click "+" to add new entries
   - Use categories to organize entries
   - Search functionality available in the top bar
   - Right-click entries for additional options

3. Security Features:
   - Auto-logout after inactivity
   - Password generator for strong passwords
   - Encrypted storage using industry standards

## Building for Release

To create a release build:

1. Update version in `PasswordVault.csproj`
2. Build in Release mode:
   ```
   dotnet publish -c Release -r win-x64 --self-contained true
   ```
3. Find the output in `bin/Release/net8.0-windows/win-x64/publish`

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [Material Design In XAML](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
- [Entity Framework Core](https://github.com/dotnet/efcore)
- Icons from [Material Design Icons](https://materialdesignicons.com/) 