using Kurs2.Custom;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using MaterialDesignThemes.Wpf;

namespace Kurs2.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddRacingResultPage.xaml
    /// </summary>
    public partial class AddRacingResultPage : Page
    {
        public AddRacingResultPage()
        {
            InitializeComponent();

            Load();

            ImportBtn.Visibility = Visibility.Collapsed;
        }

        public void Load()
        {
            foreach (var item in App._context.Seasons.ToList().OrderBy(y => y.Year))
            {
                if (DateTime.Now.Year <= item.Year)
                    SeasonC.Items.Add(item.Year);
            }
            foreach (var item in App._context.Drivers.ToList())
            {
                RacerC.Items.Add(item.LastName);
            }
            var list = App._context.Races.Where(r=>r.RaceStatus== "Завершено").ToList();
            foreach (var item in list)
            {
                RaceC.Items.Add(item.RaceName);
            }
            for (int i = 1; i <= 20; i++)
            {
                StartC.Items.Add(i.ToString());
                EndC.Items.Add(i.ToString());
            }
            StartC.Items.Add("Pit");
            EndC.Items.Add("Ret");
            EndC.Items.Add("DSQ");

            SeasonC.SelectedIndex = 0;
            RacerC.SelectedIndex = 0;
            RaceC.SelectedIndex = 0;
            StartC.SelectedIndex = 0;
            EndC.SelectedIndex = 0;
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            int racerId = RacerC.SelectedIndex+1;
            int raceId = RaceC.SelectedIndex+1;
            int seson = SeasonC.SelectedIndex+1;
            string start = StartC.SelectedValue.ToString();
            string end = EndC.SelectedValue.ToString();
            var driver = App._context.DriverConstructorAffiliations.FirstOrDefault(d => d.Id == racerId && d.EndDate == null);
            ShowToast(RaceResult.GenerateRacingResult(seson ,racerId, raceId, driver.ConstructorId, start, end));
        }

        private void Del_Click(object sender, RoutedEventArgs e)
        {
            int racerId = RacerC.SelectedIndex + 1;
            int raceId = RaceC.SelectedIndex + 1;
            int seson = SeasonC.SelectedIndex + 1;
            ShowToast(RaceResult.DeleteRacingResult(seson, racerId, raceId));
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            var importCard = new ImportRaceCard();

            importCard.Cancelled += (s, ev) =>
            {
                DialogHost.Close("ImportDialogHost");
            };

            importCard.ImportCompleted += (s, ev) =>
            {
                ShowToast("Импорт завершён успешно");
                DialogHost.Close("ImportDialogHost");
            };

            await DialogHost.Show(importCard, "ImportDialogHost");
        }
    }
}
