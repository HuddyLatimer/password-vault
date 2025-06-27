using System.Windows;

namespace PasswordVault
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Set up exception handling
            Current.DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show(
                    $"An unexpected error occurred: {args.Exception.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                args.Handled = true;
            };
        }
    }
} 