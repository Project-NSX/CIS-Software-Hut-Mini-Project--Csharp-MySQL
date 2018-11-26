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
using System.Collections;

namespace MiniPro
{
    public partial class MainWindow : Window
    {
        // Declarations
        Properties.Settings settings = Properties.Settings.Default;
        MySqlConnection conn;
        string commandString;
        string postcode;
        double longitude;
        double latitude;
        string commandString2;
        int dst;
        double unitMulti;
        string unit;
        string commandString3;
        string selectedCategories;
        string selectedCategoriesString;

        
        public MainWindow()
        {
            // Assigning Connection String
            string connectionString = "server=" + settings.mysql_server + ";"
                                      + "user id=" + settings.mysql_user + ";"
                                      + "password=" + settings.mysql_pass + ";"
                                      + "database=" + settings.mysql_database;

            conn = new MySqlConnection(connectionString);
        }
        
        // Postcode button Click
        private void BtnPostcode_Click(object sender, RoutedEventArgs e)
        {

            MySqlCommand command = conn.CreateCommand();


            try
            {

                //Open connecting using conn.
                conn.Open();

                // Assign postcode string from postcodeBox
                // This needs error handling.. somehow xD
                postcode = postcodeBox.Text;
                // Replace whitespaces with null
                postcode = postcode.Replace(" ", "");
                postcode = postcode.Replace("-", "");

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

                if ((bool)miles.IsChecked)
                {
                    unitMulti = 3958.756;
                    unit = "Miles";
                }
                else if ((bool)km.IsChecked)
                {
                    unitMulti = 6371.0002161;
                    unit = "Km";
                }

                // Query string for user entered postcode
                commandString2 = "SELECT c.categoryName AS 'Service Type', s.serviceName AS 'Service Name', CONCAT(s.street, ', ', s.city, ', ', s.postcode) AS Address, s.telNo AS 'Telephone Number', ROUND((" + unitMulti + "* acos( cos( radians(" + latitude + ") ) * cos( radians(p.latitude) ) * cos( radians(p.longitude) - radians(" + longitude + ") ) + sin( radians(" + latitude + ") ) * sin( radians(p.latitude) ) ) ),2) AS Distance" + unit + " FROM postcodes p, services s, categories c WHERE c.categoryId IN "+ selectedCategoriesString + " AND p.postcode = s.postcode AND c.categoryID = s.categoryID HAVING Distance" + unit + "<  " + dst + " ORDER BY Distance" + unit + " ASC;";
                
                // Set command using commandString2
                command.CommandText = commandString2;
                // Open new reader
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

                // Push results to datgrid (LONG + LAT)
                MySqlCommand cmdSel = new MySqlCommand(commandString2, conn);
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.Fill(dt);
                dataGrid1.DataContext = dt;
            }

            //MySQL Error Handling
            catch (MySqlException ex)
            {
                Console.Error.WriteLine("Error: {0}", ex.ToString());
                conn = null;
            }

            // Close MySQL Connection
            finally
            {
                Console.WriteLine("Closing Connection...");
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        // Distance slider method
        private void distanceVal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            dst = Convert.ToInt32(e.NewValue);
        }
        



        private void LoadCategories(object sender, RoutedEventArgs e)
        {
     
            try
            {

                // Link connection to command to get services list
                MySqlCommand command2 = conn.CreateCommand();
                //Open connecting using conn.
                conn.Open();

                // GET CATEGORIES
                // Set command string for getting catagories
                commandString3 = "SELECT categoryName FROM categories;";
                // Set command using commandString3
                command2.CommandText = commandString3;
                // Create new reader

                MySqlDataReader myReader3 = command2.ExecuteReader();

                // Open reader, while reader is reading....
                while (myReader3.Read())
                {
                    //Print categories to console... This is for testing.
                    string row = "";
                    for (int i = 0; i < myReader3.FieldCount; i++)
                        row += myReader3.GetValue(i).ToString();
                    Console.WriteLine(row);

                    // Add Categories to ListBox
                    ListBoxCategories.Items.Add(myReader3.GetString(0));
                }
            }

            //MySQL Error Handling
            catch (MySqlException ex)
            {
                Console.Error.WriteLine("Error: {0}", ex.ToString());
                conn = null;
            }

            // Close MySQL Connection
            finally
            {
                Console.WriteLine("Closing Connection...");
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        // Categories Selection changed 
        private void ListBoxCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Add items to variable called categories
            IList categories = ListBoxCategories.SelectedItems;
            
            // Loop throgh Ilist 
            for (int i = 0; i < categories.Count; i++)
            {
                
                //Print category to console
                Console.WriteLine(categories[i]);

                
                if (i == 0)
                {
                    selectedCategories = "'" + categories[i] + "'";
                }
                else if (i < categories.Count)
                {
                    selectedCategories += ", '" + categories[i] + "'";
                }
                else if (i == categories.Count)
                {
                    selectedCategories += "'" + categories[i] + "'";
                }

            }

            // Below is the start point for the test string. Going to get this to add categories to the string then insert it into the main sql statement
            selectedCategoriesString = " (SELECT categoryId FROM categories WHERE categoryName IN (" + selectedCategories + "))";
            Console.WriteLine(selectedCategoriesString);
        }
    }
}
