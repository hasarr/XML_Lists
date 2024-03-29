﻿using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Path = System.IO.Path;

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
        FileSystemWatcher filewatcherXML = new();
        FileSystemWatcher filewatcherXML_Public = new();
        FileSystemWatcher filewatcherXML_Done = new();
        #endregion

        /// <summary>
        /// Get an item from a ListView and transfer it's XML file to a datagrid
        /// </summary>
        /// /// <param name="list">Give a ListView name</param>
        /// <param name="item">Send an ListViewItem object in order to read from it</param>
        /// <param name="XmlPath">Give a specified path to an XML folder (no slashes at the end of the path)</param>
        /// <param name="listStatus">Specify the status of the list in this format: 'Status: ' + currentStatus</param>
        private void OpenListItemInDataGrid(ListView list, ListViewItem item, string XmlPath, string listStatus, string flexibleButtonContent, bool showSaveListButton)
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
                        MessageBox.Show("BŁĄD: " + ex.Message.ToString(), "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
                        deleteList(listPath);
                        refreshAdminPage();
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
                        if (MessageBox.Show("Lista nie zawiera elementów, czy chcesz ją usunąć?", "UWAGA!", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
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
                        for (int i = 1; i <= 5; i++) adminDataGrid.WideDataGrid.Columns[adminDataGrid.WideDataGrid.Columns.Count - i].IsReadOnly = true;
                    }
                }
                else
                {
                    MessageBox.Show("Wystąpił błąd z wyborem list, czy lista nadal znajduje się w katalogu?", "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (MessageBox.Show("Usunąć listę: " + selectedListTextBlock.Text + "?", "Uwaga!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("BŁĄD: " + ex.Message.ToString(), "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
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
            string[] xmlAdminFiles = Directory.GetFiles(App.Current.Properties["defaultXMLPath"] + "\\XML\\");
            foreach (String x in xmlAdminFiles)
            {
                string buffer = Path.GetFileName(x);
                adminListView.Items.Add(buffer.Remove(buffer.Length - 4));
            }

            //fill User ListView
            string[] xmlUserFiles = Directory.GetFiles(App.Current.Properties["defaultXMLPath"] + "\\XML_Public\\");
            foreach (String x in xmlUserFiles)
            {
                string buffer = Path.GetFileName(x);
                userListView.Items.Add(buffer.Remove(buffer.Length - 4));
            }

            //fill Ended ListView
            string[] xmlEndedFiles = Directory.GetFiles(App.Current.Properties["defaultXMLPath"] + "\\XML_Done\\");
            foreach (String x in xmlEndedFiles)
            {
                string buffer = Path.GetFileName(x);
                endedListView.Items.Add(buffer.Remove(buffer.Length - 4));
            }

        }



        private void adminListViewItemClick(object sender, MouseButtonEventArgs e)
        {

            OpenListItemInDataGrid(adminListView, sender as ListViewItem, App.Current.Properties["defaultXMLPath"] + "\\XML", "Status: niepubliczna", "Publikuj listę", true);


        }

        private void userListViewItemClick(object sender, MouseButtonEventArgs e)
        {
            OpenListItemInDataGrid(userListView, sender as ListViewItem, App.Current.Properties["defaultXMLPath"] + "\\XML_Public", "Status: publiczna", "Wycofaj listę", false);

        }

        private void endedListViewItemClick(object sender, MouseButtonEventArgs e)
        {
            OpenListItemInDataGrid(endedListView, sender as ListViewItem, App.Current.Properties["defaultXMLPath"] + "\\XML_Done", "Status: zakończona", string.Empty, false);

        }





        /// <summary>
        /// Default Constructor for AdminPanel
        /// </summary>
        public AdminPanel()
        {

            InitializeComponent();
            defaultView();
            refreshAdminPage();
            CreateFileWatcher(filewatcherXML_Public, App.Current.Properties["defaultXMLPath"].ToString() + "\\XML_Public");
            CreateFileWatcher(filewatcherXML, App.Current.Properties["defaultXMLPath"].ToString() + "\\XML");
            CreateFileWatcher(filewatcherXML_Done, App.Current.Properties["defaultXMLPath"].ToString() + "\\XML_Done");
        }

        private void CreateFileWatcher(FileSystemWatcher fileWatcher, string path)
        {

            fileWatcher.Path = path;
            fileWatcher.Filter = "*.xml";
            fileWatcher.Deleted += Filewatcher_Refresh; ;
            fileWatcher.Created += Filewatcher_Refresh;
            fileWatcher.EnableRaisingEvents = true;
        }

        private void Filewatcher_Refresh(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                try
                {
                    refreshAdminPage();
                    return;
                }
                catch (Exception)
                {
                    return;
                }
            }));
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
        /// <returns>Returns true if the list was moved, and false if it couldn't move the list</returns>
        private bool moveProductionList(string XmlPathFrom, string XmlPathTo)
        {
            if (File.Exists(XmlPathFrom + "\\" + selectedListTextBlock.Text + ".xml") && !(File.Exists(XmlPathTo + "\\" + selectedListTextBlock.Text + ".xml")))
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
                    MessageBox.Show(ex.Message.ToString(), "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
                    refreshAdminPage();
                    defaultView();
                    return false;
                }

            }
            else
            {
                MessageBox.Show("Nie przeniesiono", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Hand);
                return false;
            }
        }

        private DataGridCell getDataGridCell(DataGrid dataGridName, int indexOfColumn, int indexOfRow)
        {
            DataGridRow Row = (DataGridRow)dataGridName.ItemContainerGenerator.ContainerFromIndex(indexOfRow);
            if (Row != null)
            {
                try
                {
                    return dataGridName.Columns[indexOfColumn].GetCellContent(Row).Parent as DataGridCell;

                }
                catch (Exception)
                {
                    return null;
                }
            }
            else return null;
        }



        private DataTable searchForPrograms(DataGrid dataGridName, DataGridColumn TC2Column, DataGridColumn TC5Column)
        {
            DataView dataGridItems = (DataView)dataGridName.ItemsSource;
            DataTable dt = (dataGridItems).ToTable();

            for (int i=0;i<dataGridName.Items.Count-1;i++)
            {
                string TC2programMatch = null;
                string TC5programMatch = null;

                var programIndexColumn = getDataGridCell(adminDataGrid.WideDataGrid, 0, i);
                var ProgramIndexValue = ((TextBlock)programIndexColumn.Content).Text;

                var TC2ColumnCell = getDataGridCell(adminDataGrid.WideDataGrid, TC2Column.DisplayIndex, i);
                var TC2CellValue = ((CheckBox)TC2ColumnCell.Content).IsChecked.Value;

                var TC5ColumnCell = getDataGridCell(adminDataGrid.WideDataGrid, TC5Column.DisplayIndex, i);
                var TC5CellValue = ((CheckBox)TC5ColumnCell.Content).IsChecked.Value;

                var TC2ProgramsList = Directory.GetFiles(App.Current.Properties["TC2ProgramsPath"].ToString(),"*.lst",SearchOption.AllDirectories);
                var TC5ProgramsList = Directory.GetFiles(App.Current.Properties["TC5ProgramsPath"].ToString(), "*.lst", SearchOption.AllDirectories);

                try
                {
                    TC2programMatch = TC2ProgramsList.FirstOrDefault(stringToCheck => stringToCheck.Contains(ProgramIndexValue));
                    TC5programMatch = TC5ProgramsList.FirstOrDefault(stringToCheck => stringToCheck.Contains(ProgramIndexValue));

                    if (TC2programMatch != null)
                    {
                        dt.Rows[i][TC2Column.DisplayIndex] = true;
                    }
                    else
                    {
                        dt.Rows[i][TC2Column.DisplayIndex] = false;
                    }

                    if (TC5programMatch != null)
                    {
                        dt.Rows[i][TC5Column.DisplayIndex] = true;
                    }
                    else
                    {
                        dt.Rows[i][TC5Column.DisplayIndex] = false;
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show("BŁĄD SPRAWDZANIA PROGRAMÓW: " + ex.Message, "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }

               
            }

            return dt;
        }
        //Publish and Devoke the list
        private void flexibleAdminButton_Click(object sender, RoutedEventArgs e)
        {

            if (flexibleAdminButton.Content.ToString() == "Publikuj listę")
            {
                try
                {
                    if (MessageBox.Show("Na pewno opublikować listę: " + selectedListTextBlock.Text + "?",
                    "Uwaga!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        var TC2Column = adminDataGrid.WideDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "TC 2000");
                        var TC5Column = adminDataGrid.WideDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "TC 5000");

                        DataTable dt = searchForPrograms(adminDataGrid.WideDataGrid, TC2Column, TC5Column);
                        dt.WriteXml(App.Current.Properties["defaultXMLPath"] + "\\XML\\" + selectedListTextBlock.Text + ".xml", XmlWriteMode.WriteSchema, false);

                        moveProductionList(App.Current.Properties["defaultXMLPath"] + "\\XML", App.Current.Properties["defaultXMLPath"] + "\\XML_Public");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString(), "BŁĄD!", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                
            }
            else if (flexibleAdminButton.Content.ToString() == "Wycofaj listę")
            {
                if (MessageBox.Show("Na pewno wycofać listę: " + selectedListTextBlock.Text + "?",
                    "Uwaga!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    moveProductionList(App.Current.Properties["defaultXMLPath"] + "\\XML_Public", App.Current.Properties["defaultXMLPath"] + "\\XML");
                }
            }
            else return;

        }

        private void deleteListButton_Click(object sender, RoutedEventArgs e)
        {
            string path;
            if (listStatusTextBlock.Text == "Status: publiczna") path = App.Current.Properties["defaultXMLPath"] + "\\XML_Public\\" + selectedListTextBlock.Text + ".xml";
            else if (listStatusTextBlock.Text == "Status: niepubliczna") path = App.Current.Properties["defaultXMLPath"] + "\\XML\\" + selectedListTextBlock.Text + ".xml";
            else if (listStatusTextBlock.Text == "Status: zakończona") path = App.Current.Properties["defaultXMLPath"] + "\\XML_Done\\" + selectedListTextBlock.Text + ".xml";
            else
            {
                MessageBox.Show("Wystąpił nieznany błąd. Sprawdź lokalizacje pliku", "BŁĄD", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (deleteList(path))
            {
                MessageBox.Show("Usunięto pomyślnie", "INFORMACJA", MessageBoxButton.OK, MessageBoxImage.Information);
                defaultView();
                refreshAdminPage();
            }
            else MessageBox.Show("Nie udało się usunąć", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Error);





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
                MessageBox.Show("Zapisano zmiany", "INFORMACJA", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("BŁĄD: " + ex.Message.ToString(), "BŁĄD", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
