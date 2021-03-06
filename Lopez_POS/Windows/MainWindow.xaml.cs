﻿using Lopez_POS.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lopez_POS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the TextChanged event of the Search control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        internal void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayResults(NameText.Text, CarText.Text);
        }

        /// <summary>
        /// Displays the results.
        /// </summary>
        /// <param name="nameQuery">The name query.</param>
        /// <param name="carQuery">The car query.</param>
        internal void DisplayResults(string nameQuery, string carQuery)
        {
            List<Person> people = new List<Person>();

            //Search in parallel
            Parallel.ForEach(Storage.PeopleList, (Person person) =>
            {
                if (!String.IsNullOrEmpty(nameQuery))
                    if (!person.Name.ToLower().Contains(nameQuery.ToLower()))
                        return;

                if (!String.IsNullOrEmpty(carQuery))
                    if (person.Cars.FirstOrDefault(car => car.ToString().ToLower().Contains(carQuery.ToLower())) == null)
                        return;

                lock (people)
                {
                    people.Add(person);
                }
            });

            AccountsBox.Items.Clear();
            foreach (Person person in people)
            {
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

        /// <summary>
        /// Handles the MouseDoubleClick event of the Item control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            Person person = (Person)((TreeViewItem)item.Parent).Header;
            PaymentCar paymentCar = (PaymentCar)item.Header;
            //Owner property must be set
            EntryWindow entryWindow = new EntryWindow(person, paymentCar) { Owner = this, Title = person.ToString() + ": " + paymentCar.ToString() };
            entryWindow.Show();
        }

        /// <summary>
        /// Handles the Click event of the CarsButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CarsButton_Click(object sender, RoutedEventArgs e)
        {
            SalesCars salesCars = new SalesCars();
            salesCars.Show();
        }

        /// <summary>
        /// Handles the Loaded event of the Main control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Main_Loaded(object sender, RoutedEventArgs e)
        {
            Initializer.Initialize();
            Search_TextChanged(null, null);
        }

        /// <summary>
        /// Handles the Click event of the SellCarButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SellCarButton_Click(object sender, RoutedEventArgs e)
        {
            SellCarWindow carWindow = new SellCarWindow();
            carWindow.Show();
        }

        /// <summary>
        /// Handles the Click event of the InformationButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void InformationButton_Click(object sender, RoutedEventArgs e)
        {
            InformationWindow infoWindow = new InformationWindow();
            infoWindow.Show();
        }

        /// <summary>
        /// Handles the Click event of the EditPersonButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void EditPersonButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(AccountsBox.SelectedItem is TreeView tree))
            {
                MessageBox.Show(this, "Select a person.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Person person = (tree.Items[0] as TreeViewItem).Header as Person;
            EditPersonWindow editWindow = new EditPersonWindow(person) { Topmost = true };
            if (editWindow.ShowDialog() == true)
            {
                Search_TextChanged(null, null);
            }
        }
    }
}