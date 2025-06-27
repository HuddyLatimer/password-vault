using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace PasswordVault.Views
{
    public partial class MessageDialog : Window, INotifyPropertyChanged
    {
        private string _title = string.Empty;
        private string _message = string.Empty;
        private PackIconKind _iconKind;
        private Brush _iconBrush = Brushes.Black;
        private string _primaryButtonText = "OK";
        private string _secondaryButtonText = "Cancel";
        private Visibility _secondaryButtonVisibility = Visibility.Collapsed;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));
            }
        }

        public PackIconKind IconKind
        {
            get => _iconKind;
            set
            {
                _iconKind = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconKind)));
            }
        }

        public Brush IconBrush
        {
            get => _iconBrush;
            set
            {
                _iconBrush = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconBrush)));
            }
        }

        public string PrimaryButtonText
        {
            get => _primaryButtonText;
            set
            {
                _primaryButtonText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PrimaryButtonText)));
            }
        }

        public string SecondaryButtonText
        {
            get => _secondaryButtonText;
            set
            {
                _secondaryButtonText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SecondaryButtonText)));
            }
        }

        public Visibility SecondaryButtonVisibility
        {
            get => _secondaryButtonVisibility;
            set
            {
                _secondaryButtonVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SecondaryButtonVisibility)));
            }
        }

        public MessageDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnPrimaryButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnSecondaryButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public static MessageDialogResult Show(
            Window owner,
            string message,
            string title = "",
            MessageDialogType type = MessageDialogType.Information)
        {
            var dialog = new MessageDialog
            {
                Owner = owner,
                Message = message,
                Title = title
            };

            switch (type)
            {
                case MessageDialogType.Information:
                    dialog.IconKind = PackIconKind.Information;
                    dialog.IconBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3")); // Bright Blue
                    dialog.PrimaryButtonText = "OK";
                    dialog.SecondaryButtonVisibility = Visibility.Collapsed;
                    break;

                case MessageDialogType.Success:
                    dialog.IconKind = PackIconKind.CheckCircle;
                    dialog.IconBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00C853")); // Bright Green
                    dialog.PrimaryButtonText = "OK";
                    dialog.SecondaryButtonVisibility = Visibility.Collapsed;
                    break;

                case MessageDialogType.Warning:
                    dialog.IconKind = PackIconKind.Alert;
                    dialog.IconBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9100")); // Bright Orange
                    dialog.PrimaryButtonText = "OK";
                    dialog.SecondaryButtonVisibility = Visibility.Collapsed;
                    break;

                case MessageDialogType.Error:
                    dialog.IconKind = PackIconKind.AlertCircle;
                    dialog.IconBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1744")); // Bright Red
                    dialog.PrimaryButtonText = "OK";
                    dialog.SecondaryButtonVisibility = Visibility.Collapsed;
                    break;

                case MessageDialogType.Question:
                    dialog.IconKind = PackIconKind.HelpCircle;
                    dialog.IconBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#651FFF")); // Bright Purple
                    dialog.PrimaryButtonText = "Yes";
                    dialog.SecondaryButtonText = "No";
                    dialog.SecondaryButtonVisibility = Visibility.Visible;
                    break;
            }

            return dialog.ShowDialog() == true ? MessageDialogResult.Primary : MessageDialogResult.Secondary;
        }
    }

    public enum MessageDialogType
    {
        Information,
        Success,
        Warning,
        Error,
        Question
    }

    public enum MessageDialogResult
    {
        Primary,
        Secondary
    }
} 