using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using PasswordVault.Views;

namespace PasswordVault.Services
{
    public class ToastService
    {
        private readonly Window _window;
        private readonly StackPanel _container;
        private readonly Dictionary<Toast, DispatcherTimer> _timers = new();

        public ToastService(Window window)
        {
            _window = window;
            
            // Create container for toasts
            _container = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 0, 16, 16)
            };

            // Add container to window
            Grid.SetRowSpan(_container, 999);
            Grid.SetColumnSpan(_container, 999);
            if (_window.Content is Grid grid)
            {
                grid.Children.Add(_container);
            }
        }

        public void ShowToast(string message, string title = "", ToastType type = ToastType.Information, int durationMs = 3000)
        {
            var toast = new Toast
            {
                Title = title,
                Message = message,
                IsInteractive = false
            };

            ConfigureToast(toast, type);
            ShowToastInternal(toast, durationMs);
        }

        public void ShowInteractiveToast(string message, string title = "", ToastType type = ToastType.Question, Action<ToastResult>? callback = null)
        {
            var toast = new Toast
            {
                Title = title,
                Message = message,
                IsInteractive = true
            };

            ConfigureToast(toast, type);
            toast.ToastClosed += (s, result) =>
            {
                callback?.Invoke(result);
                RemoveToast(toast);
            };

            ShowToastInternal(toast);
        }

        private void ShowToastInternal(Toast toast, int durationMs = 0)
        {
            _container.Children.Insert(0, toast);
            toast.Show();

            if (durationMs > 0)
            {
                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(durationMs)
                };

                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    toast.Hide();
                };

                _timers[toast] = timer;
                timer.Start();

                toast.ToastClosed += (s, result) => RemoveToast(toast);
            }
        }

        private void RemoveToast(Toast toast)
        {
            if (_timers.ContainsKey(toast))
            {
                _timers[toast].Stop();
                _timers.Remove(toast);
            }

            _container.Children.Remove(toast);
        }

        private void ConfigureToast(Toast toast, ToastType type)
        {
            switch (type)
            {
                case ToastType.Information:
                    toast.IconKind = PackIconKind.Information;
                    toast.IconBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2196F3"));
                    break;

                case ToastType.Success:
                    toast.IconKind = PackIconKind.CheckCircle;
                    toast.IconBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00C853"));
                    break;

                case ToastType.Warning:
                    toast.IconKind = PackIconKind.Alert;
                    toast.IconBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF9100"));
                    break;

                case ToastType.Error:
                    toast.IconKind = PackIconKind.AlertCircle;
                    toast.IconBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1744"));
                    break;

                case ToastType.Question:
                    toast.IconKind = PackIconKind.HelpCircle;
                    toast.IconBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#651FFF"));
                    break;
            }
        }
    }

    public enum ToastType
    {
        Information,
        Success,
        Warning,
        Error,
        Question
    }
} 