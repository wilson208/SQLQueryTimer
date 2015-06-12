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
using GalaSoft.MvvmLight.Messaging;
using SQLQueryTimer.Model;
using SQLQueryTimer.ViewModel;

namespace SQLQueryTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;

            this.MinHeight = this.Height;
            this.MinWidth = this.Width;
        }

        private void MenuItemRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() == typeof(MenuItem))
            {
                var query = GetQueryFromMenuitem((MenuItem)sender);
                if (query != null)
                {
                    var message = new KeyValuePair<string, Query>("RefreshQuery", query);
                    Messenger.Default.Send(message); 
                }
            }
        }

        private void MenuItemDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender.GetType() == typeof(MenuItem))
            {
                var query = GetQueryFromMenuitem((MenuItem)sender);
                if (query != null)
                {
                    var message = new KeyValuePair<string, Query>("DeleteQuery", query);
                    Messenger.Default.Send(message);
                }
            }
        }

        private Query GetQueryFromMenuitem(MenuItem menuItem)
        {
            var contextMenu = (ContextMenu)menuItem.Parent;
            var item = (DataGrid)contextMenu.PlacementTarget;
            if (item.SelectedCells.Count > 0)
            {
                return ((QueryViewModel) item.SelectedCells[0].Item).Query;
            }
            return null;
        }
    }
}
