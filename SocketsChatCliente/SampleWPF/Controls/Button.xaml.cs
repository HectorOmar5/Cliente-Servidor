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

namespace ChatUI.Controls
{
    /// <summary>
    /// Lógica de interacción para Button.xaml
    /// </summary>
    public partial class Button : UserControl
    {
        public EventHandler clickEvent { set; get; }
        public string Text { get; set; }
        public Button()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            if (this.clickEvent != null) clickEvent(sender, e);
        }
    }
}
