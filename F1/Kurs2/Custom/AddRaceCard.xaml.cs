using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Kurs2.Custom
{
    public partial class AddRaceCard : UserControl
    {
        public event EventHandler? Cancelled;
        public event EventHandler<Race>? Added;

        public AddRaceCard()
        {
            InitializeComponent();
            LoadLookups();
        }

        private void LoadLookups()
        {
            SeasonCombo.ItemsSource = App._context.Seasons.ToList();
            SeasonCombo.DisplayMemberPath = "Year";
            SeasonCombo.SelectedValuePath = "Id";

            TrackCombo.ItemsSource = App._context.Circuits.ToList();
            TrackCombo.DisplayMemberPath = "CircuitName";
            TrackCombo.SelectedValuePath = "Id";
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
            => Cancelled?.Invoke(this, EventArgs.Empty);

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ClearValidationStyles();

            bool hasError = false;

            if (SeasonCombo.SelectedValue is not uint seasonId || seasonId == 0)
            {
                MarkInvalid(SeasonCombo);
                hasError = true;
            }

            if (TrackCombo.SelectedValue is not uint circuitId || circuitId == 0)
            {
                MarkInvalid(TrackCombo);
                hasError = true;
            }

            if (DatePicker.SelectedDate is not DateTime date || date == default)
            {
                MarkInvalid(DatePicker);
                hasError = true;
            }

            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MarkInvalid(NameBox);
                hasError = true;
            }

            if (hasError)
            {
                ShowToast("Заполните все обязательные поля");
                return;
            }

            var race = new Race
            {
                SeasonId = (uint)SeasonCombo.SelectedValue!,
                CircuitId = (uint)TrackCombo.SelectedValue!,
                RaceDate = DateOnly.FromDateTime(DatePicker.SelectedDate.Value),
                RaceName = NameBox.Text.Trim(),
                RaceStatus = "Запланировано"
            };

            Added?.Invoke(this, race);
        }

        private void MarkInvalid(Control ctl)
        {
            ctl.BorderBrush = Brushes.Red;
            ctl.BorderThickness = new Thickness(2);
        }

        private void ClearValidationStyles()
        {
            foreach (var ctl in new Control[] { SeasonCombo, TrackCombo, DatePicker, NameBox })
            {
                ctl.ClearValue(BorderBrushProperty);
                ctl.ClearValue(BorderThicknessProperty);
            }
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
