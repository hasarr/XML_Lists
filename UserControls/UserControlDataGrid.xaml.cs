using System;
using System.Windows;
using System.Windows.Controls;

namespace ES_SYSTEM_K_Listy.UserControls
{
    /// <summary>
    /// Logika interakcji dla klasy UserControlDataGrid.xaml
    /// </summary>
    public partial class UserControlDataGrid : UserControl
    {
        public static readonly DependencyProperty CanUserAddRowsDependency = DependencyProperty.Register(nameof(CanUserAddRows), typeof(string), typeof(UserControlDataGrid));
        public static readonly DependencyProperty ItemsSourceContentDependency = DependencyProperty.Register(nameof(ItemsSourceContent), typeof(string), typeof(UserControlDataGrid));
        public static readonly DependencyProperty DataContextContentDependency = DependencyProperty.Register(nameof(DataContextContent), typeof(string), typeof(UserControlDataGrid));
        public event EventHandler<DataGridBeginningEditEventArgs> BegininngdEdit;
        public event EventHandler<DataGridCellEditEndingEventArgs> CellEditEnding;
        public string CanUserAddRows { get; set; }
        public string ItemsSourceContent { get; set; }
        public string DataContextContent { get; set; }

        public UserControlDataGrid()
        {

            InitializeComponent();
            (Content as Grid).DataContext = this;
        }

        private void WideDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (BegininngdEdit != null)
                BegininngdEdit.Invoke(this, e);
        }

        private void WideDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (BegininngdEdit != null)
                CellEditEnding.Invoke(this, e);
        }
    }
}
