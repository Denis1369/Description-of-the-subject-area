using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Kurs2.Custom
{
    public partial class AddAffiliationCard : UserControl
    {
        public event EventHandler? Cancelled;
        public event EventHandler? Added;

        public List<Driver> _drivers;
        public List<Constructor> _constructors;

        public AddAffiliationCard()
        {
            InitializeComponent();


            _drivers = App._context.Drivers.ToList();
            _constructors = App._context.Constructors.ToList();

            DriverComboBox.ItemsSource = _drivers;
            DriverComboBox.DisplayMemberPath = nameof(Driver.LastName);

            ConstructorComboBox.ItemsSource = _constructors;
            ConstructorComboBox.DisplayMemberPath = nameof(Constructor.ConstructorName);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
            => Cancelled?.Invoke(this, EventArgs.Empty);

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

            ClearValidationStyles();

            if (DriverComboBox.SelectedItem is not Driver selectedDriver)
            {
                MarkInvalid(DriverComboBox);
                return;
            }

            if (ConstructorComboBox.SelectedItem is not Constructor selectedConstructor)
            {
                MarkInvalid(ConstructorComboBox);
                return;
            }

            if (StartDate.SelectedDate == null)
            {
                MarkInvalid(StartDate);
                return;
            }

            DriverConstructorAffiliation.Add(
                selectedDriver.Id,
                selectedConstructor.Id,
                StartDate.SelectedDate.Value);

            Added?.Invoke(this, EventArgs.Empty);
        }

        private void MarkInvalid(Control control)
        {
            control.BorderBrush = Brushes.Red;
            control.BorderThickness = new Thickness(2);
        }

        private void ClearValidationStyles()
        {
            DriverComboBox.ClearValue(BorderBrushProperty);
            DriverComboBox.ClearValue(BorderThicknessProperty);

            ConstructorComboBox.ClearValue(BorderBrushProperty);
            ConstructorComboBox.ClearValue(BorderThicknessProperty);

            StartDate.ClearValue(BorderBrushProperty);
            StartDate.ClearValue(BorderThicknessProperty);
        }

    }
}