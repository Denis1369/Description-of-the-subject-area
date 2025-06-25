using Kurs2.Custom;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Kurs2.View
{
    public partial class ResultView : Window
    {
        private readonly IConfiguration _config;

        public ResultView()
        {
            InitializeComponent();
            DriversButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        }

        private void Navigate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string pageUri)
            {
                MainFrame.Navigate(new Uri(pageUri, UriKind.Relative));
            }
        }

        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            PwdTextBox.Text = PwdBox.Password;
            PwdBox.Visibility = Visibility.Collapsed;
            PwdTextBox.Visibility = Visibility.Visible;
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            PwdBox.Password = PwdTextBox.Text;
            PwdTextBox.Visibility = Visibility.Collapsed;
            PwdBox.Visibility = Visibility.Visible;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            string inputLogin = LoginTextBox.Text;
            string inputPassword = PwdBox.Password;

            if (string.IsNullOrWhiteSpace(inputLogin) || string.IsNullOrWhiteSpace(inputPassword))
            {
                ShowToast("Заполните все поля!");
                return;
            }

            string hashedLogin = HashWithSalt(inputLogin);
            string hashedPassword = HashWithSalt(inputPassword);

            string storedLoginHash = _config["Security:AdminLoginHash"];
            string storedPasswordHash = _config["Security:AdminPasswordHash"];

            if (hashedLogin == storedLoginHash && hashedPassword == storedPasswordHash)
            {
                ShowToast("Добро пожаловать!");
                DialogHost.CloseDialogCommand.Execute(true, LoginDialogHost);
                LoginButton.Visibility = Visibility.Collapsed;
                AddResultButton.Visibility = Visibility.Visible;
                AddRaceButton.Visibility = Visibility.Visible;
                AddSeasonButton.Visibility = Visibility.Visible;
                DriverConstructorButton.Visibility = Visibility.Visible;
                DriverButton.Visibility = Visibility.Visible;
                ConstructorButton.Visibility = Visibility.Visible;
                CircuitsButton.Visibility = Visibility.Visible;
            }
            else
            {
                ShowToast("Неверный логин или пароль!");
            }

            LoginTextBox.Text = "";
            PwdBox.Password = "";
            PwdTextBox.Text = "";
        }

        private string HashWithSalt(string input)
        {
            string salt = _config["Security:Salt"];
            return HashPassword(input + salt);
        }

        private static string HashPassword(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }

        public void ShowToast(string message)
        {
            var toast = new CustomNotification { DataContext = message };

            DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
            toast.BeginAnimation(OpacityProperty, fadeIn);

            Task.Delay(2000).ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
                    fadeOut.Completed += (s, e) => ToastContainer.Children.Remove(toast);
                    toast.BeginAnimation(OpacityProperty, fadeOut);
                });
            });
            ToastContainer.Children.Add(toast);
        }
    }
}