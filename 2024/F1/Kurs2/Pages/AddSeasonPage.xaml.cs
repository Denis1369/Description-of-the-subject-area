using Kurs2.Custom;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Kurs2.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddSeasonPage.xaml
    /// </summary>
    public partial class AddSeasonPage : Page
    {
        public AddSeasonPage()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (!int.TryParse(SeasonTextBox.Text, out int year))
            {
                ShowToast("Надо год в формате yyyy");
                return;
            }
            if (App._context.Seasons.FirstOrDefault(s => s.Year == year) != null)
            {
                ShowToast("Данный сезон уже есть");
                return;
            }
            if (year < 1950)
            {
                ShowToast("Тогда Формулы-1 еще не было");
                return;
            }
            if (year > DateTime.UtcNow.Year+100)
            {
                ShowToast("О вы из будущего");
                return;
            }

            App._context.Seasons.Add(new Season { Year = year });
            App._context.SaveChanges();
            ShowToast("Добавлено");
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
