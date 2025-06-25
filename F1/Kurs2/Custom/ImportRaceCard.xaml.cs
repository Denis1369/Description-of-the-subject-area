using Kurs2.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kurs2.Custom
{
    public partial class ImportRaceCard : UserControl
    {
        public event EventHandler Cancelled;
        public event EventHandler ImportCompleted;

        private readonly PythonParser _parser;
        private const string Host = "localhost";
        private const string User = "root";
        private const string Password = "root";
        private const string Database = "formula1_2025";

        public ImportRaceCard()
        {
            InitializeComponent();

            _parser = new PythonParser(
                @"C:\Users\Denis\anaconda3\python.exe",
                @"C:\Games\get_gp_results.py");

            RaceComboBox.ItemsSource = App._context.Races
                .Where(r => r.RaceStatus == "Завершено")
                .ToList();
            SeasonComboBox.ItemsSource = App._context.Seasons.ToList();

            RaceComboBox.DisplayMemberPath = nameof(Race.RaceName);
            SeasonComboBox.DisplayMemberPath = nameof(Season.Year);
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            GridMain.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#80000000"));

            if (RaceComboBox.SelectedItem == null || SeasonComboBox.SelectedItem == null)
            {
                MarkInvalidFields();
                return;
            }

            ClearValidationStyles();
            ToggleLoading(true);

            try
            {
                var race = (Race)RaceComboBox.SelectedItem;
                var result = await _parser.ExecuteAsync(
                    (int)race.Id,
                    Host,
                    User,
                    Password,
                    Database);

                if (result.ExitCode != 0)
                    throw new Exception($"Ошибка: {result.Error}");

                ImportCompleted?.Invoke(this, EventArgs.Empty);
                MessageBox.Show("Импорт успешно завершен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка импорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ToggleLoading(false);
                GridMain.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        private void ToggleLoading(bool isLoading)
        {
            LoadingOverlay.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            AddButton.IsEnabled = !isLoading;
            CancelButton.IsEnabled = !isLoading;
        }

        private void MarkInvalidFields()
        {
            if (RaceComboBox.SelectedItem == null)
                MarkInvalid(RaceComboBox);
            if (SeasonComboBox.SelectedItem == null)
                MarkInvalid(SeasonComboBox);
        }

        private void MarkInvalid(Control control)
        {
            control.BorderBrush = Brushes.Red;
            control.BorderThickness = new Thickness(2);
        }

        private void ClearValidationStyles()
        {
            RaceComboBox.ClearValue(BorderBrushProperty);
            RaceComboBox.ClearValue(BorderThicknessProperty);
            SeasonComboBox.ClearValue(BorderBrushProperty);
            SeasonComboBox.ClearValue(BorderThicknessProperty);
        }
    }
}