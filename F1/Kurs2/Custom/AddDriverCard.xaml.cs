using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Kurs2.Custom
{
    public partial class AddDriverCard : UserControl
    {
        public event EventHandler? Cancelled;
        public event EventHandler<Driver>? Added;

        public AddDriverCard()
        {
            InitializeComponent();
        }

        public void Reset()
        {
            LastNameBox.Text = "";
            FirstNameBox.Text = "";
            DobPicker.SelectedDate = null;
            NationalityBox.Text = "";
            UrlBox.Text = "";

            LastNameBox.ClearValue(BorderBrushProperty);
            FirstNameBox.ClearValue(BorderBrushProperty);
            DobPicker.ClearValue(BorderBrushProperty);
            NationalityBox.ClearValue(BorderBrushProperty);
            UrlBox.ClearValue(BorderBrushProperty);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Reset();
            Cancelled?.Invoke(this, EventArgs.Empty);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            bool hasError = false;
            ResetErrorStyles();

            if (string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                SetErrorStyle(LastNameBox);
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text))
            {
                SetErrorStyle(FirstNameBox);
                hasError = true;
            }
            if (DobPicker.SelectedDate == null)
            {
                SetErrorStyle(DobPicker);
                hasError = true;
            }
            if (string.IsNullOrWhiteSpace(NationalityBox.Text))
            {
                SetErrorStyle(NationalityBox);
                hasError = true;
            }

            if (!string.IsNullOrWhiteSpace(UrlBox.Text))
            {
                string pattern = @"^(https?://)?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
                if (!Regex.IsMatch(UrlBox.Text.Trim(), pattern, RegexOptions.IgnoreCase))
                {
                    SetErrorStyle(UrlBox);
                    hasError = true;
                }
            }

            if (hasError) return;

            var newDriver = new Driver
            {
                LastName = LastNameBox.Text.Trim(),
                FirstName = FirstNameBox.Text.Trim(),
                DateOfBirth = DobPicker.SelectedDate.HasValue
                    ? DateOnly.FromDateTime(DobPicker.SelectedDate.Value)
                    : null,
                Nationality = NationalityBox.Text.Trim(),
                Url = string.IsNullOrWhiteSpace(UrlBox.Text)
                    ? null
                    : UrlBox.Text.Trim()
            };

            Added?.Invoke(this, newDriver);
        }

        private void ResetErrorStyles()
        {
            LastNameBox.ClearValue(BorderBrushProperty);
            FirstNameBox.ClearValue(BorderBrushProperty);
            DobPicker.ClearValue(BorderBrushProperty);
            NationalityBox.ClearValue(BorderBrushProperty);
            UrlBox.ClearValue(BorderBrushProperty);
        }

        private void SetErrorStyle(Control control)
        {
            control.BorderBrush = System.Windows.Media.Brushes.Red;
            control.BorderThickness = new Thickness(2);
        }
    }
}