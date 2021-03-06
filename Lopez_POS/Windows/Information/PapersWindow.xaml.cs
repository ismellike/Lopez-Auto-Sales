﻿using Lopez_POS.Static;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lopez_POS
{
    /// <summary>
    /// Interaction logic for PapersWindow.xaml
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class PapersWindow : Window
    {
        private string Buyer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PapersWindow"/> class.
        /// </summary>
        public PapersWindow(string name = "")
        {
            Buyer = name;
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Click event of the PrintButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (PaperInfoGrid.SelectedItem == null)
            {
                MessageBox.Show("Select some paper info to print");
                return;
            }

            PaperInfo paperInfo = PaperInfoGrid.SelectedItem as PaperInfo;

            if (ContractCheckBox.IsChecked.Value)
                MSEdit.PrintContract(paperInfo);
            if (TransferCheckBox.IsChecked.Value)
                MSEdit.PrintTransferAgreement(paperInfo);
            if (WarrantyCheckBox.IsChecked.Value)
                MSEdit.PrintWarranty(paperInfo);
            if (LegalCheckBox.IsChecked.Value)
                MSEdit.PrintLegal(paperInfo);
            if (LienCheckBox.IsChecked.Value)
                MSEdit.PrintLien(paperInfo);

            MessageBox.Show("Any selected papers are being printed.");
        }

        /// <summary>
        /// Handles the Loaded event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PaperInfoGrid.ItemsSource = null;
            PaperInfoGrid.ItemsSource = Storage.PapersList;

            SearchBox.Text = Buyer;
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Search(SearchBox.Text);
        }

        private void Search(string query)
        {
            PaperInfoGrid.ItemsSource = Storage.PapersList.FindAll(p => p.Buyer.Name.ToLower().Contains(query.ToLower()));

            if (PaperInfoGrid.Items.Count > 0)
                PaperInfoGrid.SelectedIndex = 0;
        }
    }
}