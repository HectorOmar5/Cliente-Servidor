using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Serialization;
using Chat;

namespace ChatUI.Chat
{
    public class Client
    {
        //Llamar a una funcion cuando se reciba una actualizacion (cuando se reciba un mensaje o un usuario )
        //Eventos
        public delegate void UpdateDelegate(object o);
        public UpdateDelegate onUpdate;

        
        //Administrador de chat
        User u;
        ChatManager chat;


        //Establecer conexion al servidor
        
        //Conexion del chat
        readonly IPHostEntry host; 
        readonly IPAddress dir; 
        readonly IPEndPoint Point; 
        readonly Socket socket;
        Thread listenThread;

        public Client(string username, UpdateDelegate onUpdate)
        {
            this.onUpdate = onUpdate;

            this.u = new User(Guid.NewGuid().ToString("N"), username);
            this.chat = new ChatManager(u);

            try
            {
                host = Dns.GetHostEntry("localhost"); //GetHostEntry, Resuelve un nombre de host o una dirección IP en una instancia de IPHostEntry
                dir = host.AddressList[0]; //Obtener la ip de nuestro host
                Point = new IPEndPoint(dir, 4404); //dir, por la cual va a escuchar.....Puerto (4404), obtenemos del constructor
                socket = new Socket(Point.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }catch(Exception e)
            {
                Console.WriteLine("{0}", e.Message);
            }
            
        }

        /// <summary>
        /// Iniciar el cliente
        /// </summary>
        public void Start()
        {
            this.socket.Connect(Point); //el cliente priemro tiene que enviar informacion de su usuario
            this.Send(this.socket, this.u); //hilo...que escuche los mensajes y usuarios que se conecten
            listenThread = new Thread(this.Listen); 
            listenThread.SetApartmentState(ApartmentState.STA); //ApartmentState...Actualizar la interfaz grafica por medio de otro hilo que no es el principal
            listenThread.Start(); //Iniciar hilo 
        }

        /// <summary>
        /// Escuche mensajes y usuarios del servidor
        /// </summary>
        private void Listen() 
        {
            object received;
            while(true)
            {
                received = this.Receive(this.socket);
                if(received is Message)
                {
                    this.AddMessage((Message)received); 
                }
                else if(received is User)
                {
                    this.AddUser((User)received);
                }
            }
        }

        /// <summary>
        /// Agregar un usuario a la base de datos del chat
        /// </summary>
        /// <param name="user"></param>
        private void AddUser(User user) //Actualizador que llama al metodo que actualiza la interfaz grafica
        {
            if(user.id != this.u.id && this.chat.AddUser(user)) //verificar que el usuario que se recibe no sea el mismo
                onUpdate(user);   
        }

        /// <summary>
        /// Agregar un mensaje a la base de datos del chat
        /// </summary>
        /// <param name="m">mensaje para agregar</param>
        private void AddMessage(Message m) //Actualizador que llama al metodo que actualiza la interfaz grafica
        {
            this.chat.AddMessage(m); //Agregamos el mensaje a la BD
            onUpdate(m);
        }

        /// <summary>
        /// Obtener todos los mensajes recibidos y enviarlos al usuario 
        /// </summary>
        /// <param name="u">Contexto de usuario</param>
        /// <returns></returns>
        public LinkedList<Message> GetMessages(User u) //Añadir mensajes a la BD
        {
            return chat.GetMessages(u);
        }

        /// <summary>
        /// Consigue todos los usuarios
        /// </summary>
        /// <returns>Usuarios</returns>
        public LinkedList<User> GetUsers() //Añadir los usuarios a la base de datos
        {
            return chat.GetUsers();
        }

        /// <summary>
        /// Envía un mensaje al servidor
        /// </summary>
        /// <param name="msg"></param>
        public Message SendMessage(string msg, User to)  
        {
            Message me = new Message(this.u, to, msg);
            this.Send(this.socket, me);
            this.AddMessage(me); 
            return me; 
        }

        /// <summary>
        /// Enviar un objeto al cliente
        /// </summary>
        /// <param name="s">Cliente del socket</param>
        /// <param name="o">Objeto para enviar</param>
        private void Send(Socket s, object o)
        {
            byte[] buffer = new byte[1024]; //
            byte[] obj = BinarySerialization.Serializate(o);
            Array.Copy(obj, buffer, obj.Length);
            s.Send(buffer);
        }

        /// <summary>
        /// Recibe todo el objeto serializado
        /// </summary>
        /// <param name="s">Socket que recibe el objeto</param>
        /// <returns>Objeto recibido del cliente</returns>
        private object Receive(Socket s)
        {
            byte[] buffer = new byte[1024];
            s.Receive(buffer);
            return BinarySerialization.Deserializate(buffer);
        }
    }
}
