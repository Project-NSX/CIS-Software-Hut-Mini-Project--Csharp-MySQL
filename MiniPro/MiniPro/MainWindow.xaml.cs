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
        MySqlConnection conn;
        string commandString;
        MySqlDataAdapter adapter;
        string postcode;
        double longitude;
        double latitude;
        string commandString2;

        public MainWindow()
        {
            InitializeComponent();

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

                // Assign command string - Take postcode, get long and lat
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


                // Close Reader
                myReader.Close();

                //Open Connection - Is this needed? try and get this done without opening this conn
                MySqlConnection connection = new MySqlConnection(connectionString);
                MySqlCommand cmdSel2 = new MySqlCommand(commandString, conn);
                DataTable dt2 = new DataTable();
                MySqlDataAdapter da2 = new MySqlDataAdapter(cmdSel2);
                da2.Fill(dt2);
                dataGrid2.DataContext = dt2;
                // Take long and lat, get all values within X miles.

                // User entered postcode query
                commandString2 = "SELECT s.*, FORMAT(( 3958.756 * acos( cos( radians(" + latitude + ") ) * cos( radians(p.latitude) ) * cos( radians(p.longitude) - radians(" + longitude + ") ) + sin( radians(" + latitude + ") ) * sin( radians(p.latitude) ) ) ),2) AS distance FROM postcodes p, services s WHERE p.postcode = s.postcode HAVING distance < 50 ORDER BY 'distance miles' ASC;";
                // Hard coded  long and lat query
                //commandString2 = "SELECT s.*, FORMAT(( 3958.756 * acos( cos( radians(52.9264910875289) ) * cos( radians(p.latitude) ) * cos( radians(p.longitude) - radians(-4.56893553583337) ) + sin( radians(52.9264910875289) ) * sin( radians(p.latitude) ) ) ),2) AS distance FROM postcodes p, services s WHERE p.postcode = s.postcode HAVING distance < 27 ORDER BY 'distance miles' ASC; ";


                command.CommandText = commandString2;
                MySqlDataReader myReader2 = command.ExecuteReader();
                
                // Print Results to Console                
                while (myReader2.Read())
                {
                    string row = "";
                    for (int i = 0; i < myReader2.FieldCount; i++)
                        row += myReader2.GetValue(i).ToString() + ", ";
                    Console.WriteLine(row);
                }

                // Close reader
                myReader2.Close();

                // Push results to datgrid
  
                MySqlCommand cmdSel = new MySqlCommand(commandString2, conn);
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.Fill(dt);
                dataGrid1.DataContext = dt;
                // Close connection...
                connection.Close();
                

            }

            catch (MySqlException ex)
            {
                Console.Error.WriteLine("Error: {0}", ex.ToString());
                conn = null;
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
