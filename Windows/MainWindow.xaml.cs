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
using System.Threading;
using System.Security.AccessControl;
using System.Windows.Controls.Primitives;
using ES_SYSTEM_K_Listy.Windows;

namespace ES_SYSTEM_K_Listy
{

   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
     
        #region GlobalVariables
            DataSet defaultData = new DataSet();
        FileSystemWatcher fileWatcher = new();
        private List<string> sortDirections = new List<string>();
            
        #endregion

        /// <summary>
        /// Main Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
           

            if (App.Current.Properties["defaultXMLPath"].ToString() == "" || App.Current.Properties["defaultXMLPath"] == null)
                App.Current.Properties["defaultXMLPath"] = AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\XML_FILES";
            //Create default directories if they dont exist and return true if creation was complete, or false if the program couldnt create the directories, also
            //close the MainWindow if directories weren't created
            if(makeDefaultDirectories(App.Current.Properties["defaultXMLPath"].ToString() + "\\XML")&&
            makeDefaultDirectories(App.Current.Properties["defaultXMLPath"].ToString() + "\\XML_Public")&&
            makeDefaultDirectories(App.Current.Properties["defaultXMLPath"].ToString() + "\\XML_Done"))
             {
                App.Current.Properties["mainWindowConstructorStatus"] = true;
            }
            else {
                App.Current.Properties["mainWindowConstructorStatus"] = false;
                MessageBox.Show("Nie można uruchomić programu, błąd podczas ustalania domyślnych ścieżek XML, sprawdź uprawnienia","BŁĄD!",MessageBoxButton.OK,MessageBoxImage.Error); this.Close(); return;
            }

            refreshUserPage();
            defaultView();

            //TODO: set the directory where program folders are

