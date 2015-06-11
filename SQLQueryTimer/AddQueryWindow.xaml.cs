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
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;

namespace SQLQueryTimer
{
    /// <summary>
    /// Interaction logic for AddQueryWindow.xaml
    /// </summary>
    public partial class AddQueryWindow : Window
    {
        public AddQueryWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<string>(this, (message) =>
            {
                if (message.Equals("CloseAddQueryWindow"))
                {
                    Messenger.Default.Unregister(this);
                    this.Close();
                }
            });
        }
    }
}
