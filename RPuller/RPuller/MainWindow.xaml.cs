using System;
using System.Collections.Generic;
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
        }

        private void StartFetchButton_Click(object sender, RoutedEventArgs e)
        {
            string sub = this.SubredditBox.Text;
            int amount = Convert.ToInt32(this.AmountBox.Text);

            Controller.StartFetch(sub, amount);
        }
    }
}
