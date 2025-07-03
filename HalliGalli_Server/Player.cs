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
        public string username = "없음";
        public bool isAlive = true;
        public bool isTurn = false;

        public TcpClient tcpClient;
        public StreamReader reader;
        public StreamWriter writer;

        public Player(int id, NetworkStream stream, TcpClient client)
        {
            this.playerId = id;
            this.reader = new StreamReader(stream);
            this.writer = new StreamWriter(stream); 
            this.tcpClient = client;

            //Todo: 유저이름 입력받기

        }

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

        // 수정된 BroadcastToAll 메서드
        public static void BroadcastToAll(Message message)
        {
            foreach (Player player in Table.Instance.players)
            {
                Broadcaster.Instance.SendJson(message, player.tcpClient.GetStream());
            }
        }
    }

}
