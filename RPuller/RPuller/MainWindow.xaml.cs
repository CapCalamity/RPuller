using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RPuller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Controller Controller { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Controller = new Controller(this);

            DLDelayBox.Text = "10";
            Controller.DLDelay = 10;
            AmountBox.Text = "10";
            Controller.AmountToFetch = 10;
            SubredditBox.Text = "gaming";
        }

        private void StartFetchButton_Click(object sender, RoutedEventArgs e)
        {
            string sub = this.SubredditBox.Text;

            Controller.StartFetch(sub);
        }

        private void DLDelayBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var textbox = (TextBox)sender;
                Controller.DLDelay = Convert.ToInt32(textbox.Text);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void AmountBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var textbox = (TextBox)sender;
                Controller.AmountToFetch = Convert.ToInt32(textbox.Text);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
