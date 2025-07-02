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

        public void ReceiveUserInfo()
        {
            // 쓰레드를 실행하여 유저정보를 수신함
        }
        public void ReceiveJson()
        {
            // 사용자로부터 정보를 받음
        }
        public void PlayCard()
        {
            // Json으로 받은 정보가 카드 내기일 경우
        }
        public void RingBell()
        {
            // Json으로 받은 정보가 종 울리기일 경우
        }
    }
}
