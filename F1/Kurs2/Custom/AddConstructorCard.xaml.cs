using System;
using System.Windows;
using System.Windows.Controls;

namespace Kurs2.Custom
{
    public class ConstructorEventArgs : EventArgs
    {
        public Constructor Constructor { get; }

        public ConstructorEventArgs(Constructor constructor)
        {
            Constructor = constructor;
        }
    }

    public partial class AddConstructorCard : UserControl
    {
        public event EventHandler? Cancelled;
        public event EventHandler<ConstructorEventArgs>? Added;

        public AddConstructorCard()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
            => Cancelled?.Invoke(this, EventArgs.Empty);

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = false;
            ClearValidationStyles();

            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MarkInvalid(NameBox);
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(DirectorLastBox.Text))
            {
                MarkInvalid(DirectorLastBox);
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(DirectorFirstBox.Text))
            {
                MarkInvalid(DirectorFirstBox);
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(NationBox.Text))
            {
                MarkInvalid(NationBox);
                hasError = true;
            }

            if (hasError) return;

            try
            {
                var team = new Constructor
                {
                    ConstructorName = NameBox.Text.Trim(),
                    Nationality = NationBox.Text.Trim(),
                    DirectorLastName = DirectorLastBox.Text.Trim(),
                    DirectorName = DirectorFirstBox.Text.Trim()
                };

                Added?.Invoke(this, new ConstructorEventArgs(team));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void MarkInvalid(Control ctl)
        {
            ctl.BorderBrush = System.Windows.Media.Brushes.Red;
            ctl.BorderThickness = new Thickness(2);
        }

        private void ClearValidationStyles()
        {
            foreach (var ctl in new Control[] { NameBox, NationBox, DirectorLastBox, DirectorFirstBox })
            {
                ctl.ClearValue(BorderBrushProperty);
                ctl.ClearValue(BorderThicknessProperty);
            }
        }
    }
}