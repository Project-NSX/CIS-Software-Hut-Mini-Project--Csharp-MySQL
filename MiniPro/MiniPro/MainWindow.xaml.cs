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

namespace MiniPro
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
        private void BtnLaunch_Click(object sender, RoutedEventArgs e)
        {
            double lon1 = double.Parse(TxtLon1.Text);
            double lon2 = double.Parse(TxtLon2.Text);
            double lat1 = double.Parse(txtLat1.Text);
            double lat2 = double.Parse(txtLat2.Text);
            double R = 3958.756;
            double val1 = (lat1 * (Math.PI / 180));
            double val2 = (lat2 * (Math.PI / 180));
            double val3 = ((lat2 - lat1) * (Math.PI / 180));
            double val4 = ((lon2 - lon1) * (Math.PI / 180));
            double vala = Math.Sin(val3 / 2) * Math.Sin(val3 / 2) + Math.Cos(val1) * Math.Cos(val2) * (Math.Sin(val4 / 2) * Math.Sin(val4 / 2));
            double valc = 2 * Math.Atan2(Math.Sqrt(vala), Math.Sqrt(1 - vala));

            double distance = valc * R;
            double output = Math.Round(distance, 3);
            if (distance > double.Parse(txtdistance.Text))
            {
                MessageBox.Show("distance greater than " + txtdistance.Text + "MILES" + "\n" + "distance = " + output + "Miles");
            }
            else
            {
                MessageBox.Show("distance within acceptable distance " + txtdistance.Text + "MILES" +"\n" + "distance = " + output + "Miles");
            }
        }

    }
}
