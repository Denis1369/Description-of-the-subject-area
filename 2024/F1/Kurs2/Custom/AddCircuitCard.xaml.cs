using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Kurs2.Custom
{
    public partial class AddCircuitCard : UserControl
    {
        public event EventHandler? Cancelled;
        public event EventHandler<Circuit>? Added;

        public AddCircuitCard()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
            => Cancelled?.Invoke(this, EventArgs.Empty);

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ClearValidation();

            bool hasError = false;
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MarkInvalid(NameBox);
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(LocationBox.Text))
            {
                MarkInvalid(LocationBox);
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(CountryBox.Text))
            {
                MarkInvalid(CountryBox);
                hasError = true;
            }

            if (hasError) return;

            var circuit = new Circuit
            {
                CircuitName = NameBox.Text.Trim(),
                Location = LocationBox.Text.Trim(),
                Country = CountryBox.Text.Trim()
            };

            Added?.Invoke(this, circuit);
        }

        private void MarkInvalid(Control ctl)
        {
            ctl.BorderBrush = Brushes.Red;
            ctl.BorderThickness = new Thickness(2);
        }

        private void ClearValidation()
        {
            foreach (var ctl in new Control[] { NameBox, LocationBox, CountryBox })
            {
                ctl.ClearValue(BorderBrushProperty);
                ctl.ClearValue(BorderThicknessProperty);
            }
        }
    }
}
