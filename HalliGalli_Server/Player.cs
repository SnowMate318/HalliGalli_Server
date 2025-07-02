using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HalliGalli_Server
{
    // 플레이어 클래스
    public class Player
    {
        public CardDeck deck = new();
        public int playerId;
        public string username;
        public bool isAlive = true;
        public bool isTurn = false;

        public TcpClient tcpClient;
        public StreamReader reader;
        public StreamWriter writer;

        public void ReceiveUserInfo() { }
        public void ReceiveJson() { }
        public void PlayCard() { }
        public void RingBell() { }
    }
}
