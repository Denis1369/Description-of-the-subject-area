using Kurs2.Custom;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Kurs2.Pages
{
    public partial class ConstructorsPage : Page
    {
        public ObservableCollection<Constructor> Constructors { get; set; }

        public ConstructorsPage()
        {
            InitializeComponent();
            LoadConstructorsData();
            DataContext = this;

            AddConstructorCardCtrl.Cancelled += (_, _) => HideOverlay();
            AddConstructorCardCtrl.Added += (_, args) =>
            {
                try
                {
                    Constructors.Add(args.Constructor);
                    App._context.Constructors.Add(args.Constructor);
                    App._context.SaveChanges();

                    HideOverlay();
                    ShowToast("Добавлено");
                }
                catch (Exception ex)
                {
                    ShowToast($"Ошибка добавления: {ex.Message}");
                }
            };
        }

        private void LoadConstructorsData()
        {
            foreach (var entity in App._context.ChangeTracker.Entries().ToList())
            {
                entity.State = EntityState.Detached;
            }

            Constructors = new ObservableCollection<Constructor>(App._context.Constructors.ToList());
            ConstructorsGrid.ItemsSource = Constructors;
        }

        private void Add_Click(object sender, RoutedEventArgs e) => ShowOverlay();

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            foreach (var constructor in Constructors)
            {
                if (string.IsNullOrWhiteSpace(constructor.ConstructorName))
                    errors.AppendLine($"Название команды не указано (ID: {constructor.Id})");

                if (string.IsNullOrWhiteSpace(constructor.Nationality))
                    errors.AppendLine($"Национальность не указана (ID: {constructor.Id})");

                if (string.IsNullOrWhiteSpace(constructor.DirectorLastName))
                    errors.AppendLine($"Фамилия директора не указана (ID: {constructor.Id})");

                if (string.IsNullOrWhiteSpace(constructor.DirectorName))
                    errors.AppendLine($"Имя директора не указано (ID: {constructor.Id})");
            }

            if (errors.Length > 0)
            {
                ShowToast("Ошибка:\n" + errors.ToString());
                return;
            }

            try
            {
                foreach (var constructor in Constructors)
                {
                    var entry = App._context.Entry(constructor);
                    if (entry.State == EntityState.Detached)
                    {
                        App._context.Constructors.Attach(constructor);
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
        //    if (ConstructorsGrid.SelectedItem is Constructor selected)
        //    {
        //        try
        //        {
        //            // Проверка связанных данных
        //            if (selected.DriverConstructorAffiliations.Any() || selected.RaceResults.Any())
        //            {
        //                ShowToast("Нельзя удалить: есть связанные записи");
        //                return;
        //            }

        //            App._context.Constructors.Remove(selected);
        //            App._context.SaveChanges();
        //            Constructors.Remove(selected);
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
