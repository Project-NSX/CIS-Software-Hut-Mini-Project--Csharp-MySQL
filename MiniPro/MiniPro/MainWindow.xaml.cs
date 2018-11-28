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

// TO DO
// GET RADIO BUTTONS WORKING
//      These should filter the categories list generated.
// FIT DATAGRID TO FRAME
// MAKE PROGRAM OOP
//

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
        string GetCategoryName;
        string selectedCategories;
        string selectedCategoriesString;
        string ageSelectionQuery;
        
        public MainWindow()
        {
            // Assigning Connection String
            string connectionString = "server=" + settings.mysql_server + ";"
                                      + "user id=" + settings.mysql_user + ";"
                                      + "password=" + settings.mysql_pass + ";"
                                      + "database=" + settings.mysql_database;
            // Create New connection
            conn = new MySqlConnection(connectionString);
            
        }
        

        // Postcode button Click
        private void BtnPostcode_Click(object sender, RoutedEventArgs e)
        {
            // Establish new connection
            MySqlCommand command = conn.CreateCommand();

            // Try catch block for returning results
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

                if(postcode == "")
                {
                    MessageBox.Show("Please enter a postcode.");
                    return;
                }
                
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

                // If Miles is checked unit = In miles, else it's in KM
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

                // If No categories are selected... Show messagebox asking to select service(s)
                if (selectedCategories == null)
                {
                    MessageBox.Show("Please select some services.");
                    return;

                }

                // Query string for user entered postcode
                commandString2 = "SELECT c.categoryName AS 'Service Type', s.serviceName AS 'Service Name', CONCAT(s.street, ', ', s.city, ', ', s.postcode) AS Address, s.telNo AS 'Telephone Number', ROUND((" + unitMulti + "* acos( cos( radians(" + latitude + ") ) * cos( radians(p.latitude) ) * cos( radians(p.longitude) - radians(" + longitude + ") ) + sin( radians(" + latitude + ") ) * sin( radians(p.latitude) ) ) ),2) AS Distance" + unit + " FROM postcodes p, services s, categories c WHERE c.categoryId IN "+ @selectedCategoriesString + " AND p.postcode = s.postcode AND c.categoryID = s.categoryID HAVING Distance" + unit + "<  " + dst + " ORDER BY Distance" + unit + " ASC;";
                
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


        // Method for loading categories into ListBox
        public void LoadCategories()
        {
            // If CategoriesListBox has items in it... Clear the listbox
            if (ListBoxCategories != null)
            {
                ListBoxCategories.Items.Clear();
            }
            // Then fill it again...

            // Load categories table
            try
            {
 
                // Link connection to command to get services list
                MySqlCommand command2 = conn.CreateCommand();
                //Open connecting using conn.
                conn.Open();

                // GET CATEGORIES
                // Set command string for getting catagories


                GetCategoryName = "SELECT categoryName FROM categories " + ageSelectionQuery + ";"; 
                


                // Set command using commandString3
                command2.CommandText = GetCategoryName;
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


                // Sub query for selected categories
                selectedCategoriesString = " (SELECT categoryId FROM categories WHERE categoryName IN (" + @selectedCategories + "))";
                Console.WriteLine(selectedCategoriesString);

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
            // If Categories has no items selected. Do not loop
            if (categories.Count == 0)
            {
                selectedCategories = null;
            }
            // Else loop and add categories to listbox
            else
            {
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


                // If no categories are selected
                if (selectedCategories == null)
                {
                    // Show error message
                    MessageBox.Show("Please select some services.");
                    return;

                }


            }

            // Sub query to be inserted into main MySQL Query
            selectedCategoriesString = " (SELECT categoryId FROM categories WHERE categoryName IN (" + selectedCategories + "))";
            Console.WriteLine(selectedCategoriesString);

        }

        //TextBox Method - This limits the TextBox for postcodes to only contain letters and numbers
        private void postcodeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            postcodeBox.Text = string.Concat(postcodeBox.Text.Where(char.IsLetterOrDigit));

            postcodeBox.SelectionStart = postcodeBox.Text.Length + 1;
        }

        // Age Selection boxes. If LoadCategories is called in the NurseryChecked method the program crashes.... not sure why
        // Loading categories works in all other methods...  This likely has something to do with the Nursery age being checked by default!
        // ITEM TO FIX
        private void NurseryChecked(object sender, RoutedEventArgs e)
        {
            ageSelectionQuery = "WHERE categoryName NOT IN('Primary', 'Secondary', 'School' )";
            GetCategoryName = "SELECT categoryName FROM categories " + ageSelectionQuery + ";";
            Console.WriteLine(ageSelectionQuery);
            Console.WriteLine(GetCategoryName);
            // Initial load of categories causes program to crash
            //LoadCategories();
        }

        private void PrimaryChecked(object sender, RoutedEventArgs e)
        {
            ageSelectionQuery = "WHERE categoryName NOT IN('Secondary', 'Nursery' )";
            GetCategoryName = "SELECT categoryName FROM categories " + ageSelectionQuery + ";";
            Console.WriteLine(ageSelectionQuery);
            Console.WriteLine(GetCategoryName);
            LoadCategories();
        }

        private void SecondaryChecked(object sender, RoutedEventArgs e)
        {

            ageSelectionQuery = "WHERE categoryName NOT IN('Primary', 'Nursery' )";
            GetCategoryName = "SELECT categoryName FROM categories " + ageSelectionQuery + ";";
            Console.WriteLine(ageSelectionQuery);
            Console.WriteLine(GetCategoryName);
            LoadCategories();
        }

        private void NoneChecked(object sender, RoutedEventArgs e)
        {

            ageSelectionQuery = "WHERE categoryName NOT IN('Primary', 'Nursery', 'Secondary', 'School')";
            GetCategoryName = "SELECT categoryName FROM categories " + ageSelectionQuery + ";";
            Console.WriteLine(ageSelectionQuery);
            Console.WriteLine(GetCategoryName);
            LoadCategories();
        }

    }
}
