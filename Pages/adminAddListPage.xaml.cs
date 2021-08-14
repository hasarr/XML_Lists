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
using System.Windows.Threading;

namespace ES_SYSTEM_K_Listy
{
    /// <summary>
    /// Logika interakcji dla klasy adminAddListPage.xaml
    /// </summary>
    public partial class adminAddListPage : Page 
    {
     

        /// <summary>
        /// Save list added by admin from excel
        /// </summary>
        private void saveDatagridToXML()
        {
            //Create default columns to edit by user
           DataColumn tc2 = new DataColumn("TC 2000", typeof(bool)); tc2.DefaultValue = false;
           DataColumn tc5 = new DataColumn("TC 5000", typeof(bool)); tc5.DefaultValue = false;
           DataColumn isDone = new DataColumn("Zaczęte", typeof(bool)); isDone.DefaultValue = false;
           DataColumn isStarted = new DataColumn("Zakończone", typeof(bool)); isStarted.DefaultValue = false;
           DataColumn whoStarted = new DataColumn("Kto zaczął", typeof(string)); whoStarted.DefaultValue = string.Empty;
           DataColumn annotationColumn = new DataColumn("Adnotacja", typeof(string)); annotationColumn.DefaultValue = string.Empty;

            DataTable dt = new DataTable();
            //Add default columns to edit by user
            dt = ((DataView)productionListsDataGrid.WideDataGrid.ItemsSource).ToTable();
            dt.Columns.Add(annotationColumn);
            dt.Columns.Add(tc2);
            dt.Columns.Add(tc5);
            dt.Columns.Add(isDone);
            dt.Columns.Add(isStarted);
            dt.Columns.Add(whoStarted);
            
            try
            {
                dt.TableName = nameOfList.Text + " " + dateOfList.SelectedDate.Value.ToShortDateString();
                FileStream file = File.Create(App.Current.Properties["defaultXMLPath"] + "\\XML\\" + nameOfList.Text + " " + dateOfList.SelectedDate.Value.ToShortDateString() + ".xml");  
                dt.WriteXml(file, XmlWriteMode.WriteSchema,false);
                file.Close();
                MessageBox.Show("Zapisano");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Nie udało się zapisać: " + ex.Message.ToString());
            }
           
        }


        public adminAddListPage()
        {          
            InitializeComponent();
            dateOfList.SelectedDateFormat = DatePickerFormat.Short;
            defaultView();
            
        }

        #region onClickExcelDataRead
        //Take excel data after button is clicked
        private void test_Click(object sender, RoutedEventArgs e)
        {
         
            try
            {
                OpenFileDialog excelDialog = new OpenFileDialog();
                excelDialog.Filter = "Excel  | *.xls; *.xlsx";
                excelDialog.ShowDialog();
                string filePath = excelDialog.FileName;
                if (filePath == string.Empty) return;
               
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        DataSet result;
                        if (HeaderRowExcelComboBox.Text.ToString() == "2")
                        {
                             result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true,

                                    ReadHeaderRow = (rowReader) =>
                                  {
                                      
                                    //when the header row is set to 2, get list name if it exists from the 1st row
                                      for (int i = 0; i < rowReader.FieldCount; i++)
                                      {
                                          if (rowReader.GetFieldType(i).ToString().ToLower().Contains("string"))
                                          {
                                              if (rowReader.GetString(i) != "")
                                              {
                                                  nameOfList.Text = rowReader.GetString(i).ToString();
                                                  rowReader.Read();
                                                  return;
                                              }
                                          }
                                      }
                                      rowReader.Read(); 
                                  }


                                }
                            });
                        }

                        else
                        {
                             result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                                {
                                    UseHeaderRow = true,
                                }
                            });
                        }
                        
                        // The result of each spreadsheet is in result.Tables
                        if(result != null)
                         productionListsDataGrid.WideDataGrid.ItemsSource = result.Tables[0].DefaultView;
                        
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion
        public void defaultView()
        {
            dateOfList.SelectedDate = null;
            dateOfList.DisplayDate = DateTime.Today;
            productionListsDataGrid.WideDataGrid.ItemsSource = null;
            nameOfList.Text = string.Empty;
        }
        private void publishListButton_Click(object sender, RoutedEventArgs e)
        {
            
            // Check if name, date, and datagrid of the list is empty
            if (!productionListsDataGrid.WideDataGrid.Items.IsEmpty && nameOfList.Text != String.Empty && dateOfList.SelectedDate.ToString() != String.Empty) 
            {                
                  
               try
                {                   
                    //check if XML file with the name of the list exists
                    if (File.Exists(App.Current.Properties["defaultXMLPath"] + "\\XML\\" + nameOfList.Text + " " + dateOfList.SelectedDate.Value.ToShortDateString()+ ".xml"))
                    {
                        if (MessageBox.Show("Lista o takiej nazwie i dacie już istnieje, czy chcesz ją nadpisać?", "UWAGA!!",
                            MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                          
                            //overwrite existing file
                            saveDatagridToXML();
                            defaultView();
                            return;
                        }
                        else return;
                    }

                    //save the list if it's possible
                    
                    saveDatagridToXML();
                    defaultView();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Uzupełnij puste pola");
            }
            
           
        }
    }
}
