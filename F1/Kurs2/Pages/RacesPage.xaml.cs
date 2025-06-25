using Kurs2.Custom;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Kurs2.Pages
{
    public partial class RacesPage : Page
    {
        public ObservableCollection<Race> Races { get; set; }
        public ObservableCollection<Season> Seasons { get; set; }
        public ObservableCollection<Circuit> Circuits { get; set; }
        public ObservableCollection<string> RaceStatuses { get; set; } =
            new ObservableCollection<string> { "Запланировано", "Завершено", "Отменено", "Отложено" };

        public RacesPage()
        {
            InitializeComponent();

            LoadRacesData();

            Seasons = new ObservableCollection<Season>(App._context.Seasons.ToList());
            Circuits = new ObservableCollection<Circuit>(App._context.Circuits.ToList());

            DataContext = this;

            AddRaceCard.Cancelled += (_, _) => HideOverlay();
            AddRaceCard.Added += (_, race) =>
            {
                if (race.SeasonId == 0 ||
                    race.CircuitId == 0 ||
                    string.IsNullOrWhiteSpace(race.RaceName))
                {
                    ShowToast("Ошибка: Заполните все обязательные поля");
                    return;
                }

                try
                {
                    App._context.Races.Add(race);
                    App._context.SaveChanges();

                    race.Season = Seasons.FirstOrDefault(s => s.Id == race.SeasonId);
                    race.Circuit = Circuits.FirstOrDefault(c => c.Id == race.CircuitId);

                    Races.Add(race);

                    HideOverlay();
                    ShowToast("Добавлено");
                }
                catch (Exception ex)
                {
                    ShowToast($"Ошибка добавления: {ex.Message}");
                }
            };
        }

        private void LoadRacesData()
        {
            foreach (var entity in App._context.ChangeTracker.Entries().ToList())
            {
                entity.State = EntityState.Detached;
            }

            var races = App._context.Races
                .Include(r => r.Season)
                .Include(r => r.Circuit)
                .OrderBy(r => r.Id)
                .ToList();

            Races = new ObservableCollection<Race>(races);
        }

        private void Add_Click(object sender, RoutedEventArgs e) => ShowOverlay();

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            foreach (var race in Races)
            {
                if (race.SeasonId == 0 ||
                    race.CircuitId == 0 ||
                    string.IsNullOrWhiteSpace(race.RaceName) ||
                    string.IsNullOrWhiteSpace(race.RaceStatus))
                {
                    ShowToast("Ошибка: Заполните все обязательные поля");
                    return;
                }
            }

            try
            {
                foreach (var race in Races)
                {
                    var entry = App._context.Entry(race);
                    if (entry.State == EntityState.Detached)
                    {
                        App._context.Races.Attach(race);
                    }
                    entry.State = EntityState.Modified;
                }

                App._context.SaveChanges();

                LoadRacesData();
                RacesGrid.ItemsSource = Races;

                ShowToast("Сохранено");
            }
            catch (Exception ex)
            {
                ShowToast($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (RacesGrid.SelectedItem is Race race)
            {
                try
                {
                    App._context.Races.Remove(race);
                    App._context.SaveChanges();
                    Races.Remove(race);
                    ShowToast("Запись удалена");
                }
                catch (Exception ex)
                {
                    ShowToast($"Ошибка удаления: {ex.Message}");
                }
            }
        }

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
