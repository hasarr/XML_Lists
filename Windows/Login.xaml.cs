using ES_SYSTEM_K_Listy.Windows;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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
            string XMLPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_Folder_Location_Setting.txt";
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_Folder_Location_Setting.txt"))
            {
                try
                {

                    using (StreamWriter writer = new StreamWriter(XMLPath, false))
                    {
                        writer.Write(AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_FILES");
                        App.Current.Properties["defaultXMLPath"] = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_FILES";
                        writer.Close();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nie można wygenerować pliku ustawień, aplikacja nie może działać: " + ex.ToString());

                    return;
                }
            }
            else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_Folder_Location_Setting.txt"))
            {

                try
                {
                    // Open the text file using a stream reader.
                    var sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_Folder_Location_Setting.txt");
                    
                        string path = sr.ReadLine();
                        //Check if the path is valid directory
                        if (Directory.Exists(path))
                        {
                            // Read the stream as a string and set the path of XML files
                            App.Current.Properties["defaultXMLPath"] = path;
                            sr.Close();

                        using (StreamWriter writer = new StreamWriter(XMLPath, false))
                        {
                            writer.Write(path);
                            App.Current.Properties["defaultXMLPath"] = path;
                            writer.Close();
                        }
                    }
                    
                        else
                        { 
                                MessageBox.Show("Bład formatu ściezki w pliku XML_Folder_Location_Setting.txt lub ścieżka nie istnieje. Aplikacja przywróci domyślną lokalizacje folderu XML", "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
                                sr.Close();
                            try
                            {
                                using (StreamWriter writer = new StreamWriter(XMLPath, false))
                                {
                                    writer.Write(AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_FILES");
                                    App.Current.Properties["defaultXMLPath"] = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_FILES";
                                    writer.Close();
                                }
                           

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Błąd podczas edycji pliku XML_Folder_Location_Setting.txt przez program, przywrócono domyślną ścieżkę: " + ex.Message, "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
                                    App.Current.Properties["defaultXMLPath"] = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_FILES";
                                }

                        }

                    

                    
                } 
                catch (IOException ex)
                {
                    MessageBox.Show("The XML settings file could not be read, XML path restored to default: " + ex.Message);
                    App.Current.Properties["defaultXMLPath"] = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_FILES";
                }

            }
            else
            {
                //Set default XML folders path if the path is not set by user
                App.Current.Properties["defaultXMLPath"] = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_FILES";
            }


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

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow window = new();
                window.ShowDialog();
        }
    }
}
