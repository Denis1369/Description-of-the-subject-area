using Kurs2.Custom;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace Kurs2.Pages
{
    public partial class DriverConstructorAffiliationPage : Page
    {
        public List<DriverConstructorAffiliation> Affiliations { get; set; }
        public List<Driver> Drivers { get; set; }
        public List<Constructor> Constructors { get; set; }

        public DriverConstructorAffiliationPage()
        {
            InitializeComponent();

            AddCard.Cancelled += (s, e) => HideOverlay();
            AddCard.Added += (s, e) =>
            {
                ReloadGrid();
                HideOverlay();
            };

            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            Affiliations = App._context.DriverConstructorAffiliations.Include(a => a.Driver).Include(a => a.Constructor).ToList();

            Drivers = App._context.Drivers.ToList();
            Constructors = App._context.Constructors.ToList();

            AffiliationGrid.ItemsSource = Affiliations;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            App._context.SaveChanges();
            ShowToast("Сохранено");
            LoadData();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (AffiliationGrid.SelectedItem is DriverConstructorAffiliation sel)
            {
                App._context.DriverConstructorAffiliations.Remove(sel);
                App._context.SaveChanges();
                ShowToast("Удалено");
                LoadData();
            }
        }

        private void ReloadGrid()
        {
            var list = App._context.DriverConstructorAffiliations
                           .Include(a => a.Driver)
                           .Include(a => a.Constructor)
                           .ToList();
            AffiliationGrid.ItemsSource = list;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ShowOverlay();
        }

        private void ShowOverlay()
        {
            Overlay.Visibility = Visibility.Visible;
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
            Overlay.BeginAnimation(UIElement.OpacityProperty, fadeIn);
        }

        private void HideOverlay()
        {
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.3));
            fadeOut.Completed += (s, e) =>
            {
                Overlay.Visibility = Visibility.Collapsed;
            };
            Overlay.BeginAnimation(UIElement.OpacityProperty, fadeOut);
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
