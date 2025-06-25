using Kurs2.Custom;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Kurs2.Pages
{
    public partial class CircuitsPage : Page
    {
        public ObservableCollection<Circuit> Circuits { get; set; }

        public CircuitsPage()
        {
            InitializeComponent();
            LoadCircuitsData();
            DataContext = this;

            AddCircuitCard.Cancelled += (_, _) => HideOverlay();
            AddCircuitCard.Added += (_, circuit) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(circuit.CircuitName) ||
                        string.IsNullOrWhiteSpace(circuit.Location) ||
                        string.IsNullOrWhiteSpace(circuit.Country))
                    {
                        ShowToast("Ошибка: Заполните все поля");
                        return;
                    }

                    App._context.Circuits.Add(circuit);
                    App._context.SaveChanges();

                    LoadCircuitsData();
                    HideOverlay();
                    ShowToast("Добавлено");
                }
                catch (Exception ex)
                {
                    ShowToast($"Ошибка добавления: {ex.Message}");
                }
            };
        }

        private void LoadCircuitsData()
        {
            foreach (var entity in App._context.ChangeTracker.Entries().ToList())
            {
                entity.State = EntityState.Detached;
            }

            Circuits = new ObservableCollection<Circuit>(App._context.Circuits.ToList());
            CircuitsGrid.ItemsSource = Circuits;
        }

        private void Add_Click(object sender, RoutedEventArgs e) => ShowOverlay();

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            foreach (var circuit in Circuits)
            {
                if (string.IsNullOrWhiteSpace(circuit.CircuitName))
                    errors.AppendLine($"Название не указано (ID: {circuit.Id})");

                if (string.IsNullOrWhiteSpace(circuit.Location))
                    errors.AppendLine($"Локация не указана (ID: {circuit.Id})");

                if (string.IsNullOrWhiteSpace(circuit.Country))
                    errors.AppendLine($"Страна не указана (ID: {circuit.Id})");
            }

            if (errors.Length > 0)
            {
                ShowToast("Ошибка:\n" + errors.ToString());
                return;
            }

            try
            {
                foreach (var circuit in Circuits)
                {
                    var entry = App._context.Entry(circuit);
                    if (entry.State == EntityState.Detached)
                    {
                        App._context.Circuits.Attach(circuit);
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
        //    if (CircuitsGrid.SelectedItem is Circuit selected)
        //    {
        //        try
        //        {
        //            App._context.Circuits.Remove(selected);
        //            App._context.SaveChanges();
        //            Circuits.Remove(selected);
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
