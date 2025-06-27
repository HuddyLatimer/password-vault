# Password Vault

A secure, modern password manager built with WPF and .NET 8.0. Password Vault helps you store and manage your passwords with a clean, Material Design interface. Your data is protected with military-grade AES-256 encryption and hardware-locked to your device's MAC address for enhanced security.

![Password Vault Screenshot](asset/logo.png)

## Showcase
https://github.com/user-attachments/assets/99a7cbfb-3a3e-4318-a9e8-8b087b761c22


*A quick demonstration of Password Vault in action*

## Features

- 🔒 Secure password storage with AES-256 encryption
- 🔐 Hardware-locked to device MAC address
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
   - Military-grade AES-256 encryption for all stored data
   - Hardware-locked to your device's MAC address for theft prevention
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


## Acknowledgments

- [Material Design In XAML](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)
- [Entity Framework Core](https://github.com/dotnet/efcore)
- Icons from [Material Design Icons](https://materialdesignicons.com/) 
