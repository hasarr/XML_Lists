using ES_SYSTEM_K_Listy.UserControls;
using System;
using System.Configuration;

using System.Windows;
using System.Windows.Controls;

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
            XMLFolderSettingControl.SelectFolderClick += XMLFolderSettingControl_SelectFolderClick;
            TC2ProgramsSettingControl.SelectFolderClick += TC2ProgramsSettingControl_SelectFolderClick; ;
            TC5ProgramsSettingControl.SelectFolderClick += TC5ProgramsSettingControl_SelectFolderClick; ;
        }

        private void TC5ProgramsSettingControl_SelectFolderClick(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.InputPath = AppDomain.CurrentDomain.BaseDirectory.ToString();

            try
            {
                if (folderPicker.ShowDialog() == true)
                {
                    TC5ProgramsSettingControl.LocationTextBox.Text = folderPicker.ResultPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wybierania folderu: " + ex.Message, "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TC2ProgramsSettingControl_SelectFolderClick(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.InputPath = AppDomain.CurrentDomain.BaseDirectory.ToString();

            try
            {
                if (folderPicker.ShowDialog() == true)
                {
                    TC2ProgramsSettingControl.LocationTextBox.Text = folderPicker.ResultPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wybierania folderu: " + ex.Message, "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void XMLFolderSettingControl_SelectFolderClick(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.InputPath = AppDomain.CurrentDomain.BaseDirectory.ToString();

            try
            {
                if (folderPicker.ShowDialog() == true)
                {
                    XMLFolderSettingControl.LocationTextBox.Text = folderPicker.ResultPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wybierania folderu: " + ex.Message, "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void refreshSettings()
        {
            XMLFolderSettingControl.LocationTextBox.Text = ConfigurationManager.AppSettings.Get("XMLPath");
            TC2ProgramsSettingControl.LocationTextBox.Text = ConfigurationManager.AppSettings.Get("TC2ProgramsPath");
            TC5ProgramsSettingControl.LocationTextBox.Text = ConfigurationManager.AppSettings.Get("TC5ProgramsPath");
        }
        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            
            var folderPicker = new FolderPicker();
            folderPicker.InputPath = AppDomain.CurrentDomain.BaseDirectory.ToString();

            try
            {
                if (folderPicker.ShowDialog() == true)
                {
                        XMLFolderSettingControl.LocationTextBox.Text = folderPicker.ResultPath;
                        TC2ProgramsSettingControl.LocationTextBox.Text = folderPicker.ResultPath;
                        TC5ProgramsSettingControl.LocationTextBox.Text = folderPicker.ResultPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas wybierania folderu: " + ex.Message, "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            //save XML Location
            try
            {
                string XMLpath = TC2ProgramsSettingControl.LocationTextBox.Text;
                string TC2ProgramsPath = TC2ProgramsSettingControl.LocationTextBox.Text;
                string TC5ProgramsPath = TC5ProgramsSettingControl.LocationTextBox.Text;

                try
                {
                    //save all options
                    Configuration Config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    Config.AppSettings.Settings["XMLPath"].Value = XMLpath;
                    Config.AppSettings.Settings["TC2ProgramsPath"].Value = TC2ProgramsPath;
                    Config.AppSettings.Settings["TC5ProgramsPath"].Value = TC5ProgramsPath;

                    ConfigurationManager.RefreshSection("appSettings");
                    Config.Save(ConfigurationSaveMode.Modified);
                    App.Current.Properties["defaultXMLPath"] = Config.AppSettings.Settings["XMLPath"].Value;
                    App.Current.Properties["TC2ProgramsPath"] = Config.AppSettings.Settings["TC2ProgramsPath"].Value;
                    App.Current.Properties["TC5ProgramsPath"] = Config.AppSettings.Settings["TC5ProgramsPath"].Value;

                    //refresh
                    refreshSettings();
                    MessageBox.Show("Pomyślnie zapisano ściezki", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas zapisywania ustawień do pliku, ścieżki zostaną zresetowane jeśli to możliwe: " + ex.Message, "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Configuration xmlConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    xmlConfig.AppSettings.Settings["XMLPath"].Value = "";
                    ConfigurationManager.RefreshSection("appSettings");
                    xmlConfig.Save(ConfigurationSaveMode.Modified);
                    App.Current.Properties["defaultXMLPath"] = xmlConfig.AppSettings.Settings["XMLPath"].Value;
                    refreshSettings();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Czy na pewno zresetować ścieżki? Zostaną one zmienione na domyślne i automatycznie zapisane", "UWAGA!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    XMLFolderSettingControl.LocationTextBox.Text = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_FILES";
                    TC2ProgramsSettingControl.LocationTextBox.Text = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\Programy\\TC2000R";
                    TC5ProgramsSettingControl.LocationTextBox.Text = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\Programy\\TC5000R";

                    Configuration Config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    Config.AppSettings.Settings["XMLPath"].Value = XMLFolderSettingControl.LocationTextBox.Text;
                    Config.AppSettings.Settings["TC2ProgramsPath"].Value = TC2ProgramsSettingControl.LocationTextBox.Text;
                    Config.AppSettings.Settings["TC5ProgramsPath"].Value = TC5ProgramsSettingControl.LocationTextBox.Text;

                    ConfigurationManager.RefreshSection("appSettings");
                    Config.Save(ConfigurationSaveMode.Modified);
                    App.Current.Properties["defaultXMLPath"] = Config.AppSettings.Settings["XMLPath"].Value;
                    App.Current.Properties["TC2ProgramsPath"] = Config.AppSettings.Settings["TC2ProgramsPath"].Value;
                    App.Current.Properties["TC5ProgramsPath"] = Config.AppSettings.Settings["TC5ProgramsPath"].Value;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd podczas zapisywania ustawień do pliku: " + ex.Message, "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
    }
}
