using System;
using System.Collections.Generic;
using System.Configuration;
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
            refreshSettings();
        }

        private void refreshSettings()
        {
            XMLLocationTextBox.Text = ConfigurationManager.AppSettings.Get("XMLPath");
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

                try
                {
                    Configuration xmlConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    xmlConfig.AppSettings.Settings["XMLPath"].Value = path;
                    ConfigurationManager.RefreshSection("appSettings");
                    xmlConfig.Save(ConfigurationSaveMode.Modified);
                    App.Current.Properties["defaultXMLPath"] = xmlConfig.AppSettings.Settings["XMLPath"].Value;
                    refreshSettings();
                    MessageBox.Show("Pomyślnie zapisano ściezkę", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Błąd podczas zapisywania ustawień do pliku, ścieżki zresetowane: " + ex.Message, "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Configuration xmlConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    xmlConfig.AppSettings.Settings["XMLPath"].Value = "";
                    ConfigurationManager.RefreshSection("appSettings");
                    xmlConfig.Save(ConfigurationSaveMode.Modified);
                    App.Current.Properties["defaultXMLPath"] = xmlConfig.AppSettings.Settings["XMLPath"].Value;
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
