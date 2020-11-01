using ChatUI.Chat;
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

namespace ChatUI
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
            btn.clickEvent = StartChat;
        }

        private void StartChat(object sender, EventArgs e)
        {
            string user = username.textInput.Text;
            Pages.ChatPage chatPage;
            Client c;
            if(user.Trim() != "")
            {
                chatPage = new Pages.ChatPage();
                c = new Client(user, chatPage.UpdateUI);
                c.Start();
                chatPage.client = c;
                NavigationService.Navigate(chatPage);
            }
            else
                MessageBox.Show("Ingrese un usuario");
        }
    }
}
