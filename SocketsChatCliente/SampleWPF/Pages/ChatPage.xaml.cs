using ChatUI.Chat;
using Chat;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ChatUI.Pages
{
    /// <summary>
    /// Lógica de interacción para ChatPage.xaml
    /// </summary>
    public partial class ChatPage : Page
    {
        public Client client;
        User _context;
        User context
        {
            set
            {
                if (value != null)
                {
                    _context = value;
                    this.ShowMessages();
                    this.Username.Content = value.nick;
                }
            }
            get { return _context; }
        }
        public ChatPage()
        {
            InitializeComponent();
            context = null;
            msgInput.onEnter = SendMessage;

        }
        private void SendMessage(object sender, EventArgs e)
        {
            Message msg;
            string message = ((TextBox)sender).Text;
            if (this.context != null && message.Trim() != "")
            {
                msg = this.client.SendMessage(message, this.context);
                this.addMessage(msg);
            } 
            ((TextBox)sender).Text = "";
        }
        private void ShowMessages()
        {
            User u = this.context;
            LinkedList<Message> current = this.client.GetMessages(context);

            LinkedListNode<Message> msg = current.First;
            MessagesPanel.Children.Clear();
            for (int i = 0; i < current.Count; i++)
            {
                this.addMessage(msg.Value);
                msg = msg.Next;
            }
        }

        public void UpdateUI(object o)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (o is User)
                {
                    this.addUser((User)o);
                }
                else if (o is Message)
                {
                    if(this.context != null && ((Message)o).from.id == this.context.id)
                        this.addMessage((Message)o);
                }
            }));
        }

        private void addMessage(Message msg)
        {
            Style containerStyle;
            Style contentStyle;
            if (msg.from.id == this.context.id)
            {
                containerStyle = this.FindResource("message-secondary") as Style;
                contentStyle = this.FindResource("message-secondary-content") as Style;
            }
            else
            {
                containerStyle = this.FindResource("message-primary") as Style;
                contentStyle = this.FindResource("message-primary-content") as Style;
            }
            Border msgContainer = new Border();
            msgContainer.Style = containerStyle;

            TextBlock msgContent = new TextBlock();
            msgContent.Text = msg.msg;
            msgContent.Style = contentStyle;

            msgContainer.Child = msgContent;
            MessagesPanel.Children.Add(msgContainer);
            
        }
        private void addUser(User u)
        {
            //BitmapImage img = new BitmapImage(new Uri("pack://application:,,,/Resource1.xaml"));
            //ImageBrush profile = new ImageBrush(img);
            BitmapSource source = Imaging.CreateBitmapSourceFromHBitmap(Resource1.hombre_blue.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            //Creating the background
            Rectangle background = new Rectangle();
            background.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eceff1"));


            //Creating the user image
            Border border = new Border();
            border.CornerRadius = new CornerRadius(30.0);
            border.Height = 32;
            border.Width = 32;
            border.Background = new ImageBrush(source);

            //Creating the user label 
            Label name = new Label();
            if (u.nick.Length > 10)
                name.Content = u.nick.Substring(0, 10) + "...";
            else
                name.Content = u.nick;
            name.VerticalAlignment = VerticalAlignment.Center;

            //Setting the events
            MouseEventHandler mouseEnter = new MouseEventHandler((sender, e) => { this.ChangeBackground(background, "#0D000000"); Mouse.OverrideCursor = Cursors.Hand; });
            MouseEventHandler mouseLeave = new MouseEventHandler((sender, e) => { this.ChangeBackground(background, "#eceff1"); Mouse.OverrideCursor = Cursors.Arrow; });
            MouseButtonEventHandler mouseLeftClick = new MouseButtonEventHandler((sender, o) => { this.context = u; });
            background.MouseEnter += mouseEnter;
            background.MouseLeave += mouseLeave;
            background.MouseLeftButtonUp += mouseLeftClick;
            border.MouseEnter += mouseEnter;
            border.MouseLeave += mouseLeave;
            border.MouseLeftButtonUp += mouseLeftClick;
            name.MouseEnter += mouseEnter;
            name.MouseLeave += mouseLeave;
            name.MouseLeftButtonUp += mouseLeftClick;

            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(40);

            UsersContainer.RowDefinitions.Add(row);


            UsersContainer.Children.Add(background);
            UsersContainer.Children.Add(border);
            UsersContainer.Children.Add(name);

            Grid.SetRow(background, UsersContainer.RowDefinitions.Count - 1);
            Grid.SetColumn(background, 0);
            Grid.SetColumnSpan(background, 2);

            Grid.SetColumn(border, 0);
            Grid.SetRow(border, UsersContainer.RowDefinitions.Count - 1);

            Grid.SetColumn(name, 1);
            Grid.SetRow(name, UsersContainer.RowDefinitions.Count - 1);



        }

        private void ChangeBackground(object sender, string color)
        {
            Rectangle r = (Rectangle)sender;
            r.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }
    }
}
