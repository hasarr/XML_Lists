using MySql.Data.MySqlClient;
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
using System.Windows.Shapes;

namespace ES_SYSTEM_K_Listy
{
    /// <summary>
    /// Logika interakcji dla klasy Login.xaml
    /// </summary>
    public partial class Login : Window
    {

        public Login()
        {
            InitializeComponent();
        }

        private void checkLoginFunctionForTesting()
        {
            MainWindow app = new MainWindow();
            if ((bool)App.Current.Properties["mainWindowConstructorStatus"])
            {
                this.Close();
                app.Show();
            }
            else
            {
                app.Close();
            }

        }
        private void loginSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if(loginInput.Text == "admin" && passwordInput.Password == "123")
            {
                App.Current.Properties["username"] = "admin";
                App.Current.Properties["isAdmin"] = true;
                checkLoginFunctionForTesting();
            }

            else if (loginInput.Text == "operator1" && passwordInput.Password == "123")
            {
                App.Current.Properties["username"] = "operator1";
                App.Current.Properties["isAdmin"] = false;
                checkLoginFunctionForTesting();
            }

            else if (loginInput.Text == "operator2" && passwordInput.Password == "123")
            {
                App.Current.Properties["username"] = "operator2";
                App.Current.Properties["isAdmin"] = false;
                checkLoginFunctionForTesting();
            }
            else
            {
                MessageBox.Show("Nieprawidłowy login lub hasło");
            }

            /*
            MySqlConnection loginDatabase = new MySqlConnection("SERVER=127.0.0.1;DATABASE=test;UID=root;PASSWORD=;");
            try
            {
                //connect to the database
                
                if (loginDatabase.State == System.Data.ConnectionState.Closed) loginDatabase.Open();
                //Query for checking login data
                String query = "SELECT COUNT(1) FROM logins WHERE userLogin=@Username AND userPassword=@Password";
                MySqlCommand selectLoginCmd = new MySqlCommand(query, loginDatabase);
                selectLoginCmd.CommandType = System.Data.CommandType.Text;
                selectLoginCmd.Parameters.AddWithValue("@Username",loginInput.Text);
                selectLoginCmd.Parameters.AddWithValue("@Password", passwordInput.Password);
                int count = Convert.ToInt32(selectLoginCmd.ExecuteScalar());

                //checking how many rows was returned by the query (if 1, then user exists)
                if (count==1)
                {
                    App.Current.Properties["username"] = loginInput.Text;
                    //check for admin query
                    int countAdmin = 0;
                    String checkPermissionsQuery = "SELECT COUNT(1) FROM logins WHERE userLogin=@Username AND userPassword=@Password AND userType='admin'";
                    MySqlCommand checkPermissionsCmd = new MySqlCommand(checkPermissionsQuery, loginDatabase);
                    checkPermissionsCmd.CommandType = System.Data.CommandType.Text;
                    checkPermissionsCmd.Parameters.AddWithValue("@Username", loginInput.Text);
                    checkPermissionsCmd.Parameters.AddWithValue("@Password", passwordInput.Password);
                    countAdmin = Convert.ToInt32(checkPermissionsCmd.ExecuteScalar());
                    if (countAdmin == 1)
                    {
                        App.Current.Properties["isAdmin"] = true;
                    }
                    else
                    {
                        App.Current.Properties["isAdmin"] = false;
                    }

                    //close the login windows and proceed to the main window
                    
                        MainWindow app = new MainWindow();
                    if ((bool)App.Current.Properties["mainWindowConstructorStatus"])
                    {
                        this.Close();
                        app.Show();
                    }
                    else
                    {
                        app.Close();
                    }
                    
                }
                else
                {
                    MessageBox.Show("Login lub hasło są nieprawidłowe");
                }

                
             
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
          
            finally
            {
                loginDatabase.Close();
            }
                
           */
        }

    }
}
