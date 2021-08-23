using System;
using System.Windows;
using System.Windows.Controls;

namespace ES_SYSTEM_K_Listy.UserControls
{


    /// <summary>
    /// Logika interakcji dla klasy SettingSkin.xaml
    /// </summary>
    public partial class SettingSkin : UserControl
    {
        public static readonly DependencyProperty SettingTextDependency = DependencyProperty.Register(nameof(SettingText), typeof(string), typeof(SettingSkin));

        public string SettingText { get; set; }
        public event EventHandler<RoutedEventArgs> SelectFolderClick;

        public SettingSkin()
        {
            InitializeComponent();
            (Content as Grid).DataContext = this;
        }


        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectFolderClick != null)
                SelectFolderClick.Invoke(this, e);
        }
    }
}
