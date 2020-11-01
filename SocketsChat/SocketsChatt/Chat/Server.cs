using System;
using System.Collections;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using Chat;
using Serialization;

namespace ChatServer.Chat
{
    class Server
    {
        Socket socket;
        Thread listenThread;
        Hashtable usersTable;
        public Server()
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry("localhost");
                IPAddress addr = host.AddressList[0];
                IPEndPoint endPoint = new IPEndPoint(addr, 4404);

                socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(endPoint);
                socket.Listen(10);

                listenThread = new Thread(this.Listen);
                listenThread.Start();
                usersTable = new Hashtable(); //Almacenar a los usuarios cuando recibamamos una conexion 
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e.Message);
            }
        }

        /// <summary>
        /// Listo para aceptar la conexión de los clientes
        /// </summary>
        private void Listen()
        {
            Socket cliente; //conexion del cliente 
            while(true)
            {
                cliente = this.socket.Accept(); //lo asignamos a nuestro socket principal para que acepte la conexion 
                listenThread = new Thread(this.ListenClient); //Escuchando al cliente para recibir sus mensajes 
                listenThread.Start(cliente); 
            }
        }

        /// <summary>
        /// Escuchar al cliente
        /// </summary>
        /// <param name="o">Socket del cliente</param>
        private void ListenClient(object o)
        {
            Socket cliente = (Socket)o; 
            object received; //recibir el usuario con el que el cliente se va a identificar

            //do while, este sera para recibir un objeto por parte del cliente 
            do
            {
                received = this.Receive(cliente);  
            } while (!(received is User));

            this.usersTable.Add(received, cliente);
            this.BroadCast(received); //BroadCast, enviar informacion sobre el usuario que se ha conectado
            this.SendAllUsers(cliente); //Enviarle al usuario que se conecto, informacion sobre todos los usuarios que estan ya conectados actualmente

            while (true) //ver todos los mensajes que nos esta enviando 
            {
                received = this.Receive(cliente);  
                if(received is Message)
                {
                    this.SendMessage((Message) received); //SendMessage, enviar el mensaje hacia el destinatario
                }
            }


        }

        /// <summary>
        /// Enviar un objeto a todos los usuarios conectados.
        /// </summary>
        /// <param name="o">objeto para enviar</param>
        private void BroadCast(object ob) //Enviar un mensaje hacia todos los usuarios...Enviar informacion
        {
            foreach(DictionaryEntry d in this.usersTable) //Con esto se esta poniendo en un objeto de la clase "DictionaryEntry" lo que seria cada usuario que se tiene
            {
                this.Send((Socket)d.Value, ob); //
            }
        }

        /// <summary>
        /// Envíe todos los usuarios conectados al cliente
        /// </summary>
        /// <param name="s">Socket del cliente</param>
        private void SendAllUsers(Socket so) //Enviar todos los usuarios hacia un cliente 
        {
            foreach (DictionaryEntry d in this.usersTable)
            {
                this.Send(so, d.Key); //
            }
        }

        /// <summary>
        /// Envíe un mensaje al destinatario
        /// </summary>
        /// <param name="m">Mensaje para enviar</param>
        private void SendMessage(Message m) //Enviar un mesaje hacia un destinatario
        {
            User vUser;
            foreach (DictionaryEntry d in this.usersTable)
            {
                vUser = (User) d.Key; 
                if(vUser.id == m.to.id) 
                {
                    this.Send((Socket)d.Value, m);
                    break; //romper el ciclo para no iterar demas
                }
            }
        }

        /// <summary>
        /// Enviar un objeto al cliente
        /// </summary>
        /// <param name="s">Socket del cliente</param>
        /// <param name="o">Objeto para enviar</param>
        private void Send(Socket s, object o) //Recibir y enviar informacion
        {
            byte[] buffer = new byte[1024]; //enviar un paquete de cierto tamaño, en este caso puse el 1024 bytes (se puede modificar)
            byte[] obj = BinarySerialization.Serializate(o);
            Array.Copy(obj, buffer, obj.Length);
            s.Send(buffer);
        }

        /// <summary>
        /// Recibe todo el objeto serializado
        /// </summary>
        /// <param name="s">Socket que recibe el objeto</param>
        /// <returns>Objeto recibido del cliente</returns>
        private object Receive(Socket s) //recibir y enviar informacion
        {
            byte[] buffer = new byte[1024]; //enviar un paquete de cierto tamaño, en este caso puse el 1024 bytes (se puede modificar)
            s.Receive(buffer);
            return BinarySerialization.Deserializate(buffer);
        }

        
    }
}
