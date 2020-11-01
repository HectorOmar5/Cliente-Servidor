using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    [Serializable]
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
