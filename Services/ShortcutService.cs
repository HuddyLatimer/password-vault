using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Reflection;

namespace PasswordVault.Services
{
    [SupportedOSPlatform("windows")]
    public static class ShortcutService
    {
        public static void CreateDesktopShortcut()
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string shortcutPath = Path.Combine(desktopPath, "Password Vault.lnk");

                // Don't create if shortcut already exists
                if (File.Exists(shortcutPath))
                    return;

                // Get the path of the current executable
                string exePath = GetExecutablePath();
                if (string.IsNullOrEmpty(exePath)) return;

                string workingDir = Path.GetDirectoryName(exePath) ?? "";
                string iconPath = Path.Combine(workingDir, "asset", "logo.ico");

                // If icon doesn't exist in the working directory (development environment),
                // try to find it relative to the assembly location
                if (!File.Exists(iconPath))
                {
                    string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                    string assemblyDir = Path.GetDirectoryName(assemblyLocation) ?? "";
                    iconPath = Path.Combine(assemblyDir, "..", "..", "..", "asset", "logo.ico");
                    iconPath = Path.GetFullPath(iconPath);
                }

                // Ensure paths don't have any spaces
                exePath = $"\"{exePath}\"";
                workingDir = $"\"{workingDir}\"";
                iconPath = $"\"{iconPath}\"";

                // Create PowerShell script to create shortcut
                string script = $@"
                    $WScriptShell = New-Object -ComObject WScript.Shell
                    $Shortcut = $WScriptShell.CreateShortcut('{shortcutPath}')
                    $Shortcut.TargetPath = {exePath}
                    $Shortcut.WorkingDirectory = {workingDir}
                    $Shortcut.Description = 'Password Vault - Secure Password Manager'
                    $Shortcut.IconLocation = {iconPath}
                    $Shortcut.Save()";

                // Execute PowerShell script
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // Log any errors (you might want to handle this differently)
                if (!string.IsNullOrEmpty(error))
                {
                    File.WriteAllText("shortcut_error.log", error);
                }
            }
            catch (Exception ex)
            {
                // Log the error
                File.WriteAllText("shortcut_error.log", ex.ToString());
            }
        }

        private static string GetExecutablePath()
        {
            // Try to get the path of the running executable
            string? exePath = Process.GetCurrentProcess().MainModule?.FileName;
            
            // If we're in development (running through dotnet), find the dll
            if (exePath?.EndsWith("dotnet.exe") == true)
            {
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                string dllPath = Path.Combine(
                    Path.GetDirectoryName(assemblyLocation) ?? "",
                    "PasswordVault.dll");
                
                if (File.Exists(dllPath))
                {
                    exePath = dllPath;
                }
            }

            return exePath ?? "";
        }
    }
} 