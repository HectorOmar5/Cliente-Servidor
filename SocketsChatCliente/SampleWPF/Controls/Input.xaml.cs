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
    /// Lógica de interacción para Input.xaml
    /// </summary>
    public partial class Input : UserControl
    {
        public EventHandler onEnter;
        public Input()
        {
            InitializeComponent();
        }



        private void TextInput_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && onEnter != null)
            {
                onEnter(sender, e);
            }
        }
    }
}
