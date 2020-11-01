using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    [Serializable] //Para que se pueda enviar al cliente y que el cliente la envie tambien
    public class Message
    {
        public User from;
        public User to;
        public string msg;
        

        public Message(User from, User to, string msg)
        {
            this.from = from;
            this.to = to;
            this.msg = msg;
        }
    }
}
