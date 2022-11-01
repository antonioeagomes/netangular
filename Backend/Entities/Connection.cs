using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Entities
{
    public class Connection
    {        
        public string ConnectionId { get; set; }
        public string Username { get; set; }
        public Connection(){ }

        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }
    }
}