using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

// NEXT STEPS:
// Figure out how to measure 1 mile using haversine. Convert to 2 long and 2 lat values?
// Figure out query SELECT postcode WHERE longitude BETWEEN (xxxxx AND xxxxx) AND latitude BETWEEN (xxxxx AND xxxxx)?
// Sort results by distance ?????
namespace MiniPro
{
    public partial class MainWindow : Window
    {
        // Declarations
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
        MySqlConnection conn;
        string commandString;
        MySqlDataAdapter adapter;
        string postcode;
        double longitude;
        double latitude;

        public MainWindow()
        {
            InitializeComponent();

        }
        private void Haversine()
        {
            r = 3958.756;
            val1 = (lat1 * (Math.PI / 180));
            val2 = (lat2 * (Math.PI / 180));
            val3 = ((lat2 - lat1) * (Math.PI / 180));
            val4 = ((lon2 - lon1) * (Math.PI / 180));
            valA = Math.Sin(val3 / 2) * Math.Sin(val3 / 2) + Math.Cos(val1) * Math.Cos(val2) * (Math.Sin(val4 / 2) * Math.Sin(val4 / 2));
            valC = 2 * Math.Atan2(Math.Sqrt(valA), Math.Sqrt(1 - valA));
            distance = valC * r;
            output = Math.Round(distance, 3);
        }
        // Button Method
        private void BtnLaunch_Click(object sender, RoutedEventArgs e)
        {
            // Longitude, Latitude & Distance Assignment
            // Sets bool to false if text box cannot be parsed. assigns values from text boxes if they can.
            isLon1 = double.TryParse(txtLon1.Text, out lon1);
            isLat1 = double.TryParse(txtLat1.Text, out lat1);
            isLon2 = double.TryParse(txtLon2.Text, out lon2);
            isLat2 = double.TryParse(txtLat2.Text, out lat2);
            isDistance = double.TryParse(txtdistance.Text, out distance);
             
            // Call Haversine
            Haversine();

            // If any values cannot be parsed, show error message
            if (!isLon1 || !isLat1 || !isLon2 || !isLat2 || !isDistance)
            {
                MessageBox.Show("One or more Lon / Lat / Distance values is invalid");
            }
            // Else display calculation results
            else
            {
                if (distance > double.Parse(txtdistance.Text))
                {
                    MessageBox.Show("Distance greater than  " + txtdistance.Text + " Miles." + "\n" + "distance = " + output + " Miles");
                }
                else
                {
                    MessageBox.Show("Distance within acceptable distance " + txtdistance.Text + " Miles." + "\n" + "distance = " + output + " Miles");
                }
            }
        }

        // Postcode button Click
        private void BtnPostcode_Click(object sender, RoutedEventArgs e)
        {

            // Assigning Connection String
            string connectionString = "server=localhost;user id=root;password=MyNewPass;database=mini_project";
            // Link connection to connection string
            conn = new MySqlConnection(connectionString);

            MySqlCommand command = conn.CreateCommand();

            try
            {

                //Open connecting using conn.
                conn.Open();
                
                // Assign postcode string from postcodeBox
                postcode = postcodeBox.Text;

                // Assign command string
                commandString = "SELECT longitude, latitude FROM postcodes WHERE postcode='" + postcode + "';";

                //Assign Command using the commandString declared above
                command.CommandText = commandString;

                // Declare reader using command...
                MySqlDataReader myReader = command.ExecuteReader();
                // If reader is running, assign long and lat to local variables
                if (myReader.Read())
                {
                    longitude = (double)myReader[0];
                    latitude = (double)myReader[1];
                }
                // Print long and lat to messagebox
                MessageBox.Show("Longitude: " + longitude.ToString() + " Latitude: " + latitude.ToString());
                // Close Reader
                myReader.Close(); 
            }

            catch (MySqlException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());
            }

            finally
            {
                Console.WriteLine("Closing Connection...");
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }

    }
}
