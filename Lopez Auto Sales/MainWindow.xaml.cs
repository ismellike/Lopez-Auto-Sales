﻿using Lopez_Auto_Sales.Cars;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lopez_Auto_Sales
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        internal void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Thread th = new Thread(new ThreadStart(delegate ()
            {
                AccountsBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    DisplayResults(NameText.Text, CarText.Text);
                }));
            }));

            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        internal void DisplayResults(string nameQuery, string carQuery)
        {
            AccountsBox.Items.Clear();
           foreach (Person person in Storage.People)
            {
                if (!String.IsNullOrEmpty(nameQuery))
                    if (!person.Name.ToLower().Contains(nameQuery.ToLower()))
                        continue;

                if (!String.IsNullOrEmpty(carQuery))
                    if (person.Cars.FirstOrDefault(car => car.ToString().ToLower().Contains(carQuery.ToLower())) == null)
                        continue;

                TreeView treeView = new TreeView();
                TreeViewItem header = new TreeViewItem() { Header = person };
                foreach (PaymentCar car in person.Cars)
                {
                    TreeViewItem item = new TreeViewItem() { Header = car };
                    item.MouseDoubleClick += Item_MouseDoubleClick;
                    header.Items.Add(item);
                }
                treeView.Items.Add(header);
                AccountsBox.Items.Add(treeView);
            }
        }

        private void Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            Person person = (Person)((TreeViewItem)item.Parent).Header;
            PaymentCar paymentCar = (PaymentCar)item.Header;
            EntryWindow entryWindow = new EntryWindow(person, paymentCar) { Owner= this, Title = person.ToString() + ": " + paymentCar.ToString() };
            entryWindow.Show();
        }

        private void CarsButton_Click(object sender, RoutedEventArgs e)
        {
            SalesCars salesCars = new SalesCars() { Owner = this };
            salesCars.Show();
        }

        private bool IsConnectedInternet()
        {
            try
            {
                PingReply reply = new Ping().Send("google.com");
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch { }
            return false;
        }

        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsConnectedInternet())
                MessageBox.Show(this, "You are not connected to the internet.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            Storage.Init();
            Search_TextChanged(null, null);
        }

        private void SellCarButton_Click(object sender, RoutedEventArgs e)
        {
            SellCarWindow carWindow = new SellCarWindow() { Owner = this };
            carWindow.Show();
        }

        private void PapersButton_Click(object sender, RoutedEventArgs e)
        {
            PaperWindow paperWindow = new PaperWindow() { Owner = this };
            paperWindow.Show();
        }

        private void EditPersonButton_Click(object sender, RoutedEventArgs e)
        {
            TreeView tree = AccountsBox.SelectedItem as TreeView;
            
            if(tree == null)
            {
                MessageBox.Show(this, "Select a person.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Person person = (tree.Items[0] as TreeViewItem).Header as Person;
            EditPersonWindow editWindow = new EditPersonWindow(person) { Owner = this, Topmost= true };
            if(editWindow.ShowDialog() == true)
            {
                Search_TextChanged(null, null);
            }
        }
    }

    public static class Extensions
    {
        public static string ToCapital(this string message)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(message.ToLower());
        }
    }
}
