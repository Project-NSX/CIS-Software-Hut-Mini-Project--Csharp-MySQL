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
    public partial class MainWindow : Window
    {
        double lon1;
        bool isLon1;
        double lat1;
        bool isLat1;
        double lon2;
        bool isLon2;
        double lat2;
        bool isLat2;
        double r;
        double val1;
        double val2;
        double val3;
        double val4;
        double valA;
        double valC;
        double distance;
        bool isDistance;
        double output;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void BtnLaunch_Click(object sender, RoutedEventArgs e)
        {
            // Haversine. Needs own class
            isLon1 = double.TryParse(txtLon1.Text, out lon1);
            isLat1 = double.TryParse(txtLat1.Text, out lat1);
            isLon2 = double.TryParse(txtLon2.Text, out lon2);
            isLat2 = double.TryParse(txtLat2.Text, out lat2);
            isDistance = double.TryParse(txtdistance.Text, out distance);

            r = 3958.756;
            val1 = (lat1 * (Math.PI / 180));
            val2 = (lat2 * (Math.PI / 180));
            val3 = ((lat2 - lat1) * (Math.PI / 180));
            val4 = ((lon2 - lon1) * (Math.PI / 180));
            valA = Math.Sin(val3 / 2) * Math.Sin(val3 / 2) + Math.Cos(val1) * Math.Cos(val2) * (Math.Sin(val4 / 2) * Math.Sin(val4 / 2));
            valC = 2 * Math.Atan2(Math.Sqrt(valA), Math.Sqrt(1 - valA));
            distance = valC * r;
            output = Math.Round(distance, 3);

            if (!isLon1 || !isLat1 || !isLon2 || !isLat2 || !isDistance)
            {
                MessageBox.Show("One or more Lon / Lat / Distance values is invalid");
            }
            else
            {
                if (distance > double.Parse(txtdistance.Text))
                {
                    MessageBox.Show("Distance greater than  " + txtdistance.Text + " MILES." + "\n" + "distance = " + output + " Miles");
                }
                else
                {
                    MessageBox.Show("Distance within acceptable distance " + txtdistance.Text + " MILES." + "\n" + "distance = " + output + " Miles");
                }
            }
        }

    }
}
