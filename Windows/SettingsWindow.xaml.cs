using System;
using System.Collections.Generic;
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
using System.Xml;

namespace ES_SYSTEM_K_Listy.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {


        public SettingsWindow()
        {
            InitializeComponent();
            XMLLocationTextBox.Text = App.Current.Properties["defaultXMLPath"].ToString();
        }

        private void refreshSettings()
        {
            XMLLocationTextBox.Text = App.Current.Properties["defaultXMLPath"].ToString();
        }
        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.InputPath = AppDomain.CurrentDomain.BaseDirectory.ToString();

            try
            {
                if (folderPicker.ShowDialog() == true)
                {
                    XMLLocationTextBox.Text = folderPicker.ResultPath;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Błąd podczas wybierania folderu: " + ex.Message,"BŁĄD!",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = XMLLocationTextBox.Text;
                string XMLPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_Folder_Location_Setting.txt";

                try
                {
                    using (StreamWriter writer = new StreamWriter(XMLPath, false))
                    {
                        writer.Write(path);
                        App.Current.Properties["defaultXMLPath"] = path;
                        refreshSettings();
                        writer.Close();
                        MessageBox.Show("Pomyślnie zapisano ściezkę", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Błąd podczas zapisywania ustawień do pliku, ścieżki zresetowane: " + ex.Message, "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
                    App.Current.Properties["defaultXMLPath"] = "";
                    refreshSettings();
                }
               
            }
            catch( Exception ex)
            {
                MessageBox.Show(ex.Message,"BŁĄD!",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
    }
}