            //Hide Admin Panel button if user is not an admin
            if (!(bool)App.Current.Properties["isAdmin"])
            {
                admin_panel_button.Visibility = Visibility.Hidden;
            }
            CreateFileWatcher(App.Current.Properties["defaultXMLPath"].ToString() + "\\XML_Public");
            //Set events for starting and ending edit in a datagrid
            UserWindowDataGridControl.BegininngdEdit += UserWindowDataGridControl_BeginningEdit;
            UserWindowDataGridControl.CellEditEnding += UserWindowDataGridControl_CellEditEnding;
            loginInfoTextBlock.Text += App.Current.Properties["username"].ToString();
        }

        /// <summary>
        /// Gets a cell value from currently selected row index, and given column index
        /// </summary>
        /// <param name="dataGridName"></param>
        /// <param name="indexOfColumn"></param>
        /// <returns>return dataGridCell value or null if there is no selected row value</returns>
        private DataGridCell getSelectedDataGridCell(DataGrid dataGridName, int indexOfColumn)
        {
            DataGridRow Row = (DataGridRow)dataGridName.ItemContainerGenerator.ContainerFromIndex(dataGridName.SelectedIndex);
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


        private void UserWindowDataGridControl_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if ((bool)App.Current.Properties["isAdmin"])
            {
                e.Cancel = true;
                MessageBox.Show("Aby edytować listę jako admin, wycofaj ją najpierw z publicznych list w panelu admina");
                return;
            }
            
            string filePath = App.Current.Properties["defaultXMLPath"] + "\\XML_Public\\" + selectedListTextBlock.Text + ".xml";
            if (!File.Exists(filePath))
            {
                defaultView();
                refreshUserPage();
                MessageBox.Show("Lista już nie istnieje");
                e.Cancel = true;
                return;
            }

            if (e.Column.Header.ToString() == "Zaczęte" || e.Column.Header.ToString() == "Zakończone")
            {
                //get index of who started column
                var whoStartedColumn = UserWindowDataGridControl.WideDataGrid.Columns.FirstOrDefault(column => column.Header.ToString() == "Kto zaczął");
                var indexWhoStarted = UserWindowDataGridControl.WideDataGrid.Columns.IndexOf(whoStartedColumn);
                var endedColumn = UserWindowDataGridControl.WideDataGrid.Columns.FirstOrDefault(column => column.Header.ToString() == "Zakończone");
                var startedColumn = UserWindowDataGridControl.WideDataGrid.Columns.FirstOrDefault(column => column.Header.ToString() == "Zaczęte");
                int endedColumnIndex = endedColumn.DisplayIndex;
                int startedColumnIndex = startedColumn.DisplayIndex;

                DataGridCell whoStartedCell = getSelectedDataGridCell(UserWindowDataGridControl.WideDataGrid, indexWhoStarted);
                DataGridCell endedColumnCell = getSelectedDataGridCell(UserWindowDataGridControl.WideDataGrid, endedColumnIndex);
                DataGridCell startedColumnCell = getSelectedDataGridCell(UserWindowDataGridControl.WideDataGrid, startedColumnIndex);

                bool endedColumnValue = ((CheckBox)endedColumnCell.Content).IsChecked.Value;
                bool startedColumnValue = ((CheckBox)startedColumnCell.Content).IsChecked.Value;

                if (whoStartedCell == null) return;

                string whoStartedCellValue = ((TextBlock)whoStartedCell.Content).Text;

                

                if (whoStartedCellValue.ToString() == string.Empty || whoStartedCellValue.ToString() == App.Current.Properties["username"].ToString())
                {
                    if (e.Column.Header.ToString() == "Zaczęte" && endedColumnValue == false)
                    {
                        e.Cancel = false; 
                    }
                    else if (e.Column.Header.ToString() == "Zakończone" && startedColumnValue == true)
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    e.Cancel = true;
                }
                
            }
            else e.Cancel = true;
        }


        private void UserWindowDataGridControl_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string filePath = App.Current.Properties["defaultXMLPath"] + "\\XML_Public\\" + selectedListTextBlock.Text + ".xml";
            if(!File.Exists(filePath))
            {
                defaultView();
                refreshUserPage();
                MessageBox.Show("Lista już nie istnieje");
                e.Cancel = true;
                return;
            }

            var whoStartedColumn = UserWindowDataGridControl.WideDataGrid.Columns.FirstOrDefault(column => column.Header.ToString() == "Kto zaczął");
            int indexWhoStarted = whoStartedColumn.DisplayIndex;

            DataGridCell whoStartedCell = getSelectedDataGridCell(UserWindowDataGridControl.WideDataGrid, indexWhoStarted);
            DataGridCell currentCell = getSelectedDataGridCell(UserWindowDataGridControl.WideDataGrid, e.Column.DisplayIndex);

            if (whoStartedCell == null) return;
            if (currentCell == null) return;

            string whoStartedCellValue = ((TextBlock)whoStartedCell.Content).Text;


            //internal function for saving data
            void saveData(bool valueToSave, string whoStartedValue, bool saveWhoStartedValue)
            {
                try
                {
                    //get Items from DataGrid
                    DataView dataGridItems = (DataView)UserWindowDataGridControl.WideDataGrid.ItemsSource;
                    DataTable dt = (dataGridItems).ToTable();

                    //save WhoStartedValue if saveWhoStartedValue is true
                    if(saveWhoStartedValue) dt.Rows[e.Row.GetIndex()][indexWhoStarted] = whoStartedValue;
                    //save value for cell
                    dt.Rows[e.Row.GetIndex()][e.Column.DisplayIndex] = valueToSave;
                    dataGridItems = dt.DefaultView;
                    dataGridItems.Sort = dt.Columns[0].ColumnName + " ASC";
                    dt = dataGridItems.ToTable();
                    //finally save to xml
                    dt.WriteXml(filePath, XmlWriteMode.WriteSchema, false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    e.Cancel = true;
                    defaultView();
                    return;
                }
            }

            

                if (whoStartedCellValue.ToString() == string.Empty || whoStartedCellValue.ToString() == App.Current.Properties["username"].ToString())
                {
                    if (e.Column.Header.ToString() == "Zaczęte" || e.Column.Header.ToString() == "Zakończone")
                    {
                    
                        bool currentCellValue = ((CheckBox)currentCell.Content).IsChecked.Value;


                        if (currentCellValue && e.Column.Header.ToString() == "Zaczęte")
                        {
                            saveData(true, App.Current.Properties["username"].ToString(), true);
                        }
                        else if (currentCellValue == false && App.Current.Properties["username"].ToString() == whoStartedCellValue && e.Column.Header.ToString() == "Zaczęte")
                        {
                            saveData(false, string.Empty, true);
                        }
                        else if (currentCellValue && e.Column.Header.ToString() == "Zakończone")
                        {
                            saveData(true, string.Empty, false);
                        }
                        else if (currentCellValue == false && e.Column.Header.ToString() == "Zakończone")
                        {
                            saveData(false, string.Empty, false);
                        }


                    }
                    

                }
                else e.Cancel = true;
            
 
        }



        /// <summary>
        /// Make a directory of given path
        /// </summary>
        /// <param name="defaultXmlFolderPath">Path with name of the directory to be created</param>
        /// <returns></returns>
        private bool makeDefaultDirectories(string defaultXmlFolderPath)
        {
            if (!Directory.Exists(defaultXmlFolderPath))
                try
                {
                    Directory.CreateDirectory(defaultXmlFolderPath);

                    if (Directory.Exists(defaultXmlFolderPath))
                    {
                   
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Wystąpił problem z tworzeniem domyślnego katalogu XML, aplikacja nie może działać");

                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());

                    return false;
                }
            else if (Directory.Exists(defaultXmlFolderPath)) return true; 

            else return false;
        }

        /// <summary>
        /// Create a file watcher that watches for changes in a file and then handles the event when file is changed, it is used to 
        /// refresh the dataGrid when content is changed in XML file
        /// </summary>
        /// <param name="path">full path of a XML file with a name and .xml extension</param>
        private void CreateFileWatcher(string path)
        {
            
            fileWatcher.Path = path;
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.Size | NotifyFilters.FileName ;
            fileWatcher.Filter = "*.xml";
            fileWatcher.Changed += FileWatcher_Changed;
            fileWatcher.Deleted += FileWatcher_Deleted; ;
            fileWatcher.Created += FileWatcher_Changed;
            fileWatcher.EnableRaisingEvents = true;
        }

        private void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
            {
                try
                {

                    if ((bool)App.Current.Properties["isAdmin"] == false && e.Name.Contains(selectedListTextBlock.Text))
                    {
                        refreshDataGrid(UserWindowDataGridControl.WideDataGrid, userListView, App.Current.Properties["defaultXMLPath"] + "\\XML_Public");
                        refreshUserPage();
                        defaultView();
                        MessageBox.Show("Aktualnie wybrana lista została wycofana przez administratora!", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Stop);
                    }
                    else
                    {
                        refreshDataGrid(UserWindowDataGridControl.WideDataGrid, userListView, App.Current.Properties["defaultXMLPath"] + "\\XML_Public");
                        refreshUserPage();
                    }
                    
                    
                    return;
                }
                catch (Exception)
                {
                    return;
                }
            }));
        }


        /// <summary>
        /// Event for filewatcher changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)(() =>
             {
                 try
                 {
                     refreshDataGrid(UserWindowDataGridControl.WideDataGrid, userListView, App.Current.Properties["defaultXMLPath"] + "\\XML_Public");
                     refreshUserPage();
                     return;
                 }
                 catch(Exception)
                 {
                     return;
                 }
             }));
        }

        /// <summary>
        /// Returns false, if file wasn't read and true if file was read
        /// </summary>
        /// <param name="path">Need full path with an XML folder</param>
        /// <returns></returns>
        private bool readList(string path, Button endListButton, DataGrid dataGridName, TextBlock selectedListInfoTextBlock, ListView selectedList)
        {
            string fullPath = path + "\\" + selectedList.SelectedItem.ToString() + ".xml";
            FileStream file;
            defaultView();
            if (!File.Exists(fullPath)) { defaultView(); return false; }
            try
            { 
                file = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite); 
            }
            catch(Exception x)
            {
                MessageBox.Show(x.Message.ToString());
                defaultView();
                refreshUserPage();
                return false;
            }

            //Set Visibility of a Grid and End button to Visible and clear all columns
            endListButton.Visibility = Visibility.Visible;
            dataGridName.Visibility = Visibility.Visible;
            dataGridName.Columns.Clear();
            refreshDataGrid(dataGridName, selectedList, path);
            
            file.Close();
            return true;
        }
       
        /// <summary>
        /// Refresh dataGrid
        /// </summary>
        /// <param name="dataGridName"></param>
        /// <param name="selectedList"></param>
        /// <param name="path"></param>
        private void refreshDataGrid(DataGrid dataGridName, ListView selectedList, string path)
        {

            bool transferFileToDatagrid(string fullPath)
            {
                FileStream file;

                try
                {
                    if (File.Exists(fullPath))
                        file = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    else
                    {
                        file = null;
                        return false;
                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.Message.ToString());
                    return false;
                }

                try
                {
                    if (file != null)
                    {
                        defaultData.Tables.Clear();
                        defaultData.ReadXml(file);
                        file.Dispose();
                        file.Close();
                        return true;
                    }
                }
                catch (Exception)
                {
                    //tutaj jest root element missing
                    file.Dispose();
                    file.Close();
                    return false;
                }

                return false;
            }

            //check if user is selecting a new list or refreshing
            if (selectedList.SelectedItem != null && selectedListTextBlock.Text == "Nie wybrano listy")
            {
                
    

                if (transferFileToDatagrid(path + "\\" + selectedList.SelectedItem.ToString() + ".xml")) selectedListTextBlock.Text = selectedList.SelectedItem.ToString();
                else return;

                //set itemssource for a datagrid
                if (defaultData.Tables.Count > 0)
                {
                    //set source for items
                    UserWindowDataGridControl.WideDataGrid.ItemsSource = defaultData.Tables[0].DefaultView;
                    //set default sorting
                    dataGridName.Items.SortDescriptions.Clear();
                    dataGridName.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription(dataGridName.Columns[0].SortMemberPath, System.ComponentModel.ListSortDirection.Ascending));
                    dataGridName.Columns[0].SortDirection = System.ComponentModel.ListSortDirection.Ascending;

                    //Reset List with sortdirections 
                    if (sortDirections.Count > 0)
                        sortDirections.Clear();

                    //Add sort direction field to list
                    foreach (DataGridColumn e in dataGridName.Columns)
                    {
                        sortDirections.Add(e.SortDirection.ToString());
                    }
                }
                else return;

                selectedListTextBlock.Text = selectedList.SelectedItem.ToString();
            }
            //else if the list is selected, refresh the datagrid
            else if (selectedListTextBlock.Text.ToString() != "Nie wybrano listy" && selectedListTextBlock.Text.ToString() != string.Empty)
            {

                if (!transferFileToDatagrid(path + "\\" + selectedListTextBlock.Text.ToString() + ".xml")) return;
                //Reset List with sortdirections 
                if (sortDirections.Count > 0)
                    sortDirections.Clear();

                //Add sort direction field to list
                foreach (DataGridColumn e in dataGridName.Columns)
                {
                    sortDirections.Add(e.SortDirection.ToString());
                }

                //set itemssource for a datagrid
                if (defaultData.Tables.Count > 0)
                {
                    UserWindowDataGridControl.WideDataGrid.ItemsSource = defaultData.Tables[0].DefaultView;
                }
                else return;


                //After ItemsSource is changed, retrieve the sorting order before refreshing
                if (sortDirections.Count > 0)
                {
                    int i = 0;
                    dataGridName.Items.SortDescriptions.Clear();

                    //Set sorting after datagrid reloading
                    foreach (DataGridColumn e in dataGridName.Columns)
                    {
                        if (sortDirections[i].ToString().ToLower() == "ascending")
                        {
                            dataGridName.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription(e.SortMemberPath, System.ComponentModel.ListSortDirection.Ascending));
                            e.SortDirection = System.ComponentModel.ListSortDirection.Ascending;
                            return;
                        }
                        else if (sortDirections[i].ToString().ToLower() == "descending")
                        {
                            dataGridName.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription(e.SortMemberPath, System.ComponentModel.ListSortDirection.Descending));
                            e.SortDirection = System.ComponentModel.ListSortDirection.Descending;
                            return;
                        }
                        i++;
                    }
                }
            }
            else return;


            

            foreach (DataGridColumn column in dataGridName.Columns)
            {
                column.IsReadOnly = true;
            }

            //make ended and started fields editable in datagrid
            dataGridName.CanUserAddRows = false;
            var startedColumn = dataGridName.Columns.FirstOrDefault(c => c.Header.ToString() == "Zaczęte");
            var endedColumn = dataGridName.Columns.FirstOrDefault(c => c.Header.ToString() == "Zakończone");

            dataGridName.Columns[dataGridName.Columns.IndexOf(endedColumn)].IsReadOnly = false;
            dataGridName.Columns[dataGridName.Columns.IndexOf(startedColumn)].IsReadOnly = false;

            
            
        }


        //when item on a listview is clicked, show it on a datagrid
        private void userListViewItemClick(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            
            if (item != null && item.IsSelected)
            {
                if (!readList(App.Current.Properties["defaultXMLPath"] + "\\XML_Public", endListButton, UserWindowDataGridControl.WideDataGrid, selectedListTextBlock, userListView))
                { 
                    MessageBox.Show("Nie udało się odczytać listy, sprawdź czy lista nadal jest dostępna");
                    refreshUserPage();
                }
            }

        }

        //refresh all lists
        private async void refreshUserPage()
        {
            
            userListView.Items.Clear();
            string[] xmlAdminFiles = Directory.GetFiles(App.Current.Properties["defaultXMLPath"]  + "\\XML_Public\\");
            if(xmlAdminFiles.Length == 0)
            {
                defaultView();
                return;
            }
            foreach (String x in xmlAdminFiles)
            {
                string buffer = Path.GetFileName(x);
                buffer = buffer.Remove(buffer.Length - 4);
                userListView.Items.Add(buffer);
               
            }
        }


        /// <summary>
        /// Restore default View of the app in the MainWindow 
        /// </summary>
        private async void defaultView()
        {
            UserWindowDataGridControl.WideDataGrid.Columns.Clear();
            UserWindowDataGridControl.WideDataGrid.Visibility = Visibility.Hidden;
            endListButton.Visibility = Visibility.Hidden;
            selectedListTextBlock.Text = "Nie wybrano listy";
        }
        

        private void admin_panel_button_Click(object sender, RoutedEventArgs e)
        {
            //If button is clicked, check for admin priviliges and open Admin Panel

            if ((bool)App.Current.Properties["isAdmin"])
            {
                AdminPanel adminPanel = new AdminPanel();
                adminPanel.ShowDialog();
            }
        }

        private void refreshUserPage_Click(object sender, RoutedEventArgs e)
        {
            refreshUserPage();
        }

        /// <summary>
        /// Search if given DataGridColumn contains any unchecked fields, if it does, then return false, if all fields are checked return true
        /// </summary>
        /// <param name="dataGridName"></param>
        /// <param name="columnToCheck"></param>
        /// <returns></returns>
        private bool checkForEnded(DataGrid dataGridName, DataGridColumn columnToCheck)
        {
            if (dataGridName.Items.Count <= 0) return false;

            int i = 0;
            //check every row for False value in columnToCheck
            foreach(DataRowView x in dataGridName.Items)
            {
                string currentCellValue = columnToCheck.GetCellContent(dataGridName.Items[i]).ToString();
                if (currentCellValue.Contains("False"))
                {
                    MessageBox.Show("Nie oznaczono wszystkich detali jako zakończone","UWAGA!",MessageBoxButton.OK,MessageBoxImage.Stop);
                    return false;
                }
                i++;
            }
            return true;
        }


        /// <summary>
        /// End the List as a user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void endListButton_Click(object sender, RoutedEventArgs e)
        {
            var endedColumn = UserWindowDataGridControl.WideDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Zakończone");
            

            if (MessageBox.Show("Czy na pewno chcesz zakończyć listę? Nie można tego cofnąć", "UWAGA!!",
                           MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes
                           && checkForEnded(UserWindowDataGridControl.WideDataGrid, endedColumn))
            {
                
                if (File.Exists(App.Current.Properties["defaultXMLPath"] + "\\XML_Public\\" + selectedListTextBlock.Text + ".xml"))
                {
                    if (!File.Exists(App.Current.Properties["defaultXMLPath"] + "\\XML_Done\\" + selectedListTextBlock.Text + ".xml"))
                    {
                        try
                        {
                            File.Move(App.Current.Properties["defaultXMLPath"] + "\\XML_Public\\" + selectedListTextBlock.Text + ".xml", App.Current.Properties["defaultXMLPath"] + "\\XML_Done\\" + selectedListTextBlock.Text + ".xml");
                            defaultView();
                            refreshUserPage();
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("BŁĄD: " + ex.Message.ToString());
                        }
                    }
                    else { 
                        MessageBox.Show("Nie udało się zapisać, lista o takiej samej nazwie jest już oznaczona jako zakończona");
                        try
                        {
                            File.Delete(App.Current.Properties["defaultXMLPath"] + "\\XML_Public\\" + selectedListTextBlock.Text + ".xml");
                            defaultView();
                            refreshUserPage();
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("Błąd podczas przetwarzania listy: " + ex.Message.ToString());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Plik już nie istnieje, lub lista została wycofana");
 
                }
            }
            else
            {      
                return;
            }
            
        }


        private void infoButton_Click(object sender, RoutedEventArgs e)
        {
            Info infoWIndow = new();
            infoWIndow.ShowDialog();
        }
    }
}
    


