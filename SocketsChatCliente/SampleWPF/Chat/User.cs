using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    [Serializable]
    public class User
    {
        public string id;
        public string nick;

        public User(string id, string nick)
        {
            this.id = id;
            this.nick = nick;
        }
    }
}
