using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    public class StreamManager
    {
        public TcpClient tcpClient { get; }
        public NetworkStream stream { get; }
        public StreamReader reader { get; }
        public StreamWriter writer{ get; }

        public StreamManager(NetworkStream stream, TcpClient client)
        {
            this.stream = stream;
            this.reader = new StreamReader(stream);
            this.writer = new StreamWriter(stream, leaveOpen: true);
            this.tcpClient = client;
        }

        public void Dispose()
        {
            stream.Close();
            tcpClient.Close();
        }

    }
}
