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
using Microsoft.Win32;
using System.Data;

using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using ExcelDataReader;
using Path = System.IO.Path;
using System.Collections.ObjectModel;
using System.Security.AccessControl;

namespace ES_SYSTEM_K_Listy
{
    /// <summary>
    /// Logika interakcji dla klasy AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        //Set of global variables
        #region GlobalVariables
        adminAddListPage addListPage = new();
        #endregion

        /// <summary>
        /// Get an item from a ListView and transfer it's XML file to a datagrid
        /// </summary>
        /// /// <param name="list">Give a ListView name</param>
        /// <param name="item">Send an ListViewItem object in order to read from it</param>
        /// <param name="XmlPath">Give a specified path to an XML folder (no slashes at the end of the path)</param>
        /// <param name="listStatus">Specify the status of the list in this format: 'Status: ' + currentStatus</param>
        private void OpenListItemInDataGrid(ListView list,ListViewItem item, string XmlPath, string listStatus, string flexibleButtonContent, bool showSaveListButton)
        {
            
            //set default values for a datagrid properties and restore default view
            adminDataGrid.WideDataGrid.IsReadOnly = true;
            adminDataGrid.WideDataGrid.CanUserAddRows = false;
            defaultView();
            

            //check if item is selected and if it contains something
            if (item != null && item.IsSelected)
            {
                //create a path variable
                string listPath = XmlPath + "\\" + list.SelectedItem.ToString() + ".xml";
                //check if file with the given path exists, if it does, then try opening the file
                if (File.Exists(listPath))
                {
                    DataSet data = new DataSet();
                    //read from xml
                    try
                    { 
                        data.ReadXml(listPath); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("BŁĄD: " + ex.Message.ToString());
                        defaultView();
                        return;
                    }

                    //check if data contains tables, if it does, proceed to read the list, if it doesn't return error
                    if (data.Tables.Count > 0)
                    {
                        adminDataGrid.WideDataGrid.ItemsSource = data.Tables[0].DefaultView;
                        mainFrame.Visibility = Visibility.Hidden;
                        adminDataGrid.Visibility = Visibility.Visible;
                        selectedListTextBlock.Text = list.SelectedItem.ToString();
                        listStatusTextBlock.Text = listStatus;
                        listStatusTextBlock.Visibility = Visibility.Visible;
                        deleteListButton.Visibility = Visibility.Visible;
                        if (showSaveListButton) saveListButton.Visibility = Visibility.Visible;
                    }
                    else if (data.Tables.Count <= 0)
                    {
                        if(MessageBox.Show("Lista nie zawiera elementów, czy chcesz ją usunąć?","UWAGA!",MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                        {
                            deleteList(listPath);
                            refreshAdminPage();
                            return;
                        }
                        else return;
                    }

                    //hide flexibleAdminButton if flexibleButtonContent has no content
                    if (flexibleButtonContent == string.Empty)
                    {
                        flexibleAdminButton.Visibility = Visibility.Hidden;
                    }
                    //show flexibleAdminButton if flexibleButtonContent has conteint in it and set this content to a button
                    else if (flexibleButtonContent != string.Empty)
                    {
                        flexibleAdminButton.Visibility = Visibility.Visible;
                        flexibleAdminButton.Content = flexibleButtonContent;
                    }

                    //Make fields of a datagrid editable if the list is not published
                    if (flexibleButtonContent == "Publikuj listę") 
                    { 
                        adminDataGrid.WideDataGrid.IsReadOnly = false;
                        adminDataGrid.WideDataGrid.CanUserAddRows = true;
                        //make last 5 columns readOnly, cause they should be only editable by user
                        for(int i=1;i<=5;i++) adminDataGrid.WideDataGrid.Columns[adminDataGrid.WideDataGrid.Columns.Count - i].IsReadOnly = true;
                    }
                }
                else
                {
                    MessageBox.Show("Wystąpił błąd z wyborem list, czy lista nadal znajduje się w katalogu?");
                    refreshAdminPage();
                    defaultView();
                }
                    
            }


        }

        /// <summary>
        /// Delete production list from adminpanel method
        /// </summary>
        /// <param name="path">Full path of a file to delete</param>
        /// <returns>Returns true if file was deleted, and false if delete failed</returns>
        private bool deleteList(string path)
        {
            if (MessageBox.Show("Na pewno usunąć listę: " + selectedListTextBlock.Text + "?","Uwaga!",MessageBoxButton.YesNo,MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (File.Exists(path))
                {
                    try 
                    { 
                        File.Delete(path); 
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("BŁĄD: " + ex.Message.ToString());
                        return false;
                    }
                    return true;
                }
                else return false;
            }
            else return false;
        }

       

        /// <summary>
        /// Retrieves default view (hides all the elements in the main panel)
        /// </summary>
        private void defaultView()
        {
            adminDataGrid.Visibility = Visibility.Hidden;
            mainFrame.Visibility = Visibility.Hidden;
            flexibleAdminButton.Visibility = Visibility.Hidden;
            listStatusTextBlock.Visibility = Visibility.Hidden;
            deleteListButton.Visibility = Visibility.Hidden;
            saveListButton.Visibility = Visibility.Hidden;
            listStatusTextBlock.Text = string.Empty;
            flexibleAdminButton.Content = string.Empty;
            selectedListTextBlock.Text = "Nie wybrano listy";
        }
        private void refreshAdminPage()
        {
            //clear all items from 3 lists on AdminPanel
            adminListView.Items.Clear();
            userListView.Items.Clear();
            endedListView.Items.Clear();

            //fill Admin ListView
            string [] xmlAdminFiles = Directory.GetFiles(App.Current.Properties["defaultXMLPath"] + "XML\\");
             foreach (String x in xmlAdminFiles)
            {
                string buffer = Path.GetFileName(x);
                adminListView.Items.Add(buffer.Remove(buffer.Length - 4));
            }

            //fill User ListView
            string[] xmlUserFiles = Directory.GetFiles(App.Current.Properties["defaultXMLPath"] + "XML_Public\\");
            foreach (String x in xmlUserFiles)
            {
                string buffer = Path.GetFileName(x);
                userListView.Items.Add(buffer.Remove(buffer.Length - 4));
            }

            //fill Ended ListView
            string[] xmlEndedFiles = Directory.GetFiles(App.Current.Properties["defaultXMLPath"] + "XML_Done\\");
            foreach (String x in xmlEndedFiles)
            {
                string buffer = Path.GetFileName(x);
                endedListView.Items.Add(buffer.Remove(buffer.Length - 4));
            }

        }



        private void adminListViewItemClick(object sender, MouseButtonEventArgs e)
        {

            OpenListItemInDataGrid(adminListView,sender as ListViewItem, App.Current.Properties["defaultXMLPath"] + "\\XML", "Status: niepubliczna", "Publikuj listę", true);
            

        }

        private void userListViewItemClick(object sender, MouseButtonEventArgs e)
        {
            OpenListItemInDataGrid(userListView, sender as ListViewItem, App.Current.Properties["defaultXMLPath"] + "\\XML_Public", "Status: publiczna", "Wycofaj listę",false);

        }

        private void endedListViewItemClick(object sender, MouseButtonEventArgs e)
        {
            OpenListItemInDataGrid(endedListView, sender as ListViewItem, App.Current.Properties["defaultXMLPath"] + "\\XML_Done", "Status: zakończona", string.Empty,false);

        }



        

        /// <summary>
        /// Default Constructor for AdminPanel
        /// </summary>
        public AdminPanel()
        {
                
            InitializeComponent();
            defaultView();
            refreshAdminPage();


        }

        /// <summary>
        /// Add List button onClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addListButton_Click(object sender, RoutedEventArgs e)
        {
            
            //clear content from AddListPage
            addListPage.defaultView();

            defaultView();
            //set header text
            selectedListTextBlock.Text = "Dodaj listę";
            mainFrame.Visibility = Visibility.Visible;
            mainFrame.Content = addListPage;
            refreshAdminPage();

        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            refreshAdminPage();
        }

        //TODO: ZROBIĆ KURWA TRY CATCH W CZYTANIU XML PLS KURWA NO

        /// <summary>
        /// Chenge the location of a productionList
        /// </summary>
        /// <param name="XmlPathFrom">Original path of the list </param>
        /// <param name="XmlPathTo">Path to move the list to</param>
        /// /// <param name="listViewDestinationHeader">Determine where you want to move the list (public or not public listView)</param>
        /// <returns>Returns true if the list was moved, and false if it couldn't move the list</returns>
        private bool moveProductionList(string XmlPathFrom, string XmlPathTo, string listViewDestinationHeader)
        {
            if (File.Exists(XmlPathFrom + "\\" + selectedListTextBlock.Text + ".xml") &&
                    MessageBox.Show("Na pewno przenieść listę: " + selectedListTextBlock.Text + " do katalogu " + listViewDestinationHeader + "?",
                    "Uwaga!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes
                    && !(File.Exists(XmlPathTo + "\\" + selectedListTextBlock.Text + ".xml")))
            {

                try
                {
                    File.Move(XmlPathFrom + "\\" + selectedListTextBlock.Text + ".xml",
                    XmlPathTo + "\\" + selectedListTextBlock.Text + ".xml");


                    refreshAdminPage();
                    defaultView();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    refreshAdminPage();
                    defaultView();
                    return false;
                }

            }
            else 
            {
                MessageBox.Show("Nie przeniesiono");
                return false; 
            }
        }


        //Publish and Devoke the list
        private void flexibleAdminButton_Click(object sender, RoutedEventArgs e)
        {

            if (flexibleAdminButton.Content.ToString() == "Publikuj listę")
            {
                moveProductionList(App.Current.Properties["defaultXMLPath"] + "\\XML", App.Current.Properties["defaultXMLPath"] + "\\XML_Public", "publiczna");
            }
            else if (flexibleAdminButton.Content.ToString() == "Wycofaj listę")
            {
                moveProductionList(App.Current.Properties["defaultXMLPath"] + "\\XML_Public", App.Current.Properties["defaultXMLPath"] + "\\XML", "niepubliczna");
            }
            else  return;
            
        }

        private void deleteListButton_Click(object sender, RoutedEventArgs e)
        {
            string path;
            if (listStatusTextBlock.Text == "Status: publiczna") path = App.Current.Properties["defaultXMLPath"] + "\\XML_Public\\" + selectedListTextBlock.Text + ".xml";
            else if (listStatusTextBlock.Text == "Status: niepubliczna") path = App.Current.Properties["defaultXMLPath"] + "\\XML\\" + selectedListTextBlock.Text + ".xml";
            else if (listStatusTextBlock.Text == "Status: zakończona") path = App.Current.Properties["defaultXMLPath"] + "\\XML_Done\\" + selectedListTextBlock.Text + ".xml";
            else
            {
                MessageBox.Show("Wystąpił nieznany błąd. Sprawdź lokalizacje pliku");
                return;
            }
            if (deleteList(path))
            { 
                MessageBox.Show("Usunięto pomyślnie");
                defaultView();
                refreshAdminPage();
            }
            else MessageBox.Show("Nie udało się usunąć");

            
            
            

        }

        private void saveListButton_Click(object sender, RoutedEventArgs e)
        {
            string file;
            file = App.Current.Properties["defaultXMLPath"] + "\\XML\\" + selectedListTextBlock.Text + ".xml";
            DataTable dt = ((DataView)adminDataGrid.WideDataGrid.ItemsSource).ToTable();

            try
            {
                dt.WriteXml(file,
              XmlWriteMode.WriteSchema, false);
                MessageBox.Show("Zapisano zmiany");
            }
            catch(Exception ex)
            {
                MessageBox.Show("BŁĄD: " + ex.Message.ToString());
            }
        }
    }
}
