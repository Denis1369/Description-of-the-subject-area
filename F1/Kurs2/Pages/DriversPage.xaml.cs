using Kurs2.Custom;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Kurs2.Pages
{
    public partial class DriversPage : Page
    {
        public ObservableCollection<Driver> Drivers { get; set; }

        public DriversPage()
        {
            InitializeComponent();
            LoadDriversData();
            DataContext = this;

            AddDriverCardCtrl.Cancelled += (_, _) => HideOverlay();
            AddDriverCardCtrl.Added += (_, driver) =>
            {
                try
                {
                    App._context.Drivers.Add(driver);
                    App._context.SaveChanges();

                    Drivers.Add(driver);
                    AddDriverCardCtrl.Reset();
                    HideOverlay();
                    ShowToast("Добавлено");
                }
                catch (Exception ex)
                {
                    ShowToast($"Ошибка добавления: {ex.Message}");
                }
            };
        }

        private void LoadDriversData()
        {
            foreach (var entity in App._context.ChangeTracker.Entries().ToList())
            {
                entity.State = EntityState.Detached;
            }

            Drivers = new ObservableCollection<Driver>(App._context.Drivers.ToList());
        }

        private void Add_Click(object sender, RoutedEventArgs e) => ShowOverlay();

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            foreach (var driver in Drivers)
            {
                if (string.IsNullOrWhiteSpace(driver.LastName))
                    errors.AppendLine($"Фамилия не указана (ID: {driver.Id})");

                if (string.IsNullOrWhiteSpace(driver.FirstName))
                    errors.AppendLine($"Имя не указано (ID: {driver.Id})");

                if (driver.DateOfBirth == null)
                    errors.AppendLine($"Дата рождения не указана (ID: {driver.Id})");

                if (string.IsNullOrWhiteSpace(driver.Nationality))
                    errors.AppendLine($"Национальность не указана (ID: {driver.Id})");

                if (string.IsNullOrWhiteSpace(driver.Url))
                {
                    errors.AppendLine($"Некорректный URL (ID: {driver.Id})");
                }
            }

            if (errors.Length > 0)
            {
                ShowToast("Ошибка:\n" + errors.ToString());
                return;
            }

            try
            {
                foreach (var driver in Drivers)
                {
                    var entry = App._context.Entry(driver);
                    if (entry.State == EntityState.Detached)
                    {
                        App._context.Drivers.Attach(driver);
                    }
                    entry.State = EntityState.Modified;
                }

                App._context.SaveChanges();
                ShowToast("Сохранено");
            }
            catch (Exception ex)
            {
                ShowToast($"Ошибка сохранения: {ex.Message}");
            }
        }

        //private void Delete_Click(object sender, RoutedEventArgs e)
        //{
        //    if (DriversGrid.SelectedItem is Driver driver)
        //    {
        //        try
        //        {
        //            // Проверка связей перед удалением
        //            if (driver.RaceResults.Any() || driver.DriverConstructorAffiliations.Any())
        //            {
        //                ShowToast("Невозможно удалить: есть связанные записи");
        //                return;
        //            }

        //            App._context.Drivers.Remove(driver);
        //            App._context.SaveChanges();
        //            Drivers.Remove(driver);
        //            ShowToast("Запись удалена");
        //        }
        //        catch (Exception ex)
        //        {
        //            ShowToast($"Ошибка удаления: {ex.Message}");
        //        }
        //    }
        //}

        private void ShowOverlay()
        {
            Overlay.Visibility = Visibility.Visible;
            Overlay.BeginAnimation(OpacityProperty,
                new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3)));
        }

        private void HideOverlay()
        {
            var fade = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
            fade.Completed += (_, __) => Overlay.Visibility = Visibility.Collapsed;
            Overlay.BeginAnimation(OpacityProperty, fade);
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