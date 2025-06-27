using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MaterialDesignThemes.Wpf;

namespace PasswordVault.Views
{
    public partial class Toast : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Toast), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(Toast), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register("IconKind", typeof(PackIconKind), typeof(Toast), new PropertyMetadata(PackIconKind.Information));

        public static readonly DependencyProperty IconBrushProperty =
            DependencyProperty.Register("IconBrush", typeof(Brush), typeof(Toast), new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty IsInteractiveProperty =
            DependencyProperty.Register("IsInteractive", typeof(bool), typeof(Toast), new PropertyMetadata(false));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public PackIconKind IconKind
        {
            get => (PackIconKind)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public Brush IconBrush
        {
            get => (Brush)GetValue(IconBrushProperty);
            set => SetValue(IconBrushProperty, value);
        }

        public bool IsInteractive
        {
            get => (bool)GetValue(IsInteractiveProperty);
            set => SetValue(IsInteractiveProperty, value);
        }

        public event EventHandler<ToastResult>? ToastClosed;

        public Toast()
        {
            InitializeComponent();
        }

        public void Show()
        {
            var storyboard = (Storyboard)Resources["ShowAnimation"];
            storyboard.Begin();
        }

        public void Hide()
        {
            var storyboard = (Storyboard)Resources["HideAnimation"];
            storyboard.Completed += (s, e) => ToastClosed?.Invoke(this, ToastResult.Closed);
            storyboard.Begin();
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void OnYesClick(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)Resources["HideAnimation"];
            storyboard.Completed += (s, e) => ToastClosed?.Invoke(this, ToastResult.Yes);
            storyboard.Begin();
        }

        private void OnNoClick(object sender, RoutedEventArgs e)
        {
            var storyboard = (Storyboard)Resources["HideAnimation"];
            storyboard.Completed += (s, e) => ToastClosed?.Invoke(this, ToastResult.No);
            storyboard.Begin();
        }
    }

    public enum ToastResult
    {
        Closed,
        Yes,
        No
    }
} 